using System.Collections;

namespace iTextSharp.text.html.simpleparser
{

    public class ChainedProperties
    {

        public static int[] FontSizes = { 8, 10, 12, 14, 18, 24, 36 };
        public ArrayList Chain = new ArrayList();

        public string this[string key]
        {
            get
            {
                for (int k = Chain.Count - 1; k >= 0; --k)
                {
                    object[] obj = (object[])Chain[k];
                    Hashtable prop = (Hashtable)obj[1];
                    string ret = (string)prop[key];
                    if (ret != null)
                        return ret;
                }
                return null;
            }
        }

        public void AddToChain(string key, Hashtable prop)
        {
            // adjust the font size
            string value = (string)prop[ElementTags.SIZE];
            if (value != null)
            {
                if (value.EndsWith("pt"))
                {
                    prop[ElementTags.SIZE] = value.Substring(0, value.Length - 2);
                }
                else
                {
                    int s = 0;
                    if (value.StartsWith("+") || value.StartsWith("-"))
                    {
                        string old = this["basefontsize"];
                        if (old == null)
                            old = "12";
                        float f = float.Parse(old, System.Globalization.NumberFormatInfo.InvariantInfo);
                        int c = (int)f;
                        for (int k = FontSizes.Length - 1; k >= 0; --k)
                        {
                            if (c >= FontSizes[k])
                            {
                                s = k;
                                break;
                            }
                        }
                        int inc = int.Parse(value.StartsWith("+") ? value.Substring(1) : value, CultureInfo.InvariantCulture);
                        s += inc;
                    }
                    else
                    {
                        try
                        {
                            s = int.Parse(value, CultureInfo.InvariantCulture) - 1;
                        }
                        catch
                        {
                            s = 0;
                        }
                    }
                    if (s < 0)
                        s = 0;
                    else if (s >= FontSizes.Length)
                        s = FontSizes.Length - 1;
                    prop[ElementTags.SIZE] = FontSizes[s].ToString();
                }
            }
            Chain.Add(new object[] { key, prop });
        }

        public bool HasProperty(string key)
        {
            for (int k = Chain.Count - 1; k >= 0; --k)
            {
                object[] obj = (object[])Chain[k];
                Hashtable prop = (Hashtable)obj[1];
                if (prop.ContainsKey(key))
                    return true;
            }
            return false;
        }
        public void RemoveChain(string key)
        {
            for (int k = Chain.Count - 1; k >= 0; --k)
            {
                if (key.Equals(((object[])Chain[k])[0]))
                {
                    Chain.RemoveAt(k);
                    return;
                }
            }
        }
    }
}