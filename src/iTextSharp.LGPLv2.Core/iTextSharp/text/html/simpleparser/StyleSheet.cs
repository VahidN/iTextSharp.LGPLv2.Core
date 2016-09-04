using System;
using System.Collections;

namespace iTextSharp.text.html.simpleparser
{
    public class StyleSheet
    {
        public Hashtable ClassMap = new Hashtable();
        public Hashtable TagMap = new Hashtable();

        public void ApplyStyle(string tag, Hashtable props)
        {
            Hashtable map = (Hashtable)TagMap[tag.ToLower(System.Globalization.CultureInfo.InvariantCulture)];
            Hashtable temp;
            if (map != null)
            {
                temp = new Hashtable(map);
                foreach (DictionaryEntry dc in props)
                    temp[dc.Key] = dc.Value;
                foreach (DictionaryEntry dc in temp)
                    props[dc.Key] = dc.Value;
            }
            string cm = (string)props[Markup.HTML_ATTR_CSS_CLASS];
            if (cm == null)
                return;
            map = (Hashtable)ClassMap[cm.ToLower(System.Globalization.CultureInfo.InvariantCulture)];
            if (map == null)
                return;
            props.Remove(Markup.HTML_ATTR_CSS_CLASS);
            temp = new Hashtable(map);
            foreach (DictionaryEntry dc in props)
                temp[dc.Key] = dc.Value;
            foreach (DictionaryEntry dc in temp)
                props[dc.Key] = dc.Value;
        }

        public void LoadStyle(string style, Hashtable props)
        {
            ClassMap[style.ToLower(System.Globalization.CultureInfo.InvariantCulture)] = props;
        }

        public void LoadStyle(string style, string key, string value)
        {
            style = style.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            Hashtable props = (Hashtable)ClassMap[style];
            if (props == null)
            {
                props = new Hashtable();
                ClassMap[style] = props;
            }
            props[key] = value;
        }

        public void LoadTagStyle(string tag, Hashtable props)
        {
            TagMap[tag.ToLower(System.Globalization.CultureInfo.InvariantCulture)] = props;
        }

        public void LoadTagStyle(string tag, string key, string value)
        {
            tag = tag.ToLower(System.Globalization.CultureInfo.InvariantCulture);
            Hashtable props = (Hashtable)TagMap[tag];
            if (props == null)
            {
                props = new Hashtable();
                TagMap[tag] = props;
            }
            props[key] = value;
        }

        private void applyMap(Hashtable map, Hashtable props)
        {

        }
    }
}