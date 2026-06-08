using System.Drawing;

namespace iTextSharp.text;

/// <summary>
///     A minimal, fully-managed 32-bit ARGB bitmap. It replaces the previous
///     dependency on SkiaSharp's <c>SKBitmap</c> for the (optional) raster output
///     of barcodes and for <see cref="Image" /> conversions.
///     Pixels are stored row-major in ARGB order, matching
///     <see cref="System.Drawing.Color.ToArgb" />. Consumers can read the raw
///     pixels through <see cref="GetPixel" /> / <see cref="GetArgbPixels" /> and
///     convert them to any concrete image format they need (PNG, JPEG, SKBitmap,
///     System.Drawing.Bitmap, ...).
/// </summary>
public sealed class RawBitmap
{
    private readonly int[] _argb;

    /// <summary>
    ///     Creates a new bitmap with the given dimensions. All pixels start fully
    ///     transparent (ARGB value 0).
    /// </summary>
    /// <param name="width">the width in pixels (must be positive)</param>
    /// <param name="height">the height in pixels (must be positive)</param>
    public RawBitmap(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        Width = width;
        Height = height;
        _argb = new int[width * height];
    }

    private RawBitmap(int width, int height, int[] argb)
    {
        Width = width;
        Height = height;
        _argb = argb;
    }

    /// <summary>
    ///     The width of the bitmap, in pixels.
    /// </summary>
    public int Width { get; }

    /// <summary>
    ///     The height of the bitmap, in pixels.
    /// </summary>
    public int Height { get; }

    /// <summary>
    ///     Sets the color of the pixel at the given coordinates.
    /// </summary>
    public void SetPixel(int x, int y, Color color) => _argb[(y * Width) + x] = color.ToArgb();

    /// <summary>
    ///     Gets the color of the pixel at the given coordinates.
    /// </summary>
    public Color GetPixel(int x, int y) => Color.FromArgb(_argb[(y * Width) + x]);

    /// <summary>
    ///     Returns a copy of the raw pixel buffer, row-major, in ARGB order (the
    ///     same layout as <see cref="System.Drawing.Color.ToArgb" />).
    /// </summary>
    public int[] GetArgbPixels()
    {
        var copy = new int[_argb.Length];
        Array.Copy(_argb, copy, _argb.Length);

        return copy;
    }

    /// <summary>
    ///     Creates a bitmap from a row-major ARGB pixel buffer. The buffer is copied.
    /// </summary>
    /// <param name="width">the width in pixels (must be positive)</param>
    /// <param name="height">the height in pixels (must be positive)</param>
    /// <param name="argbPixels">the source pixels, row-major, in ARGB order</param>
    public static RawBitmap FromArgb(int width, int height, int[] argbPixels)
    {
        if (argbPixels == null)
        {
            throw new ArgumentNullException(nameof(argbPixels));
        }

        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height));
        }

        if (argbPixels.Length < width * height)
        {
            throw new ArgumentException(message: "The pixel buffer is smaller than width * height.",
                nameof(argbPixels));
        }

        var copy = new int[width * height];
        Array.Copy(argbPixels, copy, copy.Length);

        return new RawBitmap(width, height, copy);
    }
}
