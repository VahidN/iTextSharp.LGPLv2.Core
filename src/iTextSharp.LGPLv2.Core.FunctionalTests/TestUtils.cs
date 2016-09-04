using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using iTextSharp.text;
using iTextSharp.text.exceptions;
using iTextSharp.text.pdf;

namespace iTextSharp.LGPLv2.Core.FunctionalTests
{
    public static class TestUtils
    {
        public static string Author => "VahidN";

        public static string GetBaseDir()
        {
            var currentAssembly = typeof(TestUtils).GetTypeInfo().Assembly;
            var root = Path.GetDirectoryName(currentAssembly.Location);
            var idx = root.IndexOf(@"\bin", StringComparison.OrdinalIgnoreCase);
            return root.Substring(0, idx);
        }

        public static string GetOutputFolder()
        {
            var dir = Path.Combine(GetBaseDir(), "bin", "out");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetOutputFileName([CallerMemberName] string methodName = null)
        {
            return Path.Combine(GetOutputFolder(), $"{methodName}.pdf");
        }

        public static string GetTahomaFontPath()
        {
            const string fontsfolder = @"c:\windows\fonts";
            return Path.Combine(fontsfolder, "tahoma.ttf");
        }

        public static Font GetUnicodeFont(
                    string fontName, string fontFilePath, float size, int style, BaseColor color)
        {
            if (!FontFactory.IsRegistered(fontName))
            {
                FontFactory.Register(fontFilePath);
            }
            return FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED, size, style, color);
        }

        public static void VerifyPdfFileIsReadable(byte[] file)
        {
            PdfReader reader = null;
            try
            {
                reader = new PdfReader(file);
                var author = reader.Info["Author"] as string;
                if (string.IsNullOrWhiteSpace(author) || !author.Equals(Author))
                {
                    throw new InvalidPdfException("This is not a valid PDF file.");
                }
            }
            finally
            {
                reader?.Close();
            }
        }

        public static void VerifyPdfFileIsReadable(string filePath)
        {
            VerifyPdfFileIsReadable(File.ReadAllBytes(filePath));
        }
    }
}