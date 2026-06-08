# Migrating off SkiaSharp

The `iTextSharp.LGPLv2.Core` library no longer depends on **SkiaSharp**
(and, transitively, native assets + HarfBuzzSharp). The library is now **100% managed**
and works the same on every target framework (`net462`, `netstandard2.0`, `net6.0`–`net10.0`)
and on Linux/containers **out of the box** — no `SkiaSharp.NativeAssets.*` /
`HarfBuzzSharp.NativeAssets.*` packages and no `.so` copy steps are required anymore.

To make this possible, the small public surface that used `SkiaSharp.SKBitmap` was switched
to a new, dependency-free managed bitmap type: **`iTextSharp.text.RawBitmap`**.

---

## Breaking changes

This is a deliberate breaking change. Only the optional *raster* surface is affected — the
core PDF generation, image decoding (PNG/GIF/BMP/TIFF/JBIG2), fonts, and the **vector**
barcode path (`PlaceBarcode` / `CreateImageWithBarcode`) are unchanged.

| Before | After |
| --- | --- |
| `Barcode.CreateDrawingImage(Color, Color)` returns `SKBitmap` | returns `RawBitmap` |
| `Image.GetInstance(SKBitmap, SKEncodedImageFormat, int quality)` | `Image.GetInstance(RawBitmap)` (direct, lossless) |
| `Image.GetInstance(SKBitmap, BaseColor, bool forceBw)` | `Image.GetInstance(RawBitmap, BaseColor, bool forceBw)` |
| `Image.GetInstance(SKBitmap, BaseColor)` | `Image.GetInstance(RawBitmap, BaseColor)` |
| `SkiaSharpExtensions.ToSKColor()` / `.ToArgb()` | removed (no longer needed) |
| `SkiaSharp` + native-asset packages in your `.csproj` | can be removed |

> The old `GetInstance(SKBitmap, SKEncodedImageFormat, quality)` re-encoded the bitmap to
> PNG/JPEG and then decoded it again. The new `GetInstance(RawBitmap)` embeds the pixels
> directly (lossless), so the `format`/`quality` arguments no longer exist.

---

## The `RawBitmap` type

`iTextSharp.text.RawBitmap` is a minimal 32-bit ARGB bitmap. Pixels are stored row-major in
**ARGB** order — the exact same layout as `System.Drawing.Color.ToArgb()` (`0xAARRGGBB`).

```csharp
namespace iTextSharp.text;

public sealed class RawBitmap
{
    public RawBitmap(int width, int height);

    public int Width  { get; }
    public int Height { get; }

    public void  SetPixel(int x, int y, System.Drawing.Color color);
    public System.Drawing.Color GetPixel(int x, int y);

    // A copy of the raw pixel buffer, row-major, in ARGB (0xAARRGGBB) order.
    public int[] GetArgbPixels();

    // Builds a bitmap from a row-major ARGB buffer (the buffer is copied).
    public static RawBitmap FromArgb(int width, int height, int[] argbPixels);
}
```

The library intentionally does **not** ship an image encoder: turning a `RawBitmap` into a
concrete format (PNG, JPEG, `SKBitmap`, `System.Drawing.Bitmap`, …) is left to you, using the
pixel accessors above. The helpers below show exactly how.

---

## You usually don't need to convert anything

If your goal is simply to place a barcode/bitmap **into a PDF**, no conversion is required:

```csharp
using iTextSharp.text;
using iTextSharp.text.pdf;

// Vector barcode straight into the PDF (recommended — sharp at any zoom):
barcode.PlaceBarcode(contentByte, barColor: null, textColor: null);
// or
document.Add(barcode.CreateImageWithBarcode(contentByte, null, null));

// Or take the raster bitmap and embed it as a PDF image:
RawBitmap raster = barcode.CreateDrawingImage(Color.Black, Color.White);
Image pdfImage = Image.GetInstance(raster);
document.Add(pdfImage);
```

Conversions to `SKBitmap` / `System.Drawing.Bitmap` are only needed when you want to **save the
raster to a file** or hand it to another imaging library.

---

## Convert `RawBitmap` → `SkiaSharp.SKBitmap`

