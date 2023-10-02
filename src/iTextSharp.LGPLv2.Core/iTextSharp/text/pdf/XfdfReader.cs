using System.util;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf;

/// <summary>
///     Reads a XFDF.
///     @author Leonard Rosenthol (leonardr@pdfsages.com)
/// </summary>
public class XfdfReader : ISimpleXmlDocHandler
{
    private readonly Stackr _fieldNames = new();

    private readonly Stackr _fieldValues = new();

    /// <summary>
    ///     stuff used during parsing to handle state
    /// </summary>
    private bool _foundRoot;

    /// <summary>
    ///     storage for the field list and their values
    /// </summary>
    internal INullValueDictionary<string, string> fields;

    /// <summary>
    ///     storage for the path to referenced PDF, if any
    /// </summary>
    internal string fileSpec;

    /// <summary>
    ///     Storage for field values if there's more than one value for a field.
    ///     @since    2.1.4
    /// </summary>
    protected INullValueDictionary<string, List<string>> ListFields;

    /// <summary>
    ///     Reads an XFDF form.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="filename">the file name of the form</param>
    public XfdfReader(string filename)
    {
        FileStream fin = null;
        try
        {
            fin = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            SimpleXmlParser.Parse(this, fin);
        }
        finally
        {
            try
            {
                fin?.Dispose();
            }
            catch
            {
            }
        }
    }

    /// <summary>
    ///     Reads an XFDF form.
    ///     @throws IOException on error
    /// </summary>
    /// <param name="xfdfIn">the byte array with the form</param>
    public XfdfReader(byte[] xfdfIn)
    {
        using var memoryStream = new MemoryStream(xfdfIn);
        SimpleXmlParser.Parse(this, memoryStream);
    }

    /// <summary>
    ///     Gets all the fields. The map is keyed by the fully qualified
    ///     field name and the value is a merged  PdfDictionary
    ///     with the field content.
    /// </summary>
    /// <returns>all the fields</returns>
    public INullValueDictionary<string, string> Fields => fields;

    /// <summary>
    ///     Gets the PDF file specification contained in the FDF.
    /// </summary>
    /// <returns>the PDF file specification contained in the FDF</returns>
    public string FileSpec => fileSpec;

    /// <summary>
    ///     Called after the document is parsed.
    /// </summary>
    public void EndDocument()
    {
    }

    /// <summary>
    ///     Called when an end tag is found.
    /// </summary>
    /// <param name="tag">the tag name</param>
    public void EndElement(string tag)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (tag.Equals("value", StringComparison.Ordinal))
        {
            var fName = "";
            for (var k = 0; k < _fieldNames.Count; ++k)
            {
                fName += "." + _fieldNames[k];
            }

            if (fName.StartsWith(".", StringComparison.Ordinal))
            {
                fName = fName.Substring(1);
            }

            var fVal = _fieldValues.Pop();
            var old = fields[fName];
            fields[fName] = fVal;
            if (old != null)
            {
                var l = ListFields[fName];
                if (l == null)
                {
                    l = new List<string>();
                    l.Add(old);
                }

                l.Add(fVal);
                ListFields[fName] = l;
            }
        }
        else if (tag.Equals("field", StringComparison.Ordinal))
        {
            if (_fieldNames.Count != 0)
            {
                _fieldNames.Pop();
            }
        }
    }

    /// <summary>
    ///     Called when the document starts to be parsed.
    /// </summary>
    public void StartDocument()
    {
        fileSpec = ""; // and this too...
    }

    /// <summary>
    ///     Called when a start tag is found.
    /// </summary>
    /// <param name="tag">the tag name</param>
    /// <param name="h">the tag's attributes</param>
    public void StartElement(string tag, INullValueDictionary<string, string> h)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (h == null)
        {
            throw new ArgumentNullException(nameof(h));
        }

        if (!_foundRoot)
        {
            if (!tag.Equals("xfdf", StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Root element is not Bookmark.");
            }

            _foundRoot = true;
        }

        if (tag.Equals("xfdf", StringComparison.Ordinal))
        {
        }
        else if (tag.Equals("f", StringComparison.Ordinal))
        {
            fileSpec = h["href"];
        }
        else if (tag.Equals("fields", StringComparison.Ordinal))
        {
            fields = new NullValueDictionary<string, string>(); // init it!
            ListFields = new NullValueDictionary<string, List<string>>();
        }
        else if (tag.Equals("field", StringComparison.Ordinal))
        {
            var fName = h["name"];
            _fieldNames.Push(fName);
        }
        else if (tag.Equals("value", StringComparison.Ordinal))
        {
            _fieldValues.Push("");
        }
    }

    /// <summary>
    ///     Called when a text element is found.
    /// </summary>
    /// <param name="str">the text element, probably a fragment.</param>
    public void Text(string str)
    {
        if (_fieldNames.Count == 0 || _fieldValues.Count == 0)
        {
            return;
        }

        var val = _fieldValues.Pop();
        val += str;
        _fieldValues.Push(val);
    }

    /// <summary>
    ///     Gets the field value.
    /// </summary>
    /// <param name="name">the fully qualified field name</param>
    /// <returns>the field's value</returns>
    public string GetField(string name) => fields[name];

    /// <summary>
    ///     Gets the field value or  null  if the field does not
    ///     exist or has no value defined.
    /// </summary>
    /// <param name="name">the fully qualified field name</param>
    /// <returns>the field value or  null </returns>
    public string GetFieldValue(string name)
    {
        var field = fields[name];
        if (field == null)
        {
            return null;
        }

        return field;
    }

    /// <summary>
    ///     Gets the field values for a list or  null  if the field does not
    ///     exist or has no value defined.
    ///     @since   2.1.4
    /// </summary>
    /// <param name="name">the fully qualified field name</param>
    /// <returns>the field values or  null </returns>
    public List<string> GetListValues(string name) => ListFields[name];

    internal class Stackr : List<string>
    {
        internal string Pop()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The stack is empty.");
            }

            var obj = this[Count - 1];
            RemoveAt(Count - 1);
            return obj;
        }

        internal void Push(string obj)
        {
            Add(obj);
        }
    }
}