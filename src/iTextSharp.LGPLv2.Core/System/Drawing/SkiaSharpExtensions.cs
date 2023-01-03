using System.Drawing;
using SkiaSharp;

namespace iTextSharp.LGPLv2.Core.System.Drawing;

public static class SkiaSharpExtensions
{
    public static SKColor ToSKColor(this Color color) => (uint)color.ToArgb();

    public static int ToArgb(this SKColor color) => (int)(uint)color;
}