Add the `SkiaSharp` NuGet package to **your** project (it's no longer pulled in by the library).

```csharp
using SkiaSharp;
using iTextSharp.text;

public static SKBitmap ToSKBitmap(RawBitmap source)
{
    var bitmap = new SKBitmap(source.Width, source.Height);

    for (var y = 0; y < source.Height; y++)
    {
        for (var x = 0; x < source.Width; x++)
        {
            var c = source.GetPixel(x, y); // System.Drawing.Color
            bitmap.SetPixel(x, y, new SKColor(c.R, c.G, c.B, c.A));
        }
    }

    return bitmap;
}
```

Faster, bulk variant (avoids per-pixel calls). `RawBitmap` already exposes ARGB ints in the
`0xAARRGGBB` layout that `SKColor` expects:

```csharp
using SkiaSharp;
using iTextSharp.text;

public static SKBitmap ToSKBitmapFast(RawBitmap source)
{
    var bitmap = new SKBitmap(source.Width, source.Height);
    var argb = source.GetArgbPixels();           // 0xAARRGGBB per pixel

    var colors = new SKColor[argb.Length];
    for (var i = 0; i < argb.Length; i++)
    {
        colors[i] = (uint)argb[i];               // implicit uint(0xAARRGGBB) -> SKColor
    }

    bitmap.Pixels = colors;
    return bitmap;
}

// Example: save a barcode as PNG
RawBitmap raster = barcode.CreateDrawingImage(Color.Black, Color.White);
using var sk = ToSKBitmap(raster);
using var data = sk.Encode(SKEncodedImageFormat.Png, quality: 100);
using var fs = File.OpenWrite("barcode.png");
data.SaveTo(fs);
```

---

## Convert `RawBitmap` → `System.Drawing.Bitmap`

> **Platform note:** `System.Drawing.Bitmap` lives in the **`System.Drawing.Common`** NuGet
> package, which is **Windows-only** since .NET 6 (it throws `PlatformNotSupportedException`
> on Linux/macOS). Use this path only on Windows; otherwise prefer the SkiaSharp path above,
> which is cross-platform.

```csharp
using System.Drawing;
using System.Drawing.Imaging;
using iTextSharp.text;

public static Bitmap ToBitmap(RawBitmap source)
{
    var bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);

    for (var y = 0; y < source.Height; y++)
    {
        for (var x = 0; x < source.Width; x++)
        {
            // RawBitmap.GetPixel already returns a System.Drawing.Color, so this is a direct copy.
            bitmap.SetPixel(x, y, source.GetPixel(x, y));
        }
    }

    return bitmap;
}
```

Faster, bulk variant via `LockBits`. The `0xAARRGGBB` ints map directly onto
`PixelFormat.Format32bppArgb`'s in-memory layout:

```csharp
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using iTextSharp.text;

public static Bitmap ToBitmapFast(RawBitmap source)
{
    var bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
    var argb = source.GetArgbPixels();
    var rect = new Rectangle(0, 0, source.Width, source.Height);

    var data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
    try
    {
        if (data.Stride == source.Width * 4)
        {
            // No row padding: copy the whole buffer at once.
            Marshal.Copy(argb, 0, data.Scan0, argb.Length);
        }
        else
        {
            // Defensive: copy row by row to honor the bitmap's stride.
            for (var y = 0; y < source.Height; y++)
            {
                Marshal.Copy(argb, y * source.Width, data.Scan0 + (y * data.Stride), source.Width);
            }
        }
    }
    finally
    {
        bitmap.UnlockBits(data);
    }

    return bitmap;
}

// Example: save a barcode as PNG (Windows)
RawBitmap raster = barcode.CreateDrawingImage(Color.Black, Color.White);
using var bmp = ToBitmap(raster);
bmp.Save("barcode.png", ImageFormat.Png);
```

---

## Reverse conversions (migrating existing code)

If your v3.x code obtained an `SKBitmap` (or `System.Drawing.Bitmap`) elsewhere and fed it to
`Image.GetInstance(...)`, convert it to a `RawBitmap` first.

### `SKBitmap` → `RawBitmap`

```csharp
using SkiaSharp;
using iTextSharp.text;

public static RawBitmap ToRawBitmap(SKBitmap source)
{
    var argb = new int[source.Width * source.Height];

    var i = 0;
    for (var y = 0; y < source.Height; y++)
    {
        for (var x = 0; x < source.Width; x++)
        {
            var c = source.GetPixel(x, y);
            argb[i++] = (c.Alpha << 24) | (c.Red << 16) | (c.Green << 8) | c.Blue;
        }
    }

    return RawBitmap.FromArgb(source.Width, source.Height, argb);
}

// Before: Image.GetInstance(skBitmap, SKEncodedImageFormat.Png);
// After:
Image pdfImage = Image.GetInstance(ToRawBitmap(skBitmap));
```

### `System.Drawing.Bitmap` → `RawBitmap` (Windows)

```csharp
using System.Drawing;
using iTextSharp.text;

public static RawBitmap ToRawBitmap(Bitmap source)
{
    var argb = new int[source.Width * source.Height];

    var i = 0;
    for (var y = 0; y < source.Height; y++)
    {
        for (var x = 0; x < source.Width; x++)
        {
            argb[i++] = source.GetPixel(x, y).ToArgb(); // System.Drawing.Color.ToArgb() == 0xAARRGGBB
        }
    }

    return RawBitmap.FromArgb(source.Width, source.Height, argb);
}
```

---

## Why this change?

SkiaSharp ships native binaries (`libSkiaSharp`, `libHarfBuzzSharp`). On Linux and in
containers this forced consumers to add extra native-asset packages and copy `.so` files via
custom MSBuild targets — a recurring source of deployment friction — for what was a very small
feature surface. Replacing it with a tiny managed `RawBitmap` removes those native dependencies
entirely while keeping behavior identical across all target frameworks.
