using System.util;

namespace iTextSharp.text.html;

/// <summary>
///     This class is a HashMap that contains the names of colors as a key and the
///     corresponding Color as value. (Source: Wikipedia
///     http://en.wikipedia.org/wiki/Web_colors )
///     @author blowagie
/// </summary>
public class WebColors : NullValueDictionary<string, int[]>
{
    public static WebColors Names = new();

    static WebColors()
    {
        Names["aliceblue"] = new[] { 0xf0, 0xf8, 0xff, 0x00 };
        Names["antiquewhite"] = new[] { 0xfa, 0xeb, 0xd7, 0x00 };
        Names["aqua"] = new[] { 0x00, 0xff, 0xff, 0x00 };
        Names["aquamarine"] = new[] { 0x7f, 0xff, 0xd4, 0x00 };
        Names["azure"] = new[] { 0xf0, 0xff, 0xff, 0x00 };
        Names["beige"] = new[] { 0xf5, 0xf5, 0xdc, 0x00 };
        Names["bisque"] = new[] { 0xff, 0xe4, 0xc4, 0x00 };
        Names["black"] = new[] { 0x00, 0x00, 0x00, 0x00 };
        Names["blanchedalmond"] = new[] { 0xff, 0xeb, 0xcd, 0x00 };
        Names["blue"] = new[] { 0x00, 0x00, 0xff, 0x00 };
        Names["blueviolet"] = new[] { 0x8a, 0x2b, 0xe2, 0x00 };
        Names["brown"] = new[] { 0xa5, 0x2a, 0x2a, 0x00 };
        Names["burlywood"] = new[] { 0xde, 0xb8, 0x87, 0x00 };
        Names["cadetblue"] = new[] { 0x5f, 0x9e, 0xa0, 0x00 };
        Names["chartreuse"] = new[] { 0x7f, 0xff, 0x00, 0x00 };
        Names["chocolate"] = new[] { 0xd2, 0x69, 0x1e, 0x00 };
        Names["coral"] = new[] { 0xff, 0x7f, 0x50, 0x00 };
        Names["cornflowerblue"] = new[] { 0x64, 0x95, 0xed, 0x00 };
        Names["cornsilk"] = new[] { 0xff, 0xf8, 0xdc, 0x00 };
        Names["crimson"] = new[] { 0xdc, 0x14, 0x3c, 0x00 };
        Names["cyan"] = new[] { 0x00, 0xff, 0xff, 0x00 };
        Names["darkblue"] = new[] { 0x00, 0x00, 0x8b, 0x00 };
        Names["darkcyan"] = new[] { 0x00, 0x8b, 0x8b, 0x00 };
        Names["darkgoldenrod"] = new[] { 0xb8, 0x86, 0x0b, 0x00 };
        Names["darkgray"] = new[] { 0xa9, 0xa9, 0xa9, 0x00 };
        Names["darkgreen"] = new[] { 0x00, 0x64, 0x00, 0x00 };
        Names["darkkhaki"] = new[] { 0xbd, 0xb7, 0x6b, 0x00 };
        Names["darkmagenta"] = new[] { 0x8b, 0x00, 0x8b, 0x00 };
        Names["darkolivegreen"] = new[] { 0x55, 0x6b, 0x2f, 0x00 };
        Names["darkorange"] = new[] { 0xff, 0x8c, 0x00, 0x00 };
        Names["darkorchid"] = new[] { 0x99, 0x32, 0xcc, 0x00 };
        Names["darkred"] = new[] { 0x8b, 0x00, 0x00, 0x00 };
        Names["darksalmon"] = new[] { 0xe9, 0x96, 0x7a, 0x00 };
        Names["darkseagreen"] = new[] { 0x8f, 0xbc, 0x8f, 0x00 };
        Names["darkslateblue"] = new[] { 0x48, 0x3d, 0x8b, 0x00 };
        Names["darkslategray"] = new[] { 0x2f, 0x4f, 0x4f, 0x00 };
        Names["darkturquoise"] = new[] { 0x00, 0xce, 0xd1, 0x00 };
        Names["darkviolet"] = new[] { 0x94, 0x00, 0xd3, 0x00 };
        Names["deeppink"] = new[] { 0xff, 0x14, 0x93, 0x00 };
        Names["deepskyblue"] = new[] { 0x00, 0xbf, 0xff, 0x00 };
        Names["dimgray"] = new[] { 0x69, 0x69, 0x69, 0x00 };
        Names["dodgerblue"] = new[] { 0x1e, 0x90, 0xff, 0x00 };
        Names["firebrick"] = new[] { 0xb2, 0x22, 0x22, 0x00 };
        Names["floralwhite"] = new[] { 0xff, 0xfa, 0xf0, 0x00 };
        Names["forestgreen"] = new[] { 0x22, 0x8b, 0x22, 0x00 };
        Names["fuchsia"] = new[] { 0xff, 0x00, 0xff, 0x00 };
        Names["gainsboro"] = new[] { 0xdc, 0xdc, 0xdc, 0x00 };
        Names["ghostwhite"] = new[] { 0xf8, 0xf8, 0xff, 0x00 };
        Names["gold"] = new[] { 0xff, 0xd7, 0x00, 0x00 };
        Names["goldenrod"] = new[] { 0xda, 0xa5, 0x20, 0x00 };
        Names["gray"] = new[] { 0x80, 0x80, 0x80, 0x00 };
        Names["green"] = new[] { 0x00, 0x80, 0x00, 0x00 };
        Names["greenyellow"] = new[] { 0xad, 0xff, 0x2f, 0x00 };
        Names["honeydew"] = new[] { 0xf0, 0xff, 0xf0, 0x00 };
        Names["hotpink"] = new[] { 0xff, 0x69, 0xb4, 0x00 };
        Names["indianred"] = new[] { 0xcd, 0x5c, 0x5c, 0x00 };
        Names["indigo"] = new[] { 0x4b, 0x00, 0x82, 0x00 };
        Names["ivory"] = new[] { 0xff, 0xff, 0xf0, 0x00 };
        Names["khaki"] = new[] { 0xf0, 0xe6, 0x8c, 0x00 };
        Names["lavender"] = new[] { 0xe6, 0xe6, 0xfa, 0x00 };
        Names["lavenderblush"] = new[] { 0xff, 0xf0, 0xf5, 0x00 };
        Names["lawngreen"] = new[] { 0x7c, 0xfc, 0x00, 0x00 };
        Names["lemonchiffon"] = new[] { 0xff, 0xfa, 0xcd, 0x00 };
        Names["lightblue"] = new[] { 0xad, 0xd8, 0xe6, 0x00 };
        Names["lightcoral"] = new[] { 0xf0, 0x80, 0x80, 0x00 };
        Names["lightcyan"] = new[] { 0xe0, 0xff, 0xff, 0x00 };
        Names["lightgoldenrodyellow"] = new[] { 0xfa, 0xfa, 0xd2, 0x00 };
        Names["lightgreen"] = new[] { 0x90, 0xee, 0x90, 0x00 };
        Names["lightgrey"] = new[] { 0xd3, 0xd3, 0xd3, 0x00 };
        Names["lightpink"] = new[] { 0xff, 0xb6, 0xc1, 0x00 };
        Names["lightsalmon"] = new[] { 0xff, 0xa0, 0x7a, 0x00 };
        Names["lightseagreen"] = new[] { 0x20, 0xb2, 0xaa, 0x00 };
        Names["lightskyblue"] = new[] { 0x87, 0xce, 0xfa, 0x00 };
        Names["lightslategray"] = new[] { 0x77, 0x88, 0x99, 0x00 };
        Names["lightsteelblue"] = new[] { 0xb0, 0xc4, 0xde, 0x00 };
        Names["lightyellow"] = new[] { 0xff, 0xff, 0xe0, 0x00 };
        Names["lime"] = new[] { 0x00, 0xff, 0x00, 0x00 };
        Names["limegreen"] = new[] { 0x32, 0xcd, 0x32, 0x00 };
        Names["linen"] = new[] { 0xfa, 0xf0, 0xe6, 0x00 };
        Names["magenta"] = new[] { 0xff, 0x00, 0xff, 0x00 };
        Names["maroon"] = new[] { 0x80, 0x00, 0x00, 0x00 };
        Names["mediumaquamarine"] = new[] { 0x66, 0xcd, 0xaa, 0x00 };
        Names["mediumblue"] = new[] { 0x00, 0x00, 0xcd, 0x00 };
        Names["mediumorchid"] = new[] { 0xba, 0x55, 0xd3, 0x00 };
        Names["mediumpurple"] = new[] { 0x93, 0x70, 0xdb, 0x00 };
        Names["mediumseagreen"] = new[] { 0x3c, 0xb3, 0x71, 0x00 };
        Names["mediumslateblue"] = new[] { 0x7b, 0x68, 0xee, 0x00 };
        Names["mediumspringgreen"] = new[] { 0x00, 0xfa, 0x9a, 0x00 };
        Names["mediumturquoise"] = new[] { 0x48, 0xd1, 0xcc, 0x00 };
        Names["mediumvioletred"] = new[] { 0xc7, 0x15, 0x85, 0x00 };
        Names["midnightblue"] = new[] { 0x19, 0x19, 0x70, 0x00 };
        Names["mintcream"] = new[] { 0xf5, 0xff, 0xfa, 0x00 };
        Names["mistyrose"] = new[] { 0xff, 0xe4, 0xe1, 0x00 };
        Names["moccasin"] = new[] { 0xff, 0xe4, 0xb5, 0x00 };
        Names["navajowhite"] = new[] { 0xff, 0xde, 0xad, 0x00 };
        Names["navy"] = new[] { 0x00, 0x00, 0x80, 0x00 };
        Names["oldlace"] = new[] { 0xfd, 0xf5, 0xe6, 0x00 };
        Names["olive"] = new[] { 0x80, 0x80, 0x00, 0x00 };
        Names["olivedrab"] = new[] { 0x6b, 0x8e, 0x23, 0x00 };
        Names["orange"] = new[] { 0xff, 0xa5, 0x00, 0x00 };
        Names["orangered"] = new[] { 0xff, 0x45, 0x00, 0x00 };
        Names["orchid"] = new[] { 0xda, 0x70, 0xd6, 0x00 };
        Names["palegoldenrod"] = new[] { 0xee, 0xe8, 0xaa, 0x00 };
        Names["palegreen"] = new[] { 0x98, 0xfb, 0x98, 0x00 };
        Names["paleturquoise"] = new[] { 0xaf, 0xee, 0xee, 0x00 };
        Names["palevioletred"] = new[] { 0xdb, 0x70, 0x93, 0x00 };
        Names["papayawhip"] = new[] { 0xff, 0xef, 0xd5, 0x00 };
        Names["peachpuff"] = new[] { 0xff, 0xda, 0xb9, 0x00 };
        Names["peru"] = new[] { 0xcd, 0x85, 0x3f, 0x00 };
        Names["pink"] = new[] { 0xff, 0xc0, 0xcb, 0x00 };
        Names["plum"] = new[] { 0xdd, 0xa0, 0xdd, 0x00 };
        Names["powderblue"] = new[] { 0xb0, 0xe0, 0xe6, 0x00 };
        Names["purple"] = new[] { 0x80, 0x00, 0x80, 0x00 };
        Names["red"] = new[] { 0xff, 0x00, 0x00, 0x00 };
        Names["rosybrown"] = new[] { 0xbc, 0x8f, 0x8f, 0x00 };
        Names["royalblue"] = new[] { 0x41, 0x69, 0xe1, 0x00 };
        Names["saddlebrown"] = new[] { 0x8b, 0x45, 0x13, 0x00 };
        Names["salmon"] = new[] { 0xfa, 0x80, 0x72, 0x00 };
        Names["sandybrown"] = new[] { 0xf4, 0xa4, 0x60, 0x00 };
        Names["seagreen"] = new[] { 0x2e, 0x8b, 0x57, 0x00 };
        Names["seashell"] = new[] { 0xff, 0xf5, 0xee, 0x00 };
        Names["sienna"] = new[] { 0xa0, 0x52, 0x2d, 0x00 };
        Names["silver"] = new[] { 0xc0, 0xc0, 0xc0, 0x00 };
        Names["skyblue"] = new[] { 0x87, 0xce, 0xeb, 0x00 };
        Names["slateblue"] = new[] { 0x6a, 0x5a, 0xcd, 0x00 };
        Names["slategray"] = new[] { 0x70, 0x80, 0x90, 0x00 };
        Names["snow"] = new[] { 0xff, 0xfa, 0xfa, 0x00 };
        Names["springgreen"] = new[] { 0x00, 0xff, 0x7f, 0x00 };
        Names["steelblue"] = new[] { 0x46, 0x82, 0xb4, 0x00 };
        Names["tan"] = new[] { 0xd2, 0xb4, 0x8c, 0x00 };
        Names["transparent"] = new[] { 0x00, 0x00, 0x00, 0xff };
        Names["teal"] = new[] { 0x00, 0x80, 0x80, 0x00 };
        Names["thistle"] = new[] { 0xd8, 0xbf, 0xd8, 0x00 };
        Names["tomato"] = new[] { 0xff, 0x63, 0x47, 0x00 };
        Names["turquoise"] = new[] { 0x40, 0xe0, 0xd0, 0x00 };
        Names["violet"] = new[] { 0xee, 0x82, 0xee, 0x00 };
        Names["wheat"] = new[] { 0xf5, 0xde, 0xb3, 0x00 };
        Names["white"] = new[] { 0xff, 0xff, 0xff, 0x00 };
        Names["whitesmoke"] = new[] { 0xf5, 0xf5, 0xf5, 0x00 };
        Names["yellow"] = new[] { 0xff, 0xff, 0x00, 0x00 };
        Names["yellowgreen"] = new[] { 0x9, 0xacd, 0x32, 0x00 };
    }

