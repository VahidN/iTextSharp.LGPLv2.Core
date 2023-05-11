using System.util;

namespace iTextSharp.text.xml;

/// <summary>
///     This interface is implemented by the peer of all the iText objects.
/// </summary>
public class XmlPeer
{
    /// <summary> This is the Map that contains the aliases of the attributes. </summary>
    protected Properties AttributeAliases = new();

    /// <summary> This is the Map that contains the default values of the attributes. </summary>
    protected Properties AttributeValues = new();

    /// <summary> This is the name of the alias. </summary>
    protected string CustomTagname;

    /// <summary> This is String that contains the default content of the attributes. </summary>
    protected string DefaultContent;

    /// <summary> This is the name of the alias. </summary>
    protected string Tagname;

    /// <summary>
    ///     Creates a XmlPeer.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="alias"></param>
    public XmlPeer(string name, string alias)
    {
        Tagname = name;
        CustomTagname = alias;
    }

    /// <summary>
    ///     Gets the alias of the peer.
    /// </summary>
    /// <value>the alias of the peer</value>
    public string Alias => CustomTagname;

    /// <summary>
    ///     Sets the default content.
    /// </summary>
    /// <value>the default content</value>
    public string Content
    {
        set => DefaultContent = value;
    }

    /// <summary>
    ///     Returns the default values.
    /// </summary>
    /// <value>the default values</value>
    public Properties DefaultValues => AttributeValues;

    /// <summary>
    ///     Gets the tagname of the peer.
    /// </summary>
    /// <value>the tagname of the peer</value>
    public string Tag => Tagname;

    /// <summary>
    ///     Sets an alias for an attribute.
    /// </summary>
    /// <param name="name">the iText tagname</param>
    /// <param name="attributeAlias">the custom tagname</param>
    public virtual void AddAlias(string name, string attributeAlias)
    {
        if (attributeAlias == null)
        {
            throw new ArgumentNullException(nameof(attributeAlias));
        }

        AttributeAliases.Add(attributeAlias, name);
    }

    /// <summary>
    ///     Sets a value for an attribute.
    /// </summary>
    /// <param name="name">the iText tagname</param>
    /// <param name="value">the default value for this tag</param>
    public void AddValue(string name, string value)
    {
        AttributeValues.Add(name, value);
    }

    /// <summary> Gets the list of attributes of the peer. </summary>
    public virtual Properties GetAttributes(INullValueDictionary<string, string> attrs)
    {
        var attributes = new Properties();
        attributes.AddAll(AttributeValues);
        if (DefaultContent != null)
        {
            attributes.Add(ElementTags.ITEXT, DefaultContent);
        }

        if (attrs != null)
        {
            foreach (var key in attrs.Keys)
            {
                attributes.Add(GetName(key), attrs[key]);
            }
        }

        return attributes;
    }

    /// <summary>
    ///     Returns the iText attribute name.
    /// </summary>
    /// <param name="name">the custom attribute name</param>
    /// <returns>the iText attribute name</returns>
    public string GetName(string name)
    {
        string value;
        if ((value = AttributeAliases[name]) != null)
        {
            return value;
        }

        return name;
    }
}