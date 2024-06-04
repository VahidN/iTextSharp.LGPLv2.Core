using System.util;
using iTextSharp.text.pdf.collection;

namespace iTextSharp.text.pdf;

/// <summary>
///     A  PdfAction  defines an action that can be triggered from a PDF file.
///     @see     PdfDictionary
/// </summary>
public class PdfAction : PdfDictionary
{
    /// <summary>
    ///     A named action to go to the first page.
    /// </summary>
    public const int FIRSTPAGE = 1;

    /// <summary>
    ///     A named action to go to the last page.
    /// </summary>
    public const int LASTPAGE = 4;

    /// <summary>
    ///     A named action to go to the next page.
    /// </summary>
    public const int NEXTPAGE = 3;

    /// <summary>
    ///     A named action to go to the previous page.
    /// </summary>
    public const int PREVPAGE = 2;

    /// <summary>
    ///     A named action to open a print dialog.
    /// </summary>
    public const int PRINTDIALOG = 5;

    /// <summary>
    ///     a possible submitvalue
    /// </summary>
    public const int RESET_EXCLUDE = 1;

    /// <summary>
    ///     a possible submitvalue
    /// </summary>
    public const int SUBMIT_CANONICAL_FORMAT = 512;

    public const int SUBMIT_COORDINATES = 16;

    /// <summary>
    ///     a possible submitvalue
    /// </summary>
    public const int SUBMIT_EMBED_FORM = 8196;

    /// <summary>
    ///     a possible submitvalue
    /// </summary>
    public const int SUBMIT_EXCL_F_KEY = 2048;

    /// <summary>
    ///     a possible submitvalue
    /// </summary>
    public const int SUBMIT_EXCL_NON_USER_ANNOTS = 1024;

    /// <summary>
    ///     constructors
    /// </summary>
    public const int SUBMIT_EXCLUDE = 1;

    public const int SUBMIT_HTML_FORMAT = 4;
    public const int SUBMIT_HTML_GET = 8;

    /// <summary>
    ///     a possible submitvalue
    /// </summary>
    public const int SUBMIT_INCLUDE_ANNOTATIONS = 128;

    /// <summary>
    ///     a possible submitvalue
    /// </summary>
    public const int SUBMIT_INCLUDE_APPEND_SAVES = 64;

    public const int SUBMIT_INCLUDE_NO_VALUE_FIELDS = 2;

    /// <summary>
    ///     a possible submitvalue
    /// </summary>
    public const int SUBMIT_PDF = 256;

    /// <summary>
    ///     a possible submitvalue
    /// </summary>
    public const int SUBMIT_XFDF = 32;

    /// <summary>
    ///     Create an empty action.
    /// </summary>
    public PdfAction()
    {
    }

    /// <summary>
    ///     Constructs a new  PdfAction  of Subtype URI.
    /// </summary>
    /// <param name="url">the Url to go to</param>
    public PdfAction(Uri url) : this(url?.AbsoluteUri ?? throw new ArgumentNullException(nameof(url)))
    {
    }

    public PdfAction(Uri url, bool isMap) : this(url?.AbsoluteUri ?? throw new ArgumentNullException(nameof(url)),
                                                 isMap)
    {
    }

    /// <summary>
    ///     Constructs a new  PdfAction  of Subtype URI.
    /// </summary>
    /// <param name="url">the url to go to</param>
    public PdfAction(string url) : this(url, false)
    {
    }

    public PdfAction(string url, bool isMap)
    {
        Put(PdfName.S, PdfName.Uri);
        Put(PdfName.Uri, new PdfString(url));
        if (isMap)
        {
            Put(PdfName.Ismap, PdfBoolean.Pdftrue);
        }
    }

    /// <summary>
    ///     Constructs a new  PdfAction  of Subtype GoTo.
    /// </summary>
    public PdfAction(string filename, string name)
    {
        Put(PdfName.S, PdfName.Gotor);
        Put(PdfName.F, new PdfString(filename));
        Put(PdfName.D, new PdfString(name));
    }

    public PdfAction(string filename, int page)
    {
        Put(PdfName.S, PdfName.Gotor);
        Put(PdfName.F, new PdfString(filename));
        Put(PdfName.D, new PdfLiteral("[" + (page - 1) + " /FitH 10000]"));
    }

    /// <summary>
    ///     Constructs a new  PdfAction  of Subtype GoToR.
    /// </summary>
    /// <param name="named">the named destination to go to</param>
    public PdfAction(int named)
    {
        Put(PdfName.S, PdfName.Named);
        switch (named)
        {
            case FIRSTPAGE:
                Put(PdfName.N, PdfName.Firstpage);
                break;
            case LASTPAGE:
                Put(PdfName.N, PdfName.Lastpage);
                break;
            case NEXTPAGE:
                Put(PdfName.N, PdfName.Nextpage);
                break;
            case PREVPAGE:
                Put(PdfName.N, PdfName.Prevpage);
                break;
            case PRINTDIALOG:
                Put(PdfName.S, PdfName.Javascript);
                Put(PdfName.Js, new PdfString("this.print(true);\r"));
                break;
            default:
                throw new ArgumentException("Invalid named action.");
        }
    }

    /// <summary>
    ///     Launchs an application or a document.
    ///     It can be  null .
    ///     "print" - Print a document.
    ///     It can be  null .
    ///     It can be  null .
    /// </summary>
    /// <param name="application">the application to be launched or the document to be opened or printed.</param>
    /// <param name="parameters">(Windows-specific) A parameter string to be passed to the application.</param>
    /// <param name="operation">(Windows-specific) the operation to perform: "open" - Open a document,</param>
    /// <param name="defaultDir">(Windows-specific) the default directory in standard DOS syntax.</param>
    public PdfAction(string application, string parameters, string operation, string defaultDir)
    {
        Put(PdfName.S, PdfName.Launch);
        if (parameters == null && operation == null && defaultDir == null)
        {
            Put(PdfName.F, new PdfString(application));
        }
        else
        {
            var dic = new PdfDictionary();
            dic.Put(PdfName.F, new PdfString(application));
            if (parameters != null)
            {
                dic.Put(PdfName.P, new PdfString(parameters));
            }

            if (operation != null)
            {
                dic.Put(PdfName.O, new PdfString(operation));
            }

            if (defaultDir != null)
            {
                dic.Put(PdfName.D, new PdfString(defaultDir));
            }

            Put(PdfName.Win, dic);
        }
    }

    internal PdfAction(PdfIndirectReference destination)
    {
        Put(PdfName.S, PdfName.Goto);
        Put(PdfName.D, destination);
    }

    public static PdfAction CreateHide(PdfAnnotation annot, bool hide)
    {
        if (annot == null)
        {
            throw new ArgumentNullException(nameof(annot));
        }

        return CreateHide(annot.IndirectReference, hide);
    }

    public static PdfAction CreateHide(string name, bool hide) => CreateHide(new PdfString(name), hide);

    public static PdfAction CreateHide(object[] names, bool hide)
    {
        if (names == null)
        {
            throw new ArgumentNullException(nameof(names));
        }

        return CreateHide(BuildArray(names), hide);
    }

    public static PdfAction CreateImportData(string file)
    {
        var action = new PdfAction();
        action.Put(PdfName.S, PdfName.Importdata);
        action.Put(PdfName.F, new PdfString(file));
        return action;
    }

    /// <summary>
    ///     Launchs an application or a document.
    ///     It can be  null .
    ///     "print" - Print a document.
    ///     It can be  null .
    ///     It can be  null .
    /// </summary>
    /// <param name="application">the application to be launched or the document to be opened or printed.</param>
    /// <param name="parameters">(Windows-specific) A parameter string to be passed to the application.</param>
    /// <param name="operation">(Windows-specific) the operation to perform: "open" - Open a document,</param>
    /// <param name="defaultDir">(Windows-specific) the default directory in standard DOS syntax.</param>
    /// <returns>a Launch action</returns>
    public static PdfAction CreateLaunch(string application, string parameters, string operation, string defaultDir) =>
        new(application, parameters, operation, defaultDir);

    public static PdfAction CreateResetForm(object[] names, int flags)
    {
        var action = new PdfAction();
        action.Put(PdfName.S, PdfName.Resetform);
        if (names != null)
        {
            action.Put(PdfName.Fields, BuildArray(names));
        }

        action.Put(PdfName.Flags, new PdfNumber(flags));
        return action;
    }

    public static PdfAction CreateSubmitForm(string file, object[] names, int flags)
    {
        var action = new PdfAction();
        action.Put(PdfName.S, PdfName.Submitform);
        var dic = new PdfDictionary();
        dic.Put(PdfName.F, new PdfString(file));
        dic.Put(PdfName.Fs, PdfName.Url);
        action.Put(PdfName.F, dic);
        if (names != null)
        {
            action.Put(PdfName.Fields, BuildArray(names));
        }

        action.Put(PdfName.Flags, new PdfNumber(flags));
        return action;
    }

    /// <summary>
    ///     Creates a GoToE action to an embedded file.
    /// </summary>
    /// <param name="filename">the root document of the target (null if the target is in the same document)</param>
    /// <param name="target"></param>
    /// <param name="dest">the named destination</param>
    /// <param name="isName">if true sets the destination as a name, if false sets it as a String</param>
    /// <param name="newWindow"></param>
    /// <returns>a GoToE action</returns>
    public static PdfAction GotoEmbedded(string filename, PdfTargetDictionary target, string dest, bool isName,
                                         bool newWindow)
    {
        if (isName)
        {
            return GotoEmbedded(filename, target, new PdfName(dest), newWindow);
        }

        return GotoEmbedded(filename, target, new PdfString(dest, null), newWindow);
    }

    /// <summary>
    ///     Creates a GoToE action to an embedded file.
    /// </summary>
    /// <param name="filename">the root document of the target (null if the target is in the same document)</param>
    /// <param name="target">a path to the target document of this action</param>
    /// <param name="dest">the destination inside the target document, can be of type PdfDestination, PdfName, or PdfString</param>
    /// <param name="newWindow">if true, the destination document should be opened in a new window</param>
    /// <returns>a GoToE action</returns>
    public static PdfAction GotoEmbedded(string filename, PdfTargetDictionary target, PdfObject dest, bool newWindow)
    {
        var action = new PdfAction();
        action.Put(PdfName.S, PdfName.Gotoe);
        action.Put(PdfName.T, target);
        action.Put(PdfName.D, dest);
        action.Put(PdfName.Newwindow, new PdfBoolean(newWindow));
        if (filename != null)
        {
            action.Put(PdfName.F, new PdfString(filename));
        }

        return action;
    }

    /// <summary>
    ///     Creates a GoTo action to an internal page.
    /// </summary>
    /// <param name="page">the page to go. First page is 1</param>
    /// <param name="dest">the destination for the page</param>
    /// <param name="writer">the writer for this action</param>
    /// <returns>a GoTo action</returns>
    public static PdfAction GotoLocalPage(int page, PdfDestination dest, PdfWriter writer)
    {
        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        var piref = writer.GetPageReference(page);
        dest.AddPage(piref);
        var action = new PdfAction();
        action.Put(PdfName.S, PdfName.Goto);
        action.Put(PdfName.D, dest);
        return action;
    }

    /// <summary>
    ///     Creates a GoTo action to a named destination.
    /// </summary>
    /// <param name="dest">the named destination</param>
    /// <param name="isName">if true sets the destination as a name, if false sets it as a String</param>
    /// <returns>a GoToR action</returns>
    public static PdfAction GotoLocalPage(string dest, bool isName)
    {
        var action = new PdfAction();
        action.Put(PdfName.S, PdfName.Goto);
        if (isName)
        {
            action.Put(PdfName.D, new PdfName(dest));
        }
        else
        {
            action.Put(PdfName.D, new PdfString(dest, null));
        }

        return action;
    }

    /// <summary>
    ///     Creates a GoToR action to a named destination.
    /// </summary>
    /// <param name="filename">the file name to go to</param>
    /// <param name="dest">the destination name</param>
    /// <param name="isName">if true sets the destination as a name, if false sets it as a String</param>
    /// <param name="newWindow">
    ///     open the document in a new window if  true , if false the current document is replaced by the
    ///     new document.
    /// </param>
    /// <returns>a GoToR action</returns>
    public static PdfAction GotoRemotePage(string filename, string dest, bool isName, bool newWindow)
    {
        var action = new PdfAction();
        action.Put(PdfName.F, new PdfString(filename));
        action.Put(PdfName.S, PdfName.Gotor);
        if (isName)
        {
            action.Put(PdfName.D, new PdfName(dest));
        }
        else
        {
            action.Put(PdfName.D, new PdfString(dest, null));
        }

        if (newWindow)
        {
            action.Put(PdfName.Newwindow, PdfBoolean.Pdftrue);
        }

        return action;
    }

    /// <summary>
    ///     Creates a JavaScript action. If the JavaScript is smaller than
    ///     50 characters it will be placed as a string, otherwise it will
    ///     be placed as a compressed stream.
    ///     Acrobat JavaScript engine does not support unicode,
    ///     so this may or may not work for you
    /// </summary>
    /// <param name="code">the JavaScript code</param>
    /// <param name="writer">the writer for this action</param>
    /// <param name="unicode">select JavaScript unicode. Note that the internal</param>
    /// <returns>the JavaScript action</returns>
    public static PdfAction JavaScript(string code, PdfWriter writer, bool unicode)
    {
        if (code == null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        if (writer == null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        var js = new PdfAction();
        js.Put(PdfName.S, PdfName.Javascript);
        if (unicode && code.Length < 50)
        {
            js.Put(PdfName.Js, new PdfString(code, TEXT_UNICODE));
        }
        else if (!unicode && code.Length < 100)
        {
            js.Put(PdfName.Js, new PdfString(code));
        }
        else
        {
            try
            {
                var b = PdfEncodings.ConvertToBytes(code, unicode ? TEXT_UNICODE : TEXT_PDFDOCENCODING);
                var stream = new PdfStream(b);
                stream.FlateCompress(writer.CompressionLevel);
                js.Put(PdfName.Js, writer.AddToBody(stream).IndirectReference);
            }
            catch
            {
                js.Put(PdfName.Js, new PdfString(code));
            }
        }

        return js;
    }

    /// <summary>
    ///     Creates a JavaScript action. If the JavaScript is smaller than
    ///     50 characters it will be place as a string, otherwise it will
    ///     be placed as a compressed stream.
    /// </summary>
    /// <param name="code">the JavaScript code</param>
    /// <param name="writer">the writer for this action</param>
    /// <returns>the JavaScript action</returns>
    public static PdfAction JavaScript(string code, PdfWriter writer) => JavaScript(code, writer, false);

    /// <summary>
    ///     Creates a Rendition action
    ///     @throws IOException
    /// </summary>
    /// <param name="file"></param>
    /// <param name="fs"></param>
    /// <param name="mimeType"></param>
    /// <param name="refi"></param>
    /// <returns>a Media Clip action</returns>
    public static PdfAction Rendition(string file, PdfFileSpecification fs, string mimeType, PdfIndirectReference refi)
    {
        var js = new PdfAction();
        js.Put(PdfName.S, PdfName.Rendition);
        js.Put(PdfName.R, new PdfRendition(file, fs, mimeType));
        js.Put(new PdfName("OP"), new PdfNumber(0));
        js.Put(new PdfName("AN"), refi);
        return js;
    }

    /// <summary>
    ///     A set-OCG-state action (PDF 1.5) sets the state of one or more optional content
    ///     groups.
    ///     or  String  (ON, OFF, or Toggle) followed by one or more optional content group dictionaries
    ///     PdfLayer  or a  PdfIndirectReference  to a  PdfLayer .
    ///     The array elements are processed from left to right; each name is applied
    ///     to the subsequent groups until the next name is encountered:
    ///     ON sets the state of subsequent groups to ON
    ///     OFF sets the state of subsequent groups to OFF
    ///     Toggle reverses the state of subsequent groups
    ///     content groups (as specified by the RBGroups entry in the current configuration
    ///     dictionary) should be preserved when the states in the
    ///     state  array are applied. That is, if a group is set to ON (either by ON or Toggle) during
    ///     processing of the  state  array, any other groups belong to the same radio-button
    ///     group are turned OFF. If a group is set to OFF, there is no effect on other groups.
    ///     If  false , radio-button state relationships, if any, are ignored
    /// </summary>
    /// <param name="state">an array consisting of any number of sequences beginning with a  PdfName </param>
    /// <param name="preserveRb">if  true , indicates that radio-button state relationships between optional</param>
    /// <returns>the action</returns>
    public static PdfAction SetOcGstate(IList<object> state, bool preserveRb)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        var action = new PdfAction();
        action.Put(PdfName.S, PdfName.Setocgstate);
        var a = new PdfArray();
        for (var k = 0; k < state.Count; ++k)
        {
            var o = state[k];
            if (o == null)
            {
                continue;
            }

            if (o is PdfIndirectReference)
            {
                a.Add((PdfIndirectReference)o);
            }
            else if (o is PdfLayer)
            {
                a.Add(((PdfLayer)o).Ref);
            }
            else if (o is PdfName)
            {
                a.Add((PdfName)o);
            }
            else if (o is string)
            {
                PdfName name = null;
                var s = (string)o;
                if (Util.EqualsIgnoreCase(s, "on"))
                {
                    name = PdfName.On;
                }
                else if (Util.EqualsIgnoreCase(s, "off"))
                {
                    name = PdfName.OFF;
                }
                else if (Util.EqualsIgnoreCase(s, "toggle"))
                {
                    name = PdfName.Toggle;
                }
                else
                {
                    throw new ArgumentException("A string '" + s +
                                                " was passed in state. Only 'ON', 'OFF' and 'Toggle' are allowed.");
                }

                a.Add(name);
            }
            else
            {
                throw new ArgumentException("Invalid type was passed in state: " + o.GetType());
            }
        }

        action.Put(PdfName.State, a);
        if (!preserveRb)
        {
            action.Put(PdfName.Preserverb, PdfBoolean.Pdffalse);
        }

        return action;
    }

    /// <summary>
    ///     Add a chained action.
    /// </summary>
    /// <param name="na">the next action</param>
    public void Next(PdfAction na)
    {
        var nextAction = Get(PdfName.Next);
        if (nextAction == null)
        {
            Put(PdfName.Next, na);
        }
        else if (nextAction.IsDictionary())
        {
            var array = new PdfArray(nextAction);
            array.Add(na);
            Put(PdfName.Next, array);
        }
        else
        {
            ((PdfArray)nextAction).Add(na);
        }
    }

    internal static PdfArray BuildArray(object[] names)
    {
        var array = new PdfArray();
        for (var k = 0; k < names.Length; ++k)
        {
            var obj = names[k];
            if (obj is string)
            {
                array.Add(new PdfString((string)obj));
            }
            else if (obj is PdfAnnotation)
            {
                array.Add(((PdfAnnotation)obj).IndirectReference);
            }
            else
            {
                throw new ArgumentException("The array must contain string or PdfAnnotation.");
            }
        }

        return array;
    }

    internal static PdfAction CreateHide(PdfObject obj, bool hide)
    {
        var action = new PdfAction();
        action.Put(PdfName.S, PdfName.Hide);
        action.Put(PdfName.T, obj);
        if (!hide)
        {
            action.Put(PdfName.H, PdfBoolean.Pdffalse);
        }

        return action;
    }
}