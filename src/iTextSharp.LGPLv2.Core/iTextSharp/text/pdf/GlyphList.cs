using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
/// </summary>
public static class GlyphList
{
    private static readonly INullValueDictionary<string, int[]> _names2Unicode =
        new NullValueDictionary<string, int[]>();

    private static readonly INullValueDictionary<int, string> _unicode2Names = new NullValueDictionary<int, string>();

    static GlyphList()
    {
        try
        {
            using var resourceStream = BaseFont.GetResourceStream($"{BaseFont.RESOURCE_PATH}glyphlist.txt");
            if (resourceStream == null)
            {
                Console.Error.WriteLine("glyphlist.txt not found as resource.");
                return;
            }

            var buf = new byte[1024];
            using var outputStream = new MemoryStream();
            while (true)
            {
                var size = resourceStream.Read(buf, 0, buf.Length);
                if (size == 0)
                {
                    break;
                }

                outputStream.Write(buf, 0, size);
            }

            var s = PdfEncodings.ConvertToString(outputStream.ToArray(), null);
            var tk = new StringTokenizer(s, "\r\n");
            while (tk.HasMoreTokens())
            {
                var line = tk.NextToken();
                if (line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                var t2 = new StringTokenizer(line, " ;\r\n\t\f");
                string name = null;
                string hex = null;
                if (!t2.HasMoreTokens())
                {
                    continue;
                }

                name = t2.NextToken();
                if (!t2.HasMoreTokens())
                {
                    continue;
                }

                hex = t2.NextToken();
                var num = int.Parse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                _unicode2Names[num] = name;
                _names2Unicode[name] = new[] { num };
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"glyphlist.txt loading error: {e.Message}");
        }
    }

    public static int[] NameToUnicode(string name) => _names2Unicode[name];

    public static string UnicodeToName(int num) => _unicode2Names[num];
}