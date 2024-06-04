using System.util;

namespace iTextSharp.text;

/// <summary>
///     An Annotation is a little note that can be added to a page
///     on a document.
/// </summary>
public class Annotation : IElement
{
    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary>This is a possible attribute.</summary>
    public const string APPLICATION = "application";

    /// <summary>This is a possible attribute.</summary>
    public const string CONTENT = "content";

    /// <summary>This is a possible attribute.</summary>
    public const string DEFAULTDIR = "defaultdir";

    /// <summary>This is a possible attribute.</summary>
    public const string DESTINATION = "destination";

    /// <summary>This is a possible attribute.</summary>
    public const string FILE = "file";

    /// <summary>This is a possible annotation type.</summary>
    public const int FILE_DEST = 3;

    /// <summary>This is a possible annotation type.</summary>
    public const int FILE_PAGE = 4;

    /// <summary>This is a possible annotation type.</summary>
    public const int LAUNCH = 6;

    /// <summary>This is a possible attribute.</summary>
    public const string LLX = "llx";

    /// <summary>This is a possible attribute.</summary>
    public const string LLY = "lly";

    /// <summary>This is a possible attribute.</summary>
    public const string MIMETYPE = "mime";

    /// <summary>This is a possible attribute.</summary>
    public const string NAMED = "named";

    /// <summary>This is a possible annotation type.</summary>
    public const int NAMED_DEST = 5;

    /// <summary>This is a possible attribute.</summary>
    public const string OPERATION = "operation";

    /// <summary>This is a possible attribute.</summary>
    public const string PAGE = "page";

    /// <summary>This is a possible attribute.</summary>
    public const string PARAMETERS = "parameters";

    /// <summary>This is a possible annotation type.</summary>
    public const int SCREEN = 7;

    /// <summary>This is a possible annotation type.</summary>
    public const int TEXT = 0;

    /// <summary>This is a possible attribute.</summary>
    public const string TITLE = "title";

    /// <summary>This is a possible attribute.</summary>
    public const string URL = "url";

    /// <summary>This is a possible annotation type.</summary>
    public const int URL_AS_STRING = 2;

    /// <summary>This is a possible annotation type.</summary>
    public const int URL_NET = 1;

    /// <summary>This is a possible attribute.</summary>
    public const string URX = "urx";

    /// <summary>This is a possible attribute.</summary>
    public const string URY = "ury";

    /// <summary>This is the lower left x-value</summary>
    private float _llx = float.NaN;

    /// <summary>This is the lower left y-value</summary>
    private float _lly = float.NaN;

    /// <summary>This is the upper right x-value</summary>
    private float _urx = float.NaN;

    /// <summary>This is the upper right y-value</summary>
    private float _ury = float.NaN;

    /// <summary>This is the title of the Annotation.</summary>
    protected INullValueDictionary<string, object> AnnotationAttributes = new NullValueDictionary<string, object>();

    /// <summary>
    ///     constructors
    /// </summary>
    public Annotation(Annotation an)
    {
        if (an == null)
        {
            throw new ArgumentNullException(nameof(an));
        }

        AnnotationType = an.AnnotationType;
        AnnotationAttributes = an.AnnotationAttributes;
        _llx = an._llx;
        _lly = an._lly;
        _urx = an._urx;
        _ury = an._ury;
    }

    /// <summary>
    ///     Constructs an Annotation with a certain title and some text.
    /// </summary>
    /// <param name="title">the title of the annotation</param>
    /// <param name="text">the content of the annotation</param>
    public Annotation(string title, string text)
    {
        AnnotationType = TEXT;
        AnnotationAttributes[TITLE] = title;
        AnnotationAttributes[CONTENT] = text;
    }

    /// <summary>
    ///     Constructs an Annotation with a certain title and some text.
    /// </summary>
    /// <param name="title">the title of the annotation</param>
    /// <param name="text">the content of the annotation</param>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    public Annotation(string title, string text, float llx, float lly, float urx, float ury) : this(llx, lly, urx, ury)
    {
        AnnotationType = TEXT;
        AnnotationAttributes[TITLE] = title;
        AnnotationAttributes[CONTENT] = text;
    }

