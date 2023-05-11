using System.util;
using iTextSharp.text.pdf;

namespace iTextSharp.text;

/// <summary>
///     An abstract Writer class for documents.
/// </summary>
/// <remarks>
///     DocWriter is the abstract class of several writers such
///     as PdfWriter and HtmlWriter.
///     A DocWriter can be added as a DocListener
///     to a certain Document by getting an instance (see method
///     GetInstance() in the specific writer-classes).
///     Every Element added to the original Document
///     will be written to the stream of the listening
///     DocWriter.
/// </remarks>
public abstract class DocWriter : IDocListener
{
    /// <summary> This is some byte that is often used. </summary>
    public const byte EQUALS = (byte)'=';

    /// <summary> This is some byte that is often used. </summary>
    public const byte FORWARD = (byte)'/';

    /// <summary> This is some byte that is often used. </summary>
    public const byte GT = (byte)'>';

    /// <summary> This is some byte that is often used. </summary>
    public const byte LT = (byte)'<';

    /// <summary> This is some byte that is often used. </summary>
    public const byte NEWLINE = (byte)'\n';

    /// <summary> This is some byte that is often used. </summary>
    public const byte QUOTE = (byte)'\"';

    /// <summary> This is some byte that is often used. </summary>
    public const byte SPACE = (byte)' ';

    /// <summary> This is some byte that is often used. </summary>
    public const byte TAB = (byte)'\t';

    /// <summary>
    ///     Closes the stream on document close
    /// </summary>
    protected bool closeStream = true;

    /// <summary> This is the document that has to be written. </summary>
    protected Document Document;

    /// <summary> Is the writer open for writing? </summary>
    protected bool open;

    /// <summary>
    ///     membervariables
    /// </summary>
    /// <summary> The stream of this writer. </summary>
    public OutputStreamCounter Os;

    /// <summary> The pageSize. </summary>
    protected Rectangle PageSize;

    /// <summary> Do we have to pause all writing actions? </summary>
    protected bool pause;

    /// <summary>
    ///     constructor
    /// </summary>
    protected DocWriter()
    {
    }

    /// <summary>
    ///     Constructs a DocWriter.
    /// </summary>
    /// <param name="document">The Document that has to be written</param>
    /// <param name="os">The Stream the writer has to write to.</param>
    protected DocWriter(Document document, Stream os)
    {
        Document = document;
        Os = new OutputStreamCounter(os);
    }

    /// <summary>
    ///     implementation of the DocListener methods
    /// </summary>

    public virtual bool CloseStream
    {
        get => closeStream;
        set => closeStream = value;
    }

    /// <summary>
    ///     Changes the footer of this document.
    /// </summary>
    /// <remarks>
    ///     This method should be overriden in the specific DocWriter classes
    ///     derived from this abstract class if they actually support the use of
    ///     footers.
    /// </remarks>
    /// <value>the new footer</value>
    public virtual HeaderFooter Footer
    {
        set { }
    }

    /// <summary>
    ///     Changes the header of this document.
    /// </summary>
    /// <remarks>
    ///     This method should be overriden in the specific DocWriter classes
    ///     derived from this abstract class if they actually support the use of
    ///     headers.
    /// </remarks>
    /// <value>the new header</value>
    public virtual HeaderFooter Header
    {
        set { }
    }

    /// <summary>
    ///     Sets the page number.
    /// </summary>
    /// <remarks>
    ///     This method should be overriden in the specific DocWriter classes
    ///     derived from this abstract class if they actually support the use of
    ///     pagenumbers.
    /// </remarks>
    public virtual int PageCount
    {
        set { }
    }

    /// <summary>
    ///     Signals that an Element was added to the Document.
    /// </summary>
    /// <remarks>
    ///     This method should be overriden in the specific DocWriter classes
    ///     derived from this abstract class.
    /// </remarks>
    /// <param name="element"></param>
    /// <returns>false</returns>
    public virtual bool Add(IElement element) => false;

    /// <summary>
    ///     Signals that the Document was closed and that no other
    ///     Elements will be added.
    /// </summary>
    public virtual void Close()
    {
        open = false;
        Os.Flush();
        if (closeStream)
        {
            Os.Dispose();
        }
    }

    /// <summary>
    ///     Signals that an new page has to be started.
    /// </summary>
    /// <remarks>
    ///     This does nothing. Has to be overridden if needed.
    /// </remarks>
    /// <returns>true if the page was added, false if not.</returns>
    public virtual bool NewPage()
    {
        if (!open)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Signals that the Document was opened.
    /// </summary>
    public virtual void Open()
    {
        open = true;
    }

    /// <summary>
    ///     Resets the footer of this document.
    /// </summary>
    /// <remarks>
    ///     This method should be overriden in the specific DocWriter classes
    ///     derived from this abstract class if they actually support the use of
    ///     footers.
    /// </remarks>
    public virtual void ResetFooter()
    {
    }

    /// <summary>
    ///     Resets the header of this document.
    /// </summary>
    /// <remarks>
    ///     This method should be overriden in the specific DocWriter classes
    ///     derived from this abstract class if they actually support the use of
    ///     headers.
    /// </remarks>
    public virtual void ResetHeader()
    {
    }

    /// <summary>
    ///     Sets the page number to 0.
    /// </summary>
    /// <remarks>
    ///     This method should be overriden in the specific DocWriter classes
    ///     derived from this abstract class if they actually support the use of
    ///     pagenumbers.
    /// </remarks>
    public virtual void ResetPageCount()
    {
    }

    public virtual bool SetMarginMirroring(bool marginMirroring) => false;

    /// <summary>
    ///     @see com.lowagie.text.DocListener#setMarginMirroring(boolean)
    ///     @since	2.1.6
    /// </summary>
    public virtual bool SetMarginMirroringTopBottom(bool marginMirroringTopBottom) => false;

    /// <summary>
    ///     Sets the margins.
    /// </summary>
    /// <remarks>
    ///     This does nothing. Has to be overridden if needed.
    /// </remarks>
    /// <param name="marginLeft">the margin on the left</param>
    /// <param name="marginRight">the margin on the right</param>
    /// <param name="marginTop">the margin on the top</param>
    /// <param name="marginBottom">the margin on the bottom</param>
    /// <returns></returns>
    public virtual bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom) => false;

    /// <summary>
    ///     Sets the pagesize.
    /// </summary>
    /// <param name="pageSize">the new pagesize</param>
    /// <returns>a boolean</returns>
    public virtual bool SetPageSize(Rectangle pageSize)
    {
        PageSize = pageSize;
        return true;
    }

    public void Dispose()
    {
        Close();
    }

    /// <summary>
    ///     Converts a string into a Byte array
    ///     according to the ISO-8859-1 codepage.
    /// </summary>
    /// <param name="text">the text to be converted</param>
    /// <returns>the conversion result</returns>
    public static byte[] GetIsoBytes(string text)
    {
        if (text == null)
        {
            return null;
        }

        var len = text.Length;
        var b = new byte[len];
        for (var k = 0; k < len; ++k)
        {
            b[k] = (byte)text[k];
        }

        return b;
    }

    /// <summary>
    ///     Flushes the Stream.
    /// </summary>
    public virtual void Flush()
    {
        Os.Flush();
    }

    public bool IsPaused() => pause;

    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Let the writer know that all writing has to be paused.
    /// </summary>
    public virtual void Pause()
    {
        pause = true;
    }

    /// <summary>
    ///     Checks if writing is paused.
    /// </summary>
    /// <returns> true  if writing temporarely has to be paused,  false  otherwise.</returns>
    /// <summary>
    ///     Let the writer know that writing may be resumed.
    /// </summary>
    public virtual void Resume()
    {
        pause = false;
    }

    /// <summary>
    ///     Writes a number of tabs.
    /// </summary>
    /// <param name="indent">the number of tabs to add</param>
    protected void AddTabs(int indent)
    {
        Os.WriteByte(NEWLINE);
        for (var i = 0; i < indent; i++)
        {
            Os.WriteByte(TAB);
        }
    }

    /// <summary>
    ///     Writes a string to the stream.
    /// </summary>
    /// <param name="str">the string to write</param>
    protected void Write(string str)
    {
        var tmp = GetIsoBytes(str);
        Os.Write(tmp, 0, tmp.Length);
    }

    /// <summary>
    ///     Writes a key-value pair to the stream.
    /// </summary>
    /// <param name="key">the name of an attribute</param>
    /// <param name="value">the value of an attribute</param>
    protected void Write(string key, string value)
    {
        Os.WriteByte(SPACE);
        Write(key);
        Os.WriteByte(EQUALS);
        Os.WriteByte(QUOTE);
        Write(value);
        Os.WriteByte(QUOTE);
    }

    /// <summary>
    ///     Writes an endtag to the stream.
    /// </summary>
    /// <param name="tag">the name of the tag</param>
    protected void WriteEnd(string tag)
    {
        Os.WriteByte(LT);
        Os.WriteByte(FORWARD);
        Write(tag);
        Os.WriteByte(GT);
    }

    /// <summary>
    ///     Writes an endtag to the stream.
    /// </summary>
    protected void WriteEnd()
    {
        Os.WriteByte(SPACE);
        Os.WriteByte(FORWARD);
        Os.WriteByte(GT);
    }

    /// <summary>
    ///     Writes the markup attributes of the specified MarkupAttributes
    ///     object to the stream.
    /// </summary>
    /// <param name="markup">the MarkupAttributes to write.</param>
    /// <returns></returns>
    protected bool WriteMarkupAttributes(Properties markup)
    {
        if (markup == null)
        {
            return false;
        }

        foreach (var name in markup.Keys)
        {
            Write(name, markup[name]);
        }

        markup.Clear();
        return true;
    }

    /// <summary>
    ///     Writes a starttag to the stream.
    /// </summary>
    /// <param name="tag">the name of the tag</param>
    protected void WriteStart(string tag)
    {
        Os.WriteByte(LT);
        Write(tag);
    }
}