    /// <summary>
    ///     Gives you a Color based on a name.
    ///     a name such as black, violet, cornflowerblue or #RGB or #RRGGBB
    ///     or rgb(R,G,B)
    ///     @throws IllegalArgumentException
    ///     if the String isn't a know representation of a color.
    /// </summary>
    /// <param name="name"></param>
    /// <returns>the corresponding Color object</returns>
    public static BaseColor GetRgbColor(string name)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        int[] c = { 0, 0, 0, 0 };
        if (name.StartsWith("#", StringComparison.OrdinalIgnoreCase))
        {
            if (name.Length == 4)
            {
                c[0] = int.Parse(name.Substring(1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture) * 16;
                c[1] = int.Parse(name.Substring(2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture) * 16;
                c[2] = int.Parse(name.Substring(3), NumberStyles.HexNumber, CultureInfo.InvariantCulture) * 16;
                return new BaseColor(c[0], c[1], c[2], c[3]);
            }

            if (name.Length == 7)
            {
                c[0] = int.Parse(name.Substring(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                c[1] = int.Parse(name.Substring(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                c[2] = int.Parse(name.Substring(5), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                return new BaseColor(c[0], c[1], c[2], c[3]);
            }

            throw new ArgumentException(
                                        "Unknown color format. Must be #RGB or #RRGGBB");
        }

        if (name.StartsWith("rgb(", StringComparison.OrdinalIgnoreCase))
        {
            var tok = new StringTokenizer(name, "rgb(), \t\r\n\f");
            for (var k = 0; k < 3; ++k)
            {
                var v = tok.NextToken();
                if (v.EndsWith("%", StringComparison.OrdinalIgnoreCase))
                {
                    c[k] = int.Parse(v.Substring(0, v.Length - 1), CultureInfo.InvariantCulture) * 255 / 100;
                }
                else
                {
                    c[k] = int.Parse(v, CultureInfo.InvariantCulture);
                }

                if (c[k] < 0)
                {
                    c[k] = 0;
                }
                else if (c[k] > 255)
                {
                    c[k] = 255;
                }
            }

            return new BaseColor(c[0], c[1], c[2], c[3]);
        }

        name = name.ToLower(CultureInfo.InvariantCulture);
        if (!Names.TryGetValue(name, out var color))
        {
            throw new ArgumentException($"Color '{name}' not found.");
        }

        c = color;
        return new BaseColor(c[0], c[1], c[2], c[3]);
    }
}