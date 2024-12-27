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
        Names[key: "aliceblue"] = new[]
        {
            0xf0, 0xf8, 0xff, 0x00
        };

        Names[key: "antiquewhite"] = new[]
        {
            0xfa, 0xeb, 0xd7, 0x00
        };

        Names[key: "aqua"] = new[]
        {
            0x00, 0xff, 0xff, 0x00
        };

        Names[key: "aquamarine"] = new[]
        {
            0x7f, 0xff, 0xd4, 0x00
        };

        Names[key: "azure"] = new[]
        {
            0xf0, 0xff, 0xff, 0x00
        };

        Names[key: "beige"] = new[]
        {
            0xf5, 0xf5, 0xdc, 0x00
        };

        Names[key: "bisque"] = new[]
        {
            0xff, 0xe4, 0xc4, 0x00
        };

        Names[key: "black"] = new[]
        {
            0x00, 0x00, 0x00, 0x00
        };

        Names[key: "blanchedalmond"] = new[]
        {
            0xff, 0xeb, 0xcd, 0x00
        };

        Names[key: "blue"] = new[]
        {
            0x00, 0x00, 0xff, 0x00
        };

        Names[key: "blueviolet"] = new[]
        {
            0x8a, 0x2b, 0xe2, 0x00
        };

        Names[key: "brown"] = new[]
        {
            0xa5, 0x2a, 0x2a, 0x00
        };

        Names[key: "burlywood"] = new[]
        {
            0xde, 0xb8, 0x87, 0x00
        };

        Names[key: "cadetblue"] = new[]
        {
            0x5f, 0x9e, 0xa0, 0x00
        };

        Names[key: "chartreuse"] = new[]
        {
            0x7f, 0xff, 0x00, 0x00
        };

        Names[key: "chocolate"] = new[]
        {
            0xd2, 0x69, 0x1e, 0x00
        };

        Names[key: "coral"] = new[]
        {
            0xff, 0x7f, 0x50, 0x00
        };

        Names[key: "cornflowerblue"] = new[]
        {
            0x64, 0x95, 0xed, 0x00
        };

        Names[key: "cornsilk"] = new[]
        {
            0xff, 0xf8, 0xdc, 0x00
        };

        Names[key: "crimson"] = new[]
        {
            0xdc, 0x14, 0x3c, 0x00
        };

        Names[key: "cyan"] = new[]
        {
            0x00, 0xff, 0xff, 0x00
        };

        Names[key: "darkblue"] = new[]
        {
            0x00, 0x00, 0x8b, 0x00
        };

        Names[key: "darkcyan"] = new[]
        {
            0x00, 0x8b, 0x8b, 0x00
        };

        Names[key: "darkgoldenrod"] = new[]
        {
            0xb8, 0x86, 0x0b, 0x00
        };

        Names[key: "darkgray"] = new[]
        {
            0xa9, 0xa9, 0xa9, 0x00
        };

        Names[key: "darkgreen"] = new[]
        {
            0x00, 0x64, 0x00, 0x00
        };

        Names[key: "darkkhaki"] = new[]
        {
            0xbd, 0xb7, 0x6b, 0x00
        };

        Names[key: "darkmagenta"] = new[]
        {
            0x8b, 0x00, 0x8b, 0x00
        };

        Names[key: "darkolivegreen"] = new[]
        {
            0x55, 0x6b, 0x2f, 0x00
        };

        Names[key: "darkorange"] = new[]
        {
            0xff, 0x8c, 0x00, 0x00
        };

        Names[key: "darkorchid"] = new[]
        {
            0x99, 0x32, 0xcc, 0x00
        };

        Names[key: "darkred"] = new[]
        {
            0x8b, 0x00, 0x00, 0x00
        };

        Names[key: "darksalmon"] = new[]
        {
            0xe9, 0x96, 0x7a, 0x00
        };

        Names[key: "darkseagreen"] = new[]
        {
            0x8f, 0xbc, 0x8f, 0x00
        };

        Names[key: "darkslateblue"] = new[]
        {
            0x48, 0x3d, 0x8b, 0x00
        };

        Names[key: "darkslategray"] = new[]
        {
            0x2f, 0x4f, 0x4f, 0x00
        };

        Names[key: "darkturquoise"] = new[]
        {
            0x00, 0xce, 0xd1, 0x00
        };

        Names[key: "darkviolet"] = new[]
        {
            0x94, 0x00, 0xd3, 0x00
        };

        Names[key: "deeppink"] = new[]
        {
            0xff, 0x14, 0x93, 0x00
        };

        Names[key: "deepskyblue"] = new[]
        {
            0x00, 0xbf, 0xff, 0x00
        };

        Names[key: "dimgray"] = new[]
        {
            0x69, 0x69, 0x69, 0x00
        };

        Names[key: "dodgerblue"] = new[]
        {
            0x1e, 0x90, 0xff, 0x00
        };

        Names[key: "firebrick"] = new[]
        {
            0xb2, 0x22, 0x22, 0x00
        };

        Names[key: "floralwhite"] = new[]
        {
            0xff, 0xfa, 0xf0, 0x00
        };

        Names[key: "forestgreen"] = new[]
        {
            0x22, 0x8b, 0x22, 0x00
        };

        Names[key: "fuchsia"] = new[]
        {
            0xff, 0x00, 0xff, 0x00
        };

        Names[key: "gainsboro"] = new[]
        {
            0xdc, 0xdc, 0xdc, 0x00
        };

        Names[key: "ghostwhite"] = new[]
        {
            0xf8, 0xf8, 0xff, 0x00
        };

        Names[key: "gold"] = new[]
        {
            0xff, 0xd7, 0x00, 0x00
        };

        Names[key: "goldenrod"] = new[]
        {
            0xda, 0xa5, 0x20, 0x00
        };

        Names[key: "gray"] = new[]
        {
            0x80, 0x80, 0x80, 0x00
        };

        Names[key: "green"] = new[]
        {
            0x00, 0x80, 0x00, 0x00
        };

        Names[key: "greenyellow"] = new[]
        {
            0xad, 0xff, 0x2f, 0x00
        };

        Names[key: "honeydew"] = new[]
        {
            0xf0, 0xff, 0xf0, 0x00
        };

        Names[key: "hotpink"] = new[]
        {
            0xff, 0x69, 0xb4, 0x00
        };

        Names[key: "indianred"] = new[]
        {
            0xcd, 0x5c, 0x5c, 0x00
        };

        Names[key: "indigo"] = new[]
        {
            0x4b, 0x00, 0x82, 0x00
        };

        Names[key: "ivory"] = new[]
        {
            0xff, 0xff, 0xf0, 0x00
        };

        Names[key: "khaki"] = new[]
        {
            0xf0, 0xe6, 0x8c, 0x00
        };

        Names[key: "lavender"] = new[]
        {
            0xe6, 0xe6, 0xfa, 0x00
        };

        Names[key: "lavenderblush"] = new[]
        {
            0xff, 0xf0, 0xf5, 0x00
        };

        Names[key: "lawngreen"] = new[]
        {
            0x7c, 0xfc, 0x00, 0x00
        };

        Names[key: "lemonchiffon"] = new[]
        {
            0xff, 0xfa, 0xcd, 0x00
        };

        Names[key: "lightblue"] = new[]
        {
            0xad, 0xd8, 0xe6, 0x00
        };

        Names[key: "lightcoral"] = new[]
        {
            0xf0, 0x80, 0x80, 0x00
        };

        Names[key: "lightcyan"] = new[]
        {
            0xe0, 0xff, 0xff, 0x00
        };

        Names[key: "lightgoldenrodyellow"] = new[]
        {
            0xfa, 0xfa, 0xd2, 0x00
        };

        Names[key: "lightgreen"] = new[]
        {
            0x90, 0xee, 0x90, 0x00
        };

        Names[key: "lightgrey"] = new[]
        {
            0xd3, 0xd3, 0xd3, 0x00
        };

        Names[key: "lightpink"] = new[]
        {
            0xff, 0xb6, 0xc1, 0x00
        };

        Names[key: "lightsalmon"] = new[]
        {
            0xff, 0xa0, 0x7a, 0x00
        };

        Names[key: "lightseagreen"] = new[]
        {
            0x20, 0xb2, 0xaa, 0x00
        };

        Names[key: "lightskyblue"] = new[]
        {
            0x87, 0xce, 0xfa, 0x00
        };

        Names[key: "lightslategray"] = new[]
        {
            0x77, 0x88, 0x99, 0x00
        };

        Names[key: "lightsteelblue"] = new[]
        {
            0xb0, 0xc4, 0xde, 0x00
        };

        Names[key: "lightyellow"] = new[]
        {
            0xff, 0xff, 0xe0, 0x00
        };

        Names[key: "lime"] = new[]
        {
            0x00, 0xff, 0x00, 0x00
        };

        Names[key: "limegreen"] = new[]
        {
            0x32, 0xcd, 0x32, 0x00
        };

        Names[key: "linen"] = new[]
        {
            0xfa, 0xf0, 0xe6, 0x00
        };

        Names[key: "magenta"] = new[]
        {
            0xff, 0x00, 0xff, 0x00
        };

        Names[key: "maroon"] = new[]
        {
            0x80, 0x00, 0x00, 0x00
        };

        Names[key: "mediumaquamarine"] = new[]
        {
            0x66, 0xcd, 0xaa, 0x00
        };

        Names[key: "mediumblue"] = new[]
        {
            0x00, 0x00, 0xcd, 0x00
        };

        Names[key: "mediumorchid"] = new[]
        {
            0xba, 0x55, 0xd3, 0x00
        };

        Names[key: "mediumpurple"] = new[]
        {
            0x93, 0x70, 0xdb, 0x00
        };

        Names[key: "mediumseagreen"] = new[]
        {
            0x3c, 0xb3, 0x71, 0x00
        };

        Names[key: "mediumslateblue"] = new[]
        {
            0x7b, 0x68, 0xee, 0x00
        };

        Names[key: "mediumspringgreen"] = new[]
        {
            0x00, 0xfa, 0x9a, 0x00
        };

        Names[key: "mediumturquoise"] = new[]
        {
            0x48, 0xd1, 0xcc, 0x00
        };

        Names[key: "mediumvioletred"] = new[]
        {
            0xc7, 0x15, 0x85, 0x00
        };

        Names[key: "midnightblue"] = new[]
        {
            0x19, 0x19, 0x70, 0x00
        };

        Names[key: "mintcream"] = new[]
        {
            0xf5, 0xff, 0xfa, 0x00
        };

        Names[key: "mistyrose"] = new[]
        {
            0xff, 0xe4, 0xe1, 0x00
        };

        Names[key: "moccasin"] = new[]
        {
            0xff, 0xe4, 0xb5, 0x00
        };

        Names[key: "navajowhite"] = new[]
        {
            0xff, 0xde, 0xad, 0x00
        };

        Names[key: "navy"] = new[]
        {
            0x00, 0x00, 0x80, 0x00
        };

        Names[key: "oldlace"] = new[]
        {
            0xfd, 0xf5, 0xe6, 0x00
        };

        Names[key: "olive"] = new[]
        {
            0x80, 0x80, 0x00, 0x00
        };

        Names[key: "olivedrab"] = new[]
        {
            0x6b, 0x8e, 0x23, 0x00
        };

        Names[key: "orange"] = new[]
        {
            0xff, 0xa5, 0x00, 0x00
        };

        Names[key: "orangered"] = new[]
        {
            0xff, 0x45, 0x00, 0x00
        };

        Names[key: "orchid"] = new[]
        {
            0xda, 0x70, 0xd6, 0x00
        };

        Names[key: "palegoldenrod"] = new[]
        {
            0xee, 0xe8, 0xaa, 0x00
        };

        Names[key: "palegreen"] = new[]
        {
            0x98, 0xfb, 0x98, 0x00
        };

        Names[key: "paleturquoise"] = new[]
        {
            0xaf, 0xee, 0xee, 0x00
        };

        Names[key: "palevioletred"] = new[]
        {
            0xdb, 0x70, 0x93, 0x00
        };

        Names[key: "papayawhip"] = new[]
        {
            0xff, 0xef, 0xd5, 0x00
        };

        Names[key: "peachpuff"] = new[]
        {
            0xff, 0xda, 0xb9, 0x00
        };

        Names[key: "peru"] = new[]
        {
            0xcd, 0x85, 0x3f, 0x00
        };

        Names[key: "pink"] = new[]
        {
            0xff, 0xc0, 0xcb, 0x00
        };

        Names[key: "plum"] = new[]
        {
            0xdd, 0xa0, 0xdd, 0x00
        };

        Names[key: "powderblue"] = new[]
        {
            0xb0, 0xe0, 0xe6, 0x00
        };

        Names[key: "purple"] = new[]
        {
            0x80, 0x00, 0x80, 0x00
        };

        Names[key: "red"] = new[]
        {
            0xff, 0x00, 0x00, 0x00
        };

        Names[key: "rosybrown"] = new[]
        {
            0xbc, 0x8f, 0x8f, 0x00
        };

        Names[key: "royalblue"] = new[]
        {
            0x41, 0x69, 0xe1, 0x00
        };

        Names[key: "saddlebrown"] = new[]
        {
            0x8b, 0x45, 0x13, 0x00
        };

        Names[key: "salmon"] = new[]
        {
            0xfa, 0x80, 0x72, 0x00
        };

        Names[key: "sandybrown"] = new[]
        {
            0xf4, 0xa4, 0x60, 0x00
        };

        Names[key: "seagreen"] = new[]
        {
            0x2e, 0x8b, 0x57, 0x00
        };

        Names[key: "seashell"] = new[]
        {
            0xff, 0xf5, 0xee, 0x00
        };

        Names[key: "sienna"] = new[]
        {
            0xa0, 0x52, 0x2d, 0x00
        };

        Names[key: "silver"] = new[]
        {
            0xc0, 0xc0, 0xc0, 0x00
        };

        Names[key: "skyblue"] = new[]
        {
            0x87, 0xce, 0xeb, 0x00
        };

        Names[key: "slateblue"] = new[]
        {
            0x6a, 0x5a, 0xcd, 0x00
        };

        Names[key: "slategray"] = new[]
        {
            0x70, 0x80, 0x90, 0x00
        };

        Names[key: "snow"] = new[]
        {
            0xff, 0xfa, 0xfa, 0x00
        };

        Names[key: "springgreen"] = new[]
        {
            0x00, 0xff, 0x7f, 0x00
        };

        Names[key: "steelblue"] = new[]
        {
            0x46, 0x82, 0xb4, 0x00
        };

        Names[key: "tan"] = new[]
        {
            0xd2, 0xb4, 0x8c, 0x00
        };

        Names[key: "transparent"] = new[]
        {
            0x00, 0x00, 0x00, 0xff
        };

        Names[key: "teal"] = new[]
        {
            0x00, 0x80, 0x80, 0x00
        };

        Names[key: "thistle"] = new[]
        {
            0xd8, 0xbf, 0xd8, 0x00
        };

        Names[key: "tomato"] = new[]
        {
            0xff, 0x63, 0x47, 0x00
        };

        Names[key: "turquoise"] = new[]
        {
            0x40, 0xe0, 0xd0, 0x00
        };

        Names[key: "violet"] = new[]
        {
            0xee, 0x82, 0xee, 0x00
        };

        Names[key: "wheat"] = new[]
        {
            0xf5, 0xde, 0xb3, 0x00
        };

        Names[key: "white"] = new[]
        {
            0xff, 0xff, 0xff, 0x00
        };

        Names[key: "whitesmoke"] = new[]
        {
            0xf5, 0xf5, 0xf5, 0x00
        };

        Names[key: "yellow"] = new[]
        {
            0xff, 0xff, 0x00, 0x00
        };

        Names[key: "yellowgreen"] = new[]
        {
            0x9, 0xacd, 0x32, 0x00
        };
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

        int[] c =
        {
            0, 0, 0, 0
        };

        if (name.StartsWith(value: '#'))
        {
            if (name.Length == 4)
            {
                c[0] = int.Parse(name.Substring(startIndex: 1, length: 1), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture) * 16;

                c[1] = int.Parse(name.Substring(startIndex: 2, length: 1), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture) * 16;

                c[2] = int.Parse(name.Substring(startIndex: 3), NumberStyles.HexNumber, CultureInfo.InvariantCulture) *
                       16;

                return new BaseColor(c[0], c[1], c[2], c[3]);
            }

            if (name.Length == 7)
            {
                c[0] = int.Parse(name.Substring(startIndex: 1, length: 2), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);

                c[1] = int.Parse(name.Substring(startIndex: 3, length: 2), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);

                c[2] = int.Parse(name.Substring(startIndex: 5), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                return new BaseColor(c[0], c[1], c[2], c[3]);
            }

            throw new ArgumentException(message: "Unknown color format. Must be #RGB or #RRGGBB");
        }

        if (name.StartsWith(value: "rgb(", StringComparison.OrdinalIgnoreCase))
        {
            var tok = new StringTokenizer(name, delim: "rgb(), \t\r\n\f");

            for (var k = 0; k < 3; ++k)
            {
                var v = tok.NextToken();

                if (v.EndsWith(value: '%'))
                {
                    c[k] = int.Parse(v.Substring(startIndex: 0, v.Length - 1), CultureInfo.InvariantCulture) * 255 /
                           100;
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