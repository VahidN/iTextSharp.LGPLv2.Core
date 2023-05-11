namespace iTextSharp.text;

/// <summary>
///     A class that implements DocListener will perform some
///     actions when some actions are performed on a Document.
/// </summary>
public interface IDocListener : IElementListener, IDisposable
{
    /// <summary>
    ///     methods
    /// </summary>
    /// <summary>
    ///     Changes the footer of this document.
    /// </summary>
    /// <value>a Footer</value>
    HeaderFooter Footer { set; }

    /// <summary>
    ///     Changes the header of this document.
    /// </summary>
    /// <value>a Header</value>
    HeaderFooter Header { set; }

    /// <summary>
    ///     Sets the page number.
    /// </summary>
    /// <value>the new page number</value>
    int PageCount { set; }

    /// <summary>
    ///     Signals that the Document was closed and that no other
    ///     Elements will be added.
    /// </summary>
    /// <remarks>
    ///     The output stream of every writer implementing IDocListener will be closed.
    /// </remarks>
    void Close();

    /// <summary>
    ///     Signals that an new page has to be started.
    /// </summary>
    /// <returns>true if the page was added, false if not.</returns>
    bool NewPage();

    /// <summary>
    ///     Signals that the Document has been opened and that
    ///     Elements can be added.
    /// </summary>
    void Open();

    /// <summary>
    ///     Resets the footer of this document.
    /// </summary>
    void ResetFooter();

    /// <summary>
    ///     Resets the header of this document.
    /// </summary>
    void ResetHeader();

    /// <summary>
    ///     Sets the page number to 0.
    /// </summary>
    void ResetPageCount();

    /// <summary>
    ///     Parameter that allows you to do margin mirroring (odd/even pages)
    /// </summary>
    /// <param name="marginMirroring"></param>
    /// <returns>true if succesfull</returns>
    bool SetMarginMirroring(bool marginMirroring);

    /// <summary>
    ///     Parameter that allows you to do top/bottom margin mirroring (odd/even pages)
    ///     @since	2.1.6
    /// </summary>
    /// <param name="marginMirroringTopBottom"></param>
    /// <returns>true if successful</returns>
    bool SetMarginMirroringTopBottom(bool marginMirroringTopBottom);

    /// <summary>
    ///     Sets the margins.
    /// </summary>
    /// <param name="marginLeft">the margin on the left</param>
    /// <param name="marginRight">the margin on the right</param>
    /// <param name="marginTop">the margin on the top</param>
    /// <param name="marginBottom">the margin on the bottom</param>
    /// <returns></returns>
    bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom);

    /// <summary>
    ///     Sets the pagesize.
    /// </summary>
    /// <param name="pageSize">the new pagesize</param>
    /// <returns>a boolean</returns>
    bool SetPageSize(Rectangle pageSize);
}