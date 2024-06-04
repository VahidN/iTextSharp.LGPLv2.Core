using System.util;
using iTextSharp.text.factories;

namespace iTextSharp.text.pdf;

/// <summary>
///     Page labels are used to identify each
///     page visually on the screen or in print.
///     @author  Paulo Soares (psoares@consiste.pt)
/// </summary>
public class PdfPageLabels
{
    /// <summary>
    ///     Logical pages will have the form 1,2,3,...
    /// </summary>
    public const int DECIMAL_ARABIC_NUMERALS = 0;

    /// <summary>
    ///     No logical page numbers are generated but fixed text may
    ///     still exist
    /// </summary>
    public const int EMPTY = 5;

    /// <summary>
    ///     Logical pages will have the form of uppercase letters
    ///     (a to z for the first 26 pages, aa to zz for the next 26, and so on)
    /// </summary>
    public const int LOWERCASE_LETTERS = 4;

    /// <summary>
    ///     Logical pages will have the form i,ii,iii,iv,...
    /// </summary>
    public const int LOWERCASE_ROMAN_NUMERALS = 2;

    /// <summary>
    ///     Logical pages will have the form of uppercase letters
    ///     (A to Z for the first 26 pages, AA to ZZ for the next 26, and so on)
    /// </summary>
    public const int UPPERCASE_LETTERS = 3;

    /// <summary>
    ///     Logical pages will have the form I,II,III,IV,...
    /// </summary>
    public const int UPPERCASE_ROMAN_NUMERALS = 1;

    /// <summary>
    ///     Dictionary values to set the logical page styles
    /// </summary>
    internal static readonly PdfName[] NumberingStyle =
    {
        PdfName.D, PdfName.R,
        new("r"), PdfName.A, new("a"),
    };

    /// <summary>
    ///     The sequence of logical pages. Will contain at least a value for page 1
    /// </summary>
    internal readonly INullValueDictionary<int, PdfDictionary> Map;

    /// <summary>
    ///     Creates a new PdfPageLabel with a default logical page 1
    /// </summary>
    public PdfPageLabels()
    {
        Map = new NullValueDictionary<int, PdfDictionary>();
        AddPageLabel(1, DECIMAL_ARABIC_NUMERALS, null, 1);
    }

    /// <summary>
    ///     Retrieves the page labels from a PDF as an array of {@link PdfPageLabelFormat} objects.
    ///     or  null  if no page labels are present
    /// </summary>
    /// <param name="reader">a PdfReader object that has the page labels you want to retrieve</param>
    /// <returns>a PdfPageLabelEntry array, containing an entry for each format change</returns>
    public static PdfPageLabelFormat[] GetPageLabelFormats(PdfReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var dict = reader.Catalog;
        var labels = (PdfDictionary)PdfReader.GetPdfObjectRelease(dict.Get(PdfName.Pagelabels));
        if (labels == null)
        {
            return null;
        }

        var numberTree = PdfNumberTree.ReadTree(labels);
        var numbers = new int[numberTree.Count];
        numberTree.Keys.CopyTo(numbers, 0);
        Array.Sort(numbers);
        var formats = new PdfPageLabelFormat[numberTree.Count];
        string prefix;
        int numberStyle;
        int pagecount;
        for (var k = 0; k < numbers.Length; ++k)
        {
            var key = numbers[k];
            var d = (PdfDictionary)PdfReader.GetPdfObjectRelease(numberTree[key]);
            if (d.Contains(PdfName.St))
            {
                pagecount = ((PdfNumber)d.Get(PdfName.St)).IntValue;
            }
            else
            {
                pagecount = 1;
            }

            if (d.Contains(PdfName.P))
            {
                prefix = ((PdfString)d.Get(PdfName.P)).ToUnicodeString();
            }
            else
            {
                prefix = "";
            }

            if (d.Contains(PdfName.S))
            {
                var type = ((PdfName)d.Get(PdfName.S)).ToString()[1];
                switch (type)
                {
                    case 'R':
                        numberStyle = UPPERCASE_ROMAN_NUMERALS;
                        break;
                    case 'r':
                        numberStyle = LOWERCASE_ROMAN_NUMERALS;
                        break;
                    case 'A':
                        numberStyle = UPPERCASE_LETTERS;
                        break;
                    case 'a':
                        numberStyle = LOWERCASE_LETTERS;
                        break;
                    default:
                        numberStyle = DECIMAL_ARABIC_NUMERALS;
                        break;
                }
            }
            else
            {
                numberStyle = EMPTY;
            }

            formats[k] = new PdfPageLabelFormat(key + 1, numberStyle, prefix, pagecount);
        }

        return formats;
    }

