using System.Text;
using System.util;

namespace iTextSharp.text.xml.xmp;

/// <summary>
///     Abstract superclass of the XmpSchemas supported by iText.
/// </summary>
public abstract class XmpSchema : Properties
{
    /// <summary>
    ///     the namesspace
    /// </summary>
    protected string xmlns;

    /// <summary>
    ///     Constructs an XMP schema.
    /// </summary>
    /// <param name="xmlns"></param>
    protected XmpSchema(string xmlns) => this.xmlns = xmlns;

    /// <summary>
    /// </summary>
    /// <returns>Returns the xmlns.</returns>
    public string Xmlns => xmlns;

    public override string this[string key]
    {
        set => base[key] = Escape(value);
    }

    /// <summary>
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string Escape(string content)
    {
        if (content == null)
        {
            throw new ArgumentNullException(nameof(content));
        }

        var buf = new StringBuilder();
        for (var i = 0; i < content.Length; i++)
        {
            switch (content[i])
            {
                case '<':
                    buf.Append("&lt;");
                    break;
                case '>':
                    buf.Append("&gt;");
                    break;
                case '\'':
                    buf.Append("&apos;");
                    break;
                case '\"':
                    buf.Append("&quot;");
                    break;
                case '&':
                    buf.Append("&amp;");
                    break;
                default:
                    buf.Append(content[i]);
                    break;
            }
        }

        return buf.ToString();
    }

    /// <summary>
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>the previous property (null if there wasn't one)</returns>
    public void AddProperty(string key, string value)
    {
        this[key] = value;
    }

    public void SetProperty(string key, XmpArray value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        base[key] = value.ToString();
    }

    /// <summary>
    ///     @see java.util.Properties#setProperty(java.lang.String, java.lang.String)
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>the previous property (null if there wasn't one)</returns>
    public void SetProperty(string key, LangAlt value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        base[key] = value.ToString();
    }

    /// <summary>
    ///     The String representation of the contents.
    /// </summary>
    /// <returns>a String representation.</returns>
    public override string ToString()
    {
        var buf = new StringBuilder();
        foreach (var key in Keys)
        {
            Process(buf, key);
        }

        return buf.ToString();
    }

    /// <summary>
    ///     Processes a property
    /// </summary>
    /// <param name="buf"></param>
    /// <param name="p"></param>
    protected void Process(StringBuilder buf, object p)
    {
        if (buf == null)
        {
            throw new ArgumentNullException(nameof(buf));
        }

        if (p == null)
        {
            throw new ArgumentNullException(nameof(p));
        }

        buf.Append('<');
        buf.Append(p);
        buf.Append('>');
        buf.Append(this[p.ToString()]);
        buf.Append("</");
        buf.Append(p);
        buf.Append('>');
    }
}