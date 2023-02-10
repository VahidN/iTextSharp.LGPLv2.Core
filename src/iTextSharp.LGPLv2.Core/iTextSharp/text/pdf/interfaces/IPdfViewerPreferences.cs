namespace iTextSharp.text.pdf.interfaces;

/// <summary>
///     Viewer preferences are described in section 3.6.1 and 8.1 of the
///     PDF Reference 1.7 (Table 3.25 on p139-142 and Table 8.1 on p579-581).
///     They are explained in section 13.1 of the book 'iText in Action'.
///     The values of the different preferences were originally stored
///     in class PdfWriter, but they have been moved to this separate interface
///     for reasons of convenience.
/// </summary>
public interface IPdfViewerPreferences
{
    /// <summary>
    ///     Sets the page layout and page mode preferences by ORing one or two of these constants.
    ///     The page layout to be used when the document is opened (choose one).
    ///     PAGE_LAYOUT_SINGLE_PAGE  - Display one page at a time. (default)
    ///     PAGE_LAYOUT_ONE_COLUMN  - Display the pages in one column.
    ///     PAGE_LAYOUT_TWO_COLUMN_LEFT  - Display the pages in two columns, with
    ///     oddnumbered pages on the left.
    ///     PAGE_LAYOUT_TWO_COLUMN_RIGHT  - Display the pages in two columns, with
    ///     oddnumbered pages on the right.
    ///     PAGE_LAYOUT_TWO_PAGE_LEFT  - Display the pages two at a time, with
    ///     oddnumbered pages on the left.
    ///     PAGE_LAYOUT_TWO_PAGE_RIGHT  - Display the pages two at a time, with
    ///     oddnumbered pages on the right.
    ///     The page mode how the document should be displayed
    ///     when opened (choose one).
    ///     PAGE_MODE_USE_NONE  - Neither document outline nor thumbnail images visible. (default)
    ///     PAGE_MODE_USE_OUTLINES  - Document outline visible.
    ///     PAGE_MODE_USE_THUMBS  - Thumbnail images visible.
    ///     PAGE_MODE_FULL_SCREEN  - Full-screen mode, with no menu bar, window
    ///     controls, or any other window visible.
    ///     PAGE_MODE_USE_OC  - Optional content group panel visible
    ///     PAGE_MODE_USE_ATTACHMENTS  - Attachments panel visible
    ///     For backward compatibility these values are also supported,
    ///     but it's better to use method  addViewerPreference(key, value)
    ///     if you want to change the following preferences:
    ///     HIDE_TOOLBAR  - A flag specifying whether to hide the viewer application's tool
    ///     bars when the document is active.
    ///     HIDE_MENUBAR  - A flag specifying whether to hide the viewer application's
    ///     menu bar when the document is active.
    ///     HIDE_WINDOW_UI  - A flag specifying whether to hide user interface elements in
    ///     the document's window (such as scroll bars and navigation controls),
    ///     leaving only the document's contents displayed.
    ///     FIT_WINDOW  - A flag specifying whether to resize the document's window to
    ///     fit the size of the first displayed page.
    ///     CENTER_WINDOW  - A flag specifying whether to position the document's window
    ///     in the center of the screen.
    ///     DISPLAY_DOC_TITLE  - A flag specifying whether to display the document's title
    ///     in the top bar.
    ///     The predominant reading order for text. This entry has no direct effect on the
    ///     document's contents or page numbering, but can be used to determine the relative
    ///     positioning of pages when displayed side by side or printed <i>n-up</i> (choose one).
    ///     DIRECTION_L2R  - Left to right
    ///     DIRECTION_R2L  - Right to left (including vertical writing systems such as
    ///     Chinese, Japanese, and Korean)
    ///     The document's page mode, specifying how to display the
    ///     document on exiting full-screen mode. It is meaningful only
    ///     if the page mode is  PageModeFullScreen  (choose one).
    ///     NON_FULL_SCREEN_PAGE_MODE_USE_NONE  - Neither document outline nor thumbnail images
    ///     visible
    ///     NON_FULL_SCREEN_PAGE_MODE_USE_OUTLINES  - Document outline visible
    ///     NON_FULL_SCREEN_PAGE_MODE_USE_THUMBS  - Thumbnail images visible
    ///     NON_FULL_SCREEN_PAGE_MODE_USE_OC  - Optional content group panel visible
    ///     PRINT_SCALING_NONE  - Indicates that the print dialog should reflect no page scaling.
    ///     @see PdfViewerPreferences#addViewerPreference
    /// </summary>
    int ViewerPreferences { set; }

    /// <summary>
    ///     Adds a viewer preference.
    ///     In case the key is one of these values:
    ///     PdfName. HIDETOOLBAR
    ///     PdfName. HIDEMENUBAR
    ///     PdfName. HIDEWINDOWUI
    ///     PdfName. FITWINDOW
    ///     PdfName. CENTERWINDOW
    ///     PdfName. DISPLAYDOCTITLE
    ///     The value must be a of type PdfBoolean (true or false).
    ///     In case the key is PdfName. NONFULLSCREENPAGEMODE ,
    ///     the value must be one of these names:
    ///     PdfName. USENONE
    ///     PdfName. USEOUTLINES
    ///     PdfName. USETHUMBS
    ///     PdfName. USEOC
    ///     In case the key is PdfName.DIRECTION,
    ///     the value must be one of these names:
    ///     PdfName. L2R
    ///     PdfName. R2L
    ///     In case the key is one of these values:
    ///     PdfName. VIEWAREA
    ///     PdfName. VIEWCLIP
    ///     PdfName. PRINTAREA
    ///     PdfName. PRINTCLIP
    ///     The value must be one of these names:
    ///     PdfName. MEDIABOX
    ///     PdfName. CROPBOX
    ///     PdfName. BLEEDBOX
    ///     PdfName. TRIMBOX
    ///     PdfName. ARTBOX
    ///     In case the key is PdfName. PRINTSCALING , the value can be
    ///     PdfName. APPDEFAULT
    ///     PdfName. NONE
    ///     In case the key is PdfName. DUPLEX , the value can be:
    ///     PdfName. SIMPLEX
    ///     PdfName. DUPLEXFLIPSHORTEDGE
    ///     PdfName. DUPLEXFLIPLONGEDGE
    ///     In case the key is PdfName. PICKTRAYBYPDFSIZE , the value must be of type PdfBoolean.
    ///     In case the key is PdfName. PRINTPAGERANGE , the value must be of type PdfArray.
    ///     In case the key is PdfName. NUMCOPIES , the value must be of type PdfNumber.
    ///     @see PdfViewerPreferences#setViewerPreferences
    /// </summary>
    /// <param name="key">name of the viewer preference</param>
    /// <param name="value">value of the viewer preference</param>
    void AddViewerPreference(PdfName key, PdfObject value);
}