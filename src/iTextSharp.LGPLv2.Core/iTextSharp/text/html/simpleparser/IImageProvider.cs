using System.util;

namespace iTextSharp.text.html.simpleparser;

public interface IImageProvider
{
    Image GetImage(string src, INullValueDictionary<string, string> h, ChainedProperties cprops, IDocListener doc);
}