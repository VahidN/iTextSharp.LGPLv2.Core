using System.util;

namespace iTextSharp.text.html.simpleparser;

public class StyleSheet
{
    public INullValueDictionary<string, INullValueDictionary<string, string>> ClassMap { set; get; } =
        new NullValueDictionary<string, INullValueDictionary<string, string>>();

    public INullValueDictionary<string, INullValueDictionary<string, string>> TagMap { set; get; } =
        new NullValueDictionary<string, INullValueDictionary<string, string>>();

    public void ApplyStyle(string tag, INullValueDictionary<string, string> props)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        var map = TagMap[tag.ToLower(CultureInfo.InvariantCulture)];
        NullValueDictionary<string, string> temp;
        if (map != null)
        {
            temp = new NullValueDictionary<string, string>(map);
            foreach (var dc in props)
            {
                temp[dc.Key] = dc.Value;
            }

            foreach (var dc in temp)
            {
                props[dc.Key] = dc.Value;
            }
        }

        var cm = props[Markup.HTML_ATTR_CSS_CLASS];
        if (cm == null)
        {
            return;
        }

        map = ClassMap[cm.ToLower(CultureInfo.InvariantCulture)];
        if (map == null)
        {
            return;
        }

        props.Remove(Markup.HTML_ATTR_CSS_CLASS);
        temp = new NullValueDictionary<string, string>(map);
        foreach (var dc in props)
        {
            temp[dc.Key] = dc.Value;
        }

        foreach (var dc in temp)
        {
            props[dc.Key] = dc.Value;
        }
    }

    public void LoadStyle(string style, INullValueDictionary<string, string> props)
    {
        if (style == null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        ClassMap[style.ToLower(CultureInfo.InvariantCulture)] = props;
    }

    public void LoadStyle(string style, string key, string value)
    {
        if (style == null)
        {
            throw new ArgumentNullException(nameof(style));
        }

        style = style.ToLower(CultureInfo.InvariantCulture);
        var props = ClassMap[style];
        if (props == null)
        {
            props = new NullValueDictionary<string, string>();
            ClassMap[style] = props;
        }

        props[key] = value;
    }

    public void LoadTagStyle(string tag, INullValueDictionary<string, string> props)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        TagMap[tag.ToLower(CultureInfo.InvariantCulture)] = props;
    }

    public void LoadTagStyle(string tag, string key, string value)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        tag = tag.ToLower(CultureInfo.InvariantCulture);
        var props = TagMap[tag];
        if (props == null)
        {
            props = new NullValueDictionary<string, string>();
            TagMap[tag] = props;
        }

        props[key] = value;
    }
}