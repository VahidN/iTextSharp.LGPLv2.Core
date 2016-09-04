using System.Collections;

namespace iTextSharp.text.pdf
{
    public class PageResources
    {

        protected PdfDictionary ColorDictionary = new PdfDictionary();
        protected PdfDictionary ExtGStateDictionary = new PdfDictionary();
        protected PdfDictionary FontDictionary = new PdfDictionary();
        protected Hashtable ForbiddenNames;
        protected int[] NamePtr = { 0 };
        protected PdfDictionary OriginalResources;
        protected PdfDictionary PatternDictionary = new PdfDictionary();
        protected PdfDictionary PropertyDictionary = new PdfDictionary();
        protected PdfDictionary ShadingDictionary = new PdfDictionary();
        protected Hashtable UsedNames;
        protected PdfDictionary XObjectDictionary = new PdfDictionary();

        internal PageResources()
        {
        }

        internal PdfDictionary Resources
        {
            get
            {
                PdfResources resources = new PdfResources();
                if (OriginalResources != null)
                    resources.Merge(OriginalResources);
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
                ColorDictionary.Remove(name);
            else
                ColorDictionary.Put(name, obj);
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

        internal bool HasResources()
        {
            return (FontDictionary.Size > 0
                || XObjectDictionary.Size > 0
                || ColorDictionary.Size > 0
                || PatternDictionary.Size > 0
                || ShadingDictionary.Size > 0
                || ExtGStateDictionary.Size > 0
                || PropertyDictionary.Size > 0);
        }

        internal void SetOriginalResources(PdfDictionary resources, int[] newNamePtr)
        {
            if (newNamePtr != null)
                NamePtr = newNamePtr;
            ForbiddenNames = new Hashtable();
            UsedNames = new Hashtable();
            if (resources == null)
                return;
            OriginalResources = new PdfDictionary();
            OriginalResources.Merge(resources);
            foreach (PdfName key in resources.Keys)
            {
                PdfObject sub = PdfReader.GetPdfObject(resources.Get(key));
                if (sub != null && sub.IsDictionary())
                {
                    PdfDictionary dic = (PdfDictionary)sub;
                    foreach (PdfName name in dic.Keys)
                    {
                        ForbiddenNames[name] = null;
                    }
                    PdfDictionary dic2 = new PdfDictionary();
                    dic2.Merge(dic);
                    OriginalResources.Put(key, dic2);
                }
            }
        }

        internal PdfName TranslateName(PdfName name)
        {
            PdfName translated = name;
            if (ForbiddenNames != null)
            {
                translated = (PdfName)UsedNames[name];
                if (translated == null)
                {
                    while (true)
                    {
                        translated = new PdfName("Xi" + (NamePtr[0]++));
                        if (!ForbiddenNames.ContainsKey(translated))
                            break;
                    }
                    UsedNames[name] = translated;
                }
            }
            return translated;
        }
    }
}