    /// <summary>
    ///     Retrieves the page labels from a PDF as an array of String objects.
    /// </summary>
    /// <param name="reader">a PdfReader object that has the page labels you want to retrieve</param>
    /// <returns>a String array or  null  if no page labels are present</returns>
    public static string[] GetPageLabels(PdfReader reader)
    {
        if (reader == null)
        {
            throw new ArgumentNullException(nameof(reader));
        }

        var n = reader.NumberOfPages;

        var dict = reader.Catalog;
        var labels = (PdfDictionary)PdfReader.GetPdfObjectRelease(dict.Get(PdfName.Pagelabels));
        if (labels == null)
        {
            return null;
        }

        var labelstrings = new string[n];
        var numberTree = PdfNumberTree.ReadTree(labels);

        var pagecount = 1;
        var prefix = "";
        var type = 'D';
        for (var i = 0; i < n; i++)
        {
            if (numberTree.TryGetValue(i, out var value))
            {
                var d = (PdfDictionary)PdfReader.GetPdfObjectRelease(value);
                if (d.Contains(PdfName.St))
                {
                    pagecount = ((PdfNumber)d.Get(PdfName.St)).IntValue;
                }
                else
                {
                    pagecount = 1;
                }

                if (d.Contains(PdfName.P))
                {
                    prefix = ((PdfString)d.Get(PdfName.P)).ToUnicodeString();
                }

                if (d.Contains(PdfName.S))
                {
                    type = ((PdfName)d.Get(PdfName.S)).ToString()[1];
                }
            }

            switch (type)
            {
                default:
                    labelstrings[i] = prefix + pagecount;
                    break;
                case 'R':
                    labelstrings[i] = prefix + RomanNumberFactory.GetUpperCaseString(pagecount);
                    break;
                case 'r':
                    labelstrings[i] = prefix + RomanNumberFactory.GetLowerCaseString(pagecount);
                    break;
                case 'A':
                    labelstrings[i] = prefix + RomanAlphabetFactory.GetUpperCaseString(pagecount);
                    break;
                case 'a':
                    labelstrings[i] = prefix + RomanAlphabetFactory.GetLowerCaseString(pagecount);
                    break;
            }

            pagecount++;
        }

        return labelstrings;
    }

    /// <summary>
    ///     Adds or replaces a page label.
    /// </summary>
    /// <param name="page">the real page to start the numbering. First page is 1</param>
    /// <param name="numberStyle">the numbering style such as LOWERCASE_ROMAN_NUMERALS</param>
    /// <param name="text">the text to prefix the number. Can be  null  or empty</param>
    /// <param name="firstPage">the first logical page number</param>
    public void AddPageLabel(int page, int numberStyle, string text, int firstPage)
    {
        if (page < 1 || firstPage < 1)
        {
            throw new ArgumentException("In a page label the page numbers must be greater or equal to 1.");
        }

        var dic = new PdfDictionary();
        if (numberStyle >= 0 && numberStyle < NumberingStyle.Length)
        {
            dic.Put(PdfName.S, NumberingStyle[numberStyle]);
        }

        if (text != null)
        {
            dic.Put(PdfName.P, new PdfString(text, PdfObject.TEXT_UNICODE));
        }

        if (firstPage != 1)
        {
            dic.Put(PdfName.St, new PdfNumber(firstPage));
        }

        Map[page - 1] = dic;
    }

    /// <summary>
    ///     Adds or replaces a page label. The first logical page has the default
    ///     of 1.
    /// </summary>
    /// <param name="page">the real page to start the numbering. First page is 1</param>
    /// <param name="numberStyle">the numbering style such as LOWERCASE_ROMAN_NUMERALS</param>
    /// <param name="text">the text to prefix the number. Can be  null  or empty</param>
    public void AddPageLabel(int page, int numberStyle, string text)
    {
        AddPageLabel(page, numberStyle, text, 1);
    }

    /// <summary>
    ///     Adds or replaces a page label. There is no text prefix and the first
    ///     logical page has the default of 1.
    /// </summary>
    /// <param name="page">the real page to start the numbering. First page is 1</param>
    /// <param name="numberStyle">the numbering style such as LOWERCASE_ROMAN_NUMERALS</param>
    public void AddPageLabel(int page, int numberStyle)
    {
        AddPageLabel(page, numberStyle, null, 1);
    }

    /// <summary>
    ///     Adds or replaces a page label.
    /// </summary>
    public void AddPageLabel(PdfPageLabelFormat format)
    {
        if (format == null)
        {
            throw new ArgumentNullException(nameof(format));
        }

        AddPageLabel(format.PhysicalPage, format.NumberStyle, format.Prefix, format.LogicalPage);
    }

    /// <summary>
    ///     Removes a page label. The first page lagel can not be removed, only changed.
    /// </summary>
    /// <param name="page">the real page to remove</param>
    public void RemovePageLabel(int page)
    {
        if (page <= 1)
        {
            return;
        }

        Map.Remove(page - 1);
    }

    /// <summary>
    ///     Gets the page label dictionary to insert into the document.
    /// </summary>
    /// <returns>the page label dictionary</returns>
    internal PdfDictionary GetDictionary(PdfWriter writer) => PdfNumberTree.WriteTree(Map, writer);

    public class PdfPageLabelFormat
    {
        public int LogicalPage;
        public int NumberStyle;
        public int PhysicalPage;
        public string Prefix;

        /// <summary>
        ///     Creates a page label format.
        /// </summary>
        /// <param name="physicalPage">the real page to start the numbering. First page is 1</param>
        /// <param name="numberStyle">the numbering style such as LOWERCASE_ROMAN_NUMERALS</param>
        /// <param name="prefix">the text to prefix the number. Can be  null  or empty</param>
        /// <param name="logicalPage">the first logical page number</param>
        public PdfPageLabelFormat(int physicalPage, int numberStyle, string prefix, int logicalPage)
        {
            PhysicalPage = physicalPage;
            NumberStyle = numberStyle;
            Prefix = prefix;
            LogicalPage = logicalPage;
        }
    }
}