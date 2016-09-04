using System;
using System.Collections;
using System.IO;
using System.Globalization;
using System.util;

namespace iTextSharp.text.pdf
{
    /// <summary>
    ///
    /// </summary>
    public class GlyphList
    {
        private static readonly Hashtable _names2Unicode = new Hashtable();
        private static readonly Hashtable _unicode2Names = new Hashtable();

        static GlyphList()
        {
            Stream istr = null;
            try
            {
                istr = BaseFont.GetResourceStream($"{BaseFont.RESOURCE_PATH}glyphlist.txt");
                if (istr == null)
                {
                    string msg = "glyphlist.txt not found as resource.";
                    throw new Exception(msg);
                }
                byte[] buf = new byte[1024];
                MemoryStream outp = new MemoryStream();
                while (true)
                {
                    int size = istr.Read(buf, 0, buf.Length);
                    if (size == 0)
                        break;
                    outp.Write(buf, 0, size);
                }
                istr.Dispose();
                istr = null;
                string s = PdfEncodings.ConvertToString(outp.ToArray(), null);
                StringTokenizer tk = new StringTokenizer(s, "\r\n");
                while (tk.HasMoreTokens())
                {
                    string line = tk.NextToken();
                    if (line.StartsWith("#"))
                        continue;
                    StringTokenizer t2 = new StringTokenizer(line, " ;\r\n\t\f");
                    string name = null;
                    string hex = null;
                    if (!t2.HasMoreTokens())
                        continue;
                    name = t2.NextToken();
                    if (!t2.HasMoreTokens())
                        continue;
                    hex = t2.NextToken();
                    int num = int.Parse(hex, NumberStyles.HexNumber);
                    _unicode2Names[num] = name;
                    _names2Unicode[name] = new[] { num };
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"glyphlist.txt loading error: {e.Message}");
            }
            finally
            {
                if (istr != null)
                {
                    try
                    {
                        istr.Dispose();
                    }
                    catch
                    {
                        // empty on purpose
                    }
                }
            }
        }

        public static int[] NameToUnicode(string name)
        {
            return (int[])_names2Unicode[name];
        }

        public static string UnicodeToName(int num)
        {
            return (string)_unicode2Names[num];
        }
    }
}