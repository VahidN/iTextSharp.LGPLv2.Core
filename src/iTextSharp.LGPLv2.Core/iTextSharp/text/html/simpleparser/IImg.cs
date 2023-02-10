using System.util;

namespace iTextSharp.text.html.simpleparser;

public interface IImg
{
    bool Process(Image img, INullValueDictionary<string, string> h, ChainedProperties cprops, IDocListener doc);
}