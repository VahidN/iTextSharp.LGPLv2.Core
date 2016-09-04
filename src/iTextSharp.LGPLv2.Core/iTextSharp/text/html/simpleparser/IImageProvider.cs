using System.Collections;

namespace iTextSharp.text.html.simpleparser
{
    public interface IImageProvider
    {
        Image GetImage(string src, Hashtable h, ChainedProperties cprops, IDocListener doc);
    }
}