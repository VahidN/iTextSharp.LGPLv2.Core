using System.Text;
using System.util;

namespace iTextSharp.text.xml.xmp;

public class LangAlt : Properties
{
    /// <summary>
    ///     Key for the default language.
    /// </summary>
    public const string DEFAULT = "x-default";

    /// <summary>
    ///     Creates a Properties object that stores languages for use in an XmpSchema
    /// </summary>
    public LangAlt(string defaultValue)
    {
        AddLanguage(DEFAULT, defaultValue);
    }

    /// <summary>
    ///     Creates a Properties object that stores languages for use in an XmpSchema
    /// </summary>
    public LangAlt()
    {
    }

    /// <summary>
    ///     Add a language.
    /// </summary>
    public void AddLanguage(string language, string value)
    {
        this[language] = XmpSchema.Escape(value);
    }

    /// <summary>
    ///     Creates a String that can be used in an XmpSchema.
    /// </summary>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("<rdf:Alt>");
        foreach (var s in Keys)
        {
            Process(sb, s);
        }

        sb.Append("</rdf:Alt>");
        return sb.ToString();
    }

    /// <summary>
    ///     Process a property.
    /// </summary>
    protected internal void Process(StringBuilder buf, string lang)
    {
        if (buf == null)
        {
            throw new ArgumentNullException(nameof(buf));
        }

        buf.Append("<rdf:li xml:lang=\"");
        buf.Append(lang);
        buf.Append("\" >");
        buf.Append(this[lang]);
        buf.Append("</rdf:li>");
    }
}