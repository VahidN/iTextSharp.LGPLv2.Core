using System.Collections;

namespace iTextSharp.text.html.simpleparser
{
    public interface IImg
    {
        bool Process(Image img, Hashtable h, ChainedProperties cprops, IDocListener doc);
    }
}