    /// <summary>
    ///     Constructs an Annotation.
    /// </summary>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    /// <param name="url">the external reference</param>
    public Annotation(float llx, float lly, float urx, float ury, Uri url) : this(llx, lly, urx, ury)
    {
        AnnotationType = URL_NET;
        AnnotationAttributes[URL] = url;
    }

    /// <summary>
    ///     Constructs an Annotation.
    /// </summary>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    /// <param name="url">the external reference</param>
    public Annotation(float llx, float lly, float urx, float ury, string url) : this(llx, lly, urx, ury)
    {
        AnnotationType = URL_AS_STRING;
        AnnotationAttributes[FILE] = url;
    }

    /// <summary>
    ///     Constructs an Annotation.
    /// </summary>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    /// <param name="file">an external PDF file</param>
    /// <param name="dest">the destination in this file</param>
    public Annotation(float llx, float lly, float urx, float ury, string file, string dest) : this(llx, lly, urx, ury)
    {
        AnnotationType = FILE_DEST;
        AnnotationAttributes[FILE] = file;
        AnnotationAttributes[DESTINATION] = dest;
    }

    /// <summary>
    ///     Creates a Screen anotation to embed media clips
    /// </summary>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    /// <param name="moviePath">path to the media clip file</param>
    /// <param name="mimeType">mime type of the media</param>
    /// <param name="showOnDisplay">if true play on display of the page</param>
    public Annotation(float llx, float lly, float urx, float ury,
                      string moviePath, string mimeType, bool showOnDisplay) : this(llx, lly, urx, ury)
    {
        AnnotationType = SCREEN;
        AnnotationAttributes[FILE] = moviePath;
        AnnotationAttributes[MIMETYPE] = mimeType;
        AnnotationAttributes[PARAMETERS] = new[] { false /* embedded */, showOnDisplay };
    }

    /// <summary>
    ///     Constructs an Annotation.
    /// </summary>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    /// <param name="file">an external PDF file</param>
    /// <param name="page">a page number in this file</param>
    public Annotation(float llx, float lly, float urx, float ury, string file, int page) : this(llx, lly, urx, ury)
    {
        AnnotationType = FILE_PAGE;
        AnnotationAttributes[FILE] = file;
        AnnotationAttributes[PAGE] = page;
    }

    /// <summary>
    ///     Constructs an Annotation.
    /// </summary>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    /// <param name="named">a named destination in this file</param>
    /// <overloads>
    ///     Has nine overloads.
    /// </overloads>
    public Annotation(float llx, float lly, float urx, float ury, int named) : this(llx, lly, urx, ury)
    {
        AnnotationType = NAMED_DEST;
        AnnotationAttributes[NAMED] = named;
    }

    /// <summary>
    ///     Constructs an Annotation.
    /// </summary>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    /// <param name="application">an external application</param>
    /// <param name="parameters">parameters to pass to this application</param>
    /// <param name="operation">the operation to pass to this application</param>
    /// <param name="defaultdir">the default directory to run this application in</param>
    public Annotation(float llx, float lly, float urx, float ury, string application, string parameters,
                      string operation, string defaultdir) : this(llx, lly, urx, ury)
    {
        AnnotationType = LAUNCH;
        AnnotationAttributes[APPLICATION] = application;
        AnnotationAttributes[PARAMETERS] = parameters;
        AnnotationAttributes[OPERATION] = operation;
        AnnotationAttributes[DEFAULTDIR] = defaultdir;
    }

    /// <summary>
    ///     Constructs an Annotation with a certain title and some text.
    /// </summary>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    private Annotation(float llx, float lly, float urx, float ury)
    {
        _llx = llx;
        _lly = lly;
        _urx = urx;
        _ury = ury;
    }

    /// <summary>
    ///     implementation of the Element-methods
    /// </summary>
    /// <summary>
    ///     Returns the type of this Annotation.
    /// </summary>
    /// <value>a type</value>
    public int AnnotationType { get; }

    /// <summary>
    ///     Gets the content of this Annotation.
    /// </summary>
    /// <value>a reference</value>
    public INullValueDictionary<string, object> Attributes => AnnotationAttributes;

    /// <summary>
    ///     Gets the content of this Annotation.
    /// </summary>
    /// <value>a reference</value>
    public string Content
    {
        get
        {
            var s = (string)AnnotationAttributes[CONTENT];
            if (s == null)
            {
                s = "";
            }

            return s;
        }
    }

    /// <summary>
    ///     Returns the title of this Annotation.
    /// </summary>
    /// <value>a name</value>
    public string Title
    {
        get
        {
            var s = (string)AnnotationAttributes[TITLE];
            if (s == null)
            {
                s = "";
            }

            return s;
        }
    }

    /// <summary>
    ///     Gets all the chunks in this element.
    /// </summary>
    /// <value>an ArrayList</value>
    public IList<Chunk> Chunks => new List<Chunk>();

    /// <summary>
    ///     Gets the type of the text element
    /// </summary>
    public int Type => Element.ANNOTATION;

    /// <summary>
    ///     @see com.lowagie.text.Element#isContent()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsContent() => true;

    /// <summary>
    ///     @see com.lowagie.text.Element#isNestable()
    ///     @since   iText 2.0.8
    /// </summary>
    public bool IsNestable() => true;

    /// <summary>
    ///     Processes the element by adding it (or the different parts) to an
    ///     IElementListener.
    /// </summary>
    /// <param name="listener">an IElementListener</param>
    /// <returns>true if the element was process successfully</returns>
    public bool Process(IElementListener listener)
    {
        if (listener == null)
        {
            throw new ArgumentNullException(nameof(listener));
        }

        try
        {
            return listener.Add(this);
        }
        catch (DocumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Returns the lower left x-value.
    /// </summary>
    /// <returns>a value</returns>
    public float GetLlx() => _llx;

    /// <summary>
    ///     Returns the lower left x-value.
    /// </summary>
    /// <param name="def">the default value</param>
    /// <returns>a value</returns>
    public float GetLlx(float def)
    {
        if (float.IsNaN(_llx))
        {
            return def;
        }

        return _llx;
    }

    /// <summary>
    ///     methods to retrieve information
    /// </summary>
    /// <summary>
    ///     Returns the lower left y-value.
    /// </summary>
    /// <returns>a value</returns>
    public float GetLly() => _lly;

    /// <summary>
    ///     Returns the lower left y-value.
    /// </summary>
    /// <param name="def">the default value</param>
    /// <returns>a value</returns>
    public float GetLly(float def)
    {
        if (float.IsNaN(_lly))
        {
            return def;
        }

        return _lly;
    }

    /// <summary>
    ///     Returns the uppper right x-value.
    /// </summary>
    /// <returns>a value</returns>
    public float GetUrx() => _urx;

    /// <summary>
    ///     Returns the upper right x-value.
    /// </summary>
    /// <param name="def">the default value</param>
    /// <returns>a value</returns>
    public float GetUrx(float def)
    {
        if (float.IsNaN(_urx))
        {
            return def;
        }

        return _urx;
    }

    /// <summary>
    ///     Returns the uppper right y-value.
    /// </summary>
    /// <returns>a value</returns>
    public float GetUry() => _ury;

    /// <summary>
    ///     Returns the upper right y-value.
    /// </summary>
    /// <param name="def">the default value</param>
    /// <returns>a value</returns>
    public float GetUry(float def)
    {
        if (float.IsNaN(_ury))
        {
            return def;
        }

        return _ury;
    }

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Sets the dimensions of this annotation.
    /// </summary>
    /// <param name="llx">the lower left x-value</param>
    /// <param name="lly">the lower left y-value</param>
    /// <param name="urx">the upper right x-value</param>
    /// <param name="ury">the upper right y-value</param>
    public void SetDimensions(float llx, float lly, float urx, float ury)
    {
        _llx = llx;
        _lly = lly;
        _urx = urx;
        _ury = ury;
    }
}