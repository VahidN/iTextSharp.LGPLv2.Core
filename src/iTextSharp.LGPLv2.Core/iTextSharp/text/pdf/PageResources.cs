using System.util;

namespace iTextSharp.text.pdf;

public class PageResources
{
    protected PdfDictionary ColorDictionary = new();
    protected PdfDictionary ExtGStateDictionary = new();
    protected PdfDictionary FontDictionary = new();
    protected INullValueDictionary<PdfName, object> ForbiddenNames;
    protected int[] NamePtr = { 0 };
    protected PdfDictionary OriginalResources;
    protected PdfDictionary PatternDictionary = new();
    protected PdfDictionary PropertyDictionary = new();
    protected PdfDictionary ShadingDictionary = new();
    protected INullValueDictionary<PdfName, PdfName> UsedNames;
    protected PdfDictionary XObjectDictionary = new();

    internal PageResources()
    {
    }

    internal PdfDictionary Resources
    {
        get
        {
            var resources = new PdfResources();
            if (OriginalResources != null)
            {
                resources.Merge(OriginalResources);
            }

            resources.Put(PdfName.Procset, new PdfLiteral("[/PDF /Text /ImageB /ImageC /ImageI]"));
            resources.Add(PdfName.Font, FontDictionary);
            resources.Add(PdfName.Xobject, XObjectDictionary);
            resources.Add(PdfName.Colorspace, ColorDictionary);
            resources.Add(PdfName.Pattern, PatternDictionary);
            resources.Add(PdfName.Shading, ShadingDictionary);
            resources.Add(PdfName.Extgstate, ExtGStateDictionary);
            resources.Add(PdfName.Properties, PropertyDictionary);
            return resources;
        }
    }

    internal PdfName AddColor(PdfName name, PdfIndirectReference reference)
    {
        name = TranslateName(name);
        ColorDictionary.Put(name, reference);
        return name;
    }

    internal void AddDefaultColor(PdfName name, PdfObject obj)
    {
        if (obj == null || obj.IsNull())
        {
            ColorDictionary.Remove(name);
        }
        else
        {
            ColorDictionary.Put(name, obj);
        }
    }

    internal void AddDefaultColor(PdfDictionary dic)
    {
        ColorDictionary.Merge(dic);
    }

    internal void AddDefaultColorDiff(PdfDictionary dic)
    {
        ColorDictionary.MergeDifferent(dic);
    }

    internal PdfName AddExtGState(PdfName name, PdfIndirectReference reference)
    {
        name = TranslateName(name);
        ExtGStateDictionary.Put(name, reference);
        return name;
    }

    internal PdfName AddFont(PdfName name, PdfIndirectReference reference)
    {
        name = TranslateName(name);
        FontDictionary.Put(name, reference);
        return name;
    }

    internal PdfName AddPattern(PdfName name, PdfIndirectReference reference)
    {
        name = TranslateName(name);
        PatternDictionary.Put(name, reference);
        return name;
    }

    internal PdfName AddProperty(PdfName name, PdfIndirectReference reference)
    {
        name = TranslateName(name);
        PropertyDictionary.Put(name, reference);
        return name;
    }

    internal PdfName AddShading(PdfName name, PdfIndirectReference reference)
    {
        name = TranslateName(name);
        ShadingDictionary.Put(name, reference);
        return name;
    }

    internal PdfName AddXObject(PdfName name, PdfIndirectReference reference)
    {
        name = TranslateName(name);
        XObjectDictionary.Put(name, reference);
        return name;
    }

    internal bool HasResources() =>
        FontDictionary.Size > 0
        || XObjectDictionary.Size > 0
        || ColorDictionary.Size > 0
        || PatternDictionary.Size > 0
        || ShadingDictionary.Size > 0
        || ExtGStateDictionary.Size > 0
        || PropertyDictionary.Size > 0;

    internal void SetOriginalResources(PdfDictionary resources, int[] newNamePtr)
    {
        if (newNamePtr != null)
        {
            NamePtr = newNamePtr;
        }

        ForbiddenNames = new NullValueDictionary<PdfName, object>();
        UsedNames = new NullValueDictionary<PdfName, PdfName>();
        if (resources == null)
        {
            return;
        }

        OriginalResources = new PdfDictionary();
        OriginalResources.Merge(resources);
        foreach (var key in resources.Keys)
        {
            var sub = PdfReader.GetPdfObject(resources.Get(key));
            if (sub != null && sub.IsDictionary())
            {
                var dic = (PdfDictionary)sub;
                foreach (var name in dic.Keys)
                {
                    ForbiddenNames[name] = null;
                }

                var dic2 = new PdfDictionary();
                dic2.Merge(dic);
                OriginalResources.Put(key, dic2);
            }
        }
    }

    internal PdfName TranslateName(PdfName name)
    {
        var translated = name;
        if (ForbiddenNames != null)
        {
            translated = UsedNames[name];
            if (translated == null)
            {
                while (true)
                {
                    translated = new PdfName("Xi" + NamePtr[0]++);
                    if (!ForbiddenNames.ContainsKey(translated))
                    {
                        break;
                    }
                }

                UsedNames[name] = translated;
            }
        }

        return translated;
    }
}