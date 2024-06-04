namespace iTextSharp.text.pdf.intern;

public class PdfAnnotationsImp
{
    /// <summary>
    ///     This is the AcroForm object for the complete document.
    /// </summary>
    protected internal PdfAcroForm acroForm;

    /// <summary>
    ///     This is the array containing the references to annotations
    ///     that were added to the document.
    /// </summary>
    protected internal List<PdfAnnotation> Annotations;

    /// <summary>
    ///     This is an array containg references to some delayed annotations
    ///     (that were added for a page that doesn't exist yet).
    /// </summary>
    protected internal List<PdfAnnotation> DelayedAnnotations = new();


    public PdfAnnotationsImp(PdfWriter writer) => acroForm = new PdfAcroForm(writer);

    /// <summary>
    ///     Gets the AcroForm object.
    /// </summary>
    /// <returns>the PdfAcroform object of the PdfDocument</returns>
    public PdfAcroForm AcroForm => acroForm;

    public int SigFlags
    {
        set => acroForm.SigFlags = value;
    }

    public static PdfAnnotation ConvertAnnotation(PdfWriter writer, Annotation annot, Rectangle defaultRect)
    {
        if (annot == null)
        {
            throw new ArgumentNullException(nameof(annot));
        }

        if (defaultRect == null)
        {
            throw new ArgumentNullException(nameof(defaultRect));
        }

        switch (annot.AnnotationType)
        {
            case Annotation.URL_NET:
                return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(),
                                         new PdfAction((Uri)annot.Attributes[Annotation.URL]));
            case Annotation.URL_AS_STRING:
                return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(),
                                         new PdfAction((string)annot.Attributes[Annotation.FILE]));
            case Annotation.FILE_DEST:
                return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(),
                                         new PdfAction((string)annot.Attributes[Annotation.FILE],
                                                       (string)annot.Attributes[Annotation.DESTINATION]));
            case Annotation.SCREEN:
                var sparams = (bool[])annot.Attributes[Annotation.PARAMETERS];
                var fname = (string)annot.Attributes[Annotation.FILE];
                var mimetype = (string)annot.Attributes[Annotation.MIMETYPE];
                PdfFileSpecification fs;
                if (sparams[0])
                {
                    fs = PdfFileSpecification.FileEmbedded(writer, fname, fname, null);
                }
                else
                {
                    fs = PdfFileSpecification.FileExtern(writer, fname);
                }

                var ann = PdfAnnotation.CreateScreen(writer,
                                                     new Rectangle(annot.GetLlx(), annot.GetLly(), annot.GetUrx(),
                                                                   annot.GetUry()),
                                                     fname, fs, mimetype, sparams[1]);
                return ann;
            case Annotation.FILE_PAGE:
                return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(),
                                         new PdfAction((string)annot.Attributes[Annotation.FILE],
                                                       (int)annot.Attributes[Annotation.PAGE]));
            case Annotation.NAMED_DEST:
                return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(),
                                         new PdfAction((int)annot.Attributes[Annotation.NAMED]));
            case Annotation.LAUNCH:
                return new PdfAnnotation(writer, annot.GetLlx(), annot.GetLly(), annot.GetUrx(), annot.GetUry(),
                                         new PdfAction((string)annot.Attributes[Annotation.APPLICATION],
                                                       (string)annot.Attributes[Annotation.PARAMETERS],
                                                       (string)annot.Attributes[Annotation.OPERATION],
                                                       (string)annot.Attributes[Annotation.DEFAULTDIR]));
            default:
                return new PdfAnnotation(writer, defaultRect.Left, defaultRect.Bottom, defaultRect.Right,
                                         defaultRect.Top, new PdfString(annot.Title, PdfObject.TEXT_UNICODE),
                                         new PdfString(annot.Content, PdfObject.TEXT_UNICODE));
        }
    }

    public void AddAnnotation(PdfAnnotation annot)
    {
        if (annot == null)
        {
            throw new ArgumentNullException(nameof(annot));
        }

        if (annot.IsForm())
        {
            var field = (PdfFormField)annot;
            if (field.Parent == null)
            {
                addFormFieldRaw(field);
            }
        }
        else
        {
            Annotations.Add(annot);
        }
    }

    public void AddCalculationOrder(PdfFormField formField)
    {
        acroForm.AddCalculationOrder(formField);
    }

    public void AddPlainAnnotation(PdfAnnotation annot)
    {
        Annotations.Add(annot);
    }

    public bool HasUnusedAnnotations() => Annotations.Count > 0;

    /// <summary>
    ///     Checks if the AcroForm is valid.
    /// </summary>
    public bool HasValidAcroForm() => acroForm.IsValid();

    public void ResetAnnotations()
    {
        Annotations = DelayedAnnotations;
        DelayedAnnotations = new List<PdfAnnotation>();
    }

    public PdfArray RotateAnnotations(PdfWriter writer, Rectangle pageSize)
    {
        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        if (pageSize == null)
        {
            throw new ArgumentNullException(nameof(pageSize));
        }

        var array = new PdfArray();
        var rotation = pageSize.Rotation % 360;
        var currentPage = writer.CurrentPageNumber;
        for (var k = 0; k < Annotations.Count; ++k)
        {
            var dic = Annotations[k];
            var page = dic.PlaceInPage;
            if (page > currentPage)
            {
                DelayedAnnotations.Add(dic);
                continue;
            }

            if (dic.IsForm())
            {
                if (!dic.IsUsed())
                {
                    var templates = dic.Templates;
                    if (templates != null)
                    {
                        acroForm.AddFieldTemplates(templates);
                    }
                }

                var field = (PdfFormField)dic;
                if (field.Parent == null)
                {
                    acroForm.AddDocumentField(field.IndirectReference);
                }
            }

            if (dic.IsAnnotation())
            {
                array.Add(dic.IndirectReference);
                if (!dic.IsUsed())
                {
                    var rect = (PdfRectangle)dic.Get(PdfName.Rect);
                    if (rect != null)
                    {
                        switch (rotation)
                        {
                            case 90:
                                dic.Put(PdfName.Rect, new PdfRectangle(
                                                                       pageSize.Top - rect.Bottom,
                                                                       rect.Left,
                                                                       pageSize.Top - rect.Top,
                                                                       rect.Right));
                                break;
                            case 180:
                                dic.Put(PdfName.Rect, new PdfRectangle(
                                                                       pageSize.Right - rect.Left,
                                                                       pageSize.Top - rect.Bottom,
                                                                       pageSize.Right - rect.Right,
                                                                       pageSize.Top - rect.Top));
                                break;
                            case 270:
                                dic.Put(PdfName.Rect, new PdfRectangle(
                                                                       rect.Bottom,
                                                                       pageSize.Right - rect.Left,
                                                                       rect.Top,
                                                                       pageSize.Right - rect.Right));
                                break;
                        }
                    }
                }
            }

            if (!dic.IsUsed())
            {
                dic.SetUsed();
                writer.AddToBody(dic, dic.IndirectReference);
            }
        }

        return array;
    }

    private void addFormFieldRaw(PdfFormField field)
    {
        Annotations.Add(field);
        var kids = field.Kids;
        if (kids != null)
        {
            for (var k = 0; k < kids.Count; ++k)
            {
                addFormFieldRaw(kids[k]);
            }
        }
    }
}