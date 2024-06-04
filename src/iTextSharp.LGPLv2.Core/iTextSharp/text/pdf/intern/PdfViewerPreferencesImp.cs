using iTextSharp.text.pdf.interfaces;

namespace iTextSharp.text.pdf.intern;

/// <summary>
///     Stores the information concerning viewer preferences,
///     and contains the business logic that allows you to set viewer preferences.
/// </summary>
public class PdfViewerPreferencesImp : IPdfViewerPreferences
{
    /// <summary>
    ///     The mask to decide if a ViewerPreferences dictionary is needed
    /// </summary>
    private const int ViewerPreferencesMask = 0xfff000;

    /// <summary>
    ///     A series of viewer preferences.
    /// </summary>
    public static readonly PdfName[] DirectionPreferences =
    {
        PdfName.L2R, PdfName.R2L,
    };

    /// <summary>
    ///     A series of viewer preferences.
    /// </summary>
    public static readonly PdfName[] DuplexPreferences =
    {
        PdfName.Simplex, PdfName.Duplexflipshortedge,
        PdfName.Duplexfliplongedge,
    };

    /// <summary>
    ///     A series of viewer preferences.
    /// </summary>
    public static readonly PdfName[] NonfullscreenpagemodePreferences =
    {
        PdfName.Usenone, PdfName.Useoutlines,
        PdfName.Usethumbs, PdfName.Useoc,
    };

    /// <summary>
    ///     A series of viewer preferences.
    /// </summary>
    public static readonly PdfName[] PageBoundaries =
    {
        PdfName.Mediabox, PdfName.Cropbox, PdfName.Bleedbox,
        PdfName.Trimbox, PdfName.Artbox,
    };

    /// <summary>
    ///     A series of viewer preferences
    /// </summary>
    public static readonly PdfName[] PrintscalingPreferences =
    {
        PdfName.Appdefault, PdfName.None,
    };

    public static readonly PdfName[] VIEWER_PREFERENCES =
    {
        PdfName.Hidetoolbar, // 0
        PdfName.Hidemenubar, // 1
        PdfName.Hidewindowui, // 2
        PdfName.Fitwindow, // 3
        PdfName.Centerwindow, // 4
        PdfName.Displaydoctitle, // 5
        PdfName.Nonfullscreenpagemode, // 6
        PdfName.Direction, // 7
        PdfName.Viewarea, // 8
        PdfName.Viewclip, // 9
        PdfName.Printarea, // 10
        PdfName.Printclip, // 11
        PdfName.Printscaling, // 12
        PdfName.Duplex, // 13
        PdfName.Picktraybypdfsize, // 14
        PdfName.Printpagerange, // 15
        PdfName.Numcopies, // 16
    };

    /// <summary>
    ///     This dictionary holds the viewer preferences (other than page layout and page mode).
    /// </summary>
    private readonly PdfDictionary _viewerPreferences = new();

    /// <summary>
    ///     This value will hold the viewer preferences for the page layout and page mode.
    /// </summary>
    private int _pageLayoutAndMode;

    /// <summary>
    ///     Returns the page layout and page mode value.
    /// </summary>
    public int PageLayoutAndMode => _pageLayoutAndMode;

    /// <summary>
    ///     Sets the viewer preferences as the sum of several constants.
    ///     the viewer preferences
    ///     @see PdfWriter#setViewerPreferences
    /// </summary>
    public int ViewerPreferences
    {
        set
        {
            var preferences = value;
            _pageLayoutAndMode |= preferences;
            // for backwards compatibility, it is also possible
            // to set the following viewer preferences with this method:
            if ((preferences & ViewerPreferencesMask) != 0)
            {
                _pageLayoutAndMode = ~ViewerPreferencesMask & _pageLayoutAndMode;
                if ((preferences & PdfWriter.HideToolbar) != 0)
                {
                    _viewerPreferences.Put(PdfName.Hidetoolbar, PdfBoolean.Pdftrue);
                }

                if ((preferences & PdfWriter.HideMenubar) != 0)
                {
                    _viewerPreferences.Put(PdfName.Hidemenubar, PdfBoolean.Pdftrue);
                }

                if ((preferences & PdfWriter.HideWindowUI) != 0)
                {
                    _viewerPreferences.Put(PdfName.Hidewindowui, PdfBoolean.Pdftrue);
                }

                if ((preferences & PdfWriter.FitWindow) != 0)
                {
                    _viewerPreferences.Put(PdfName.Fitwindow, PdfBoolean.Pdftrue);
                }

                if ((preferences & PdfWriter.CenterWindow) != 0)
                {
                    _viewerPreferences.Put(PdfName.Centerwindow, PdfBoolean.Pdftrue);
                }

                if ((preferences & PdfWriter.DisplayDocTitle) != 0)
                {
                    _viewerPreferences.Put(PdfName.Displaydoctitle, PdfBoolean.Pdftrue);
                }

                if ((preferences & PdfWriter.NonFullScreenPageModeUseNone) != 0)
                {
                    _viewerPreferences.Put(PdfName.Nonfullscreenpagemode, PdfName.Usenone);
                }
                else if ((preferences & PdfWriter.NonFullScreenPageModeUseOutlines) != 0)
                {
                    _viewerPreferences.Put(PdfName.Nonfullscreenpagemode, PdfName.Useoutlines);
                }
                else if ((preferences & PdfWriter.NonFullScreenPageModeUseThumbs) != 0)
                {
                    _viewerPreferences.Put(PdfName.Nonfullscreenpagemode, PdfName.Usethumbs);
                }
                else if ((preferences & PdfWriter.NonFullScreenPageModeUseOC) != 0)
                {
                    _viewerPreferences.Put(PdfName.Nonfullscreenpagemode, PdfName.Useoc);
                }

                if ((preferences & PdfWriter.DirectionL2R) != 0)
                {
                    _viewerPreferences.Put(PdfName.Direction, PdfName.L2R);
                }
                else if ((preferences & PdfWriter.DirectionR2L) != 0)
                {
                    _viewerPreferences.Put(PdfName.Direction, PdfName.R2L);
                }

                if ((preferences & PdfWriter.PrintScalingNone) != 0)
                {
                    _viewerPreferences.Put(PdfName.Printscaling, PdfName.None);
                }
            }
        }
    }

    /// <summary>
    ///     Sets the viewer preferences for printing.
    /// </summary>
    public virtual void AddViewerPreference(PdfName key, PdfObject value)
    {
        switch (getIndex(key))
        {
            case 0: // HIDETOOLBAR
            case 1: // HIDEMENUBAR
            case 2: // HIDEWINDOWUI
            case 3: // FITWINDOW
            case 4: // CENTERWINDOW
            case 5: // DISPLAYDOCTITLE
            case 14: // PICKTRAYBYPDFSIZE
                if (value is PdfBoolean)
                {
                    _viewerPreferences.Put(key, value);
                }

                break;
            case 6: // NONFULLSCREENPAGEMODE
                if (value is PdfName
                    && isPossibleValue((PdfName)value, NonfullscreenpagemodePreferences))
                {
                    _viewerPreferences.Put(key, value);
                }

                break;
            case 7: // DIRECTION
                if (value is PdfName
                    && isPossibleValue((PdfName)value, DirectionPreferences))
                {
                    _viewerPreferences.Put(key, value);
                }

                break;
            case 8: // VIEWAREA
            case 9: // VIEWCLIP
            case 10: // PRINTAREA
            case 11: // PRINTCLIP
                if (value is PdfName
                    && isPossibleValue((PdfName)value, PageBoundaries))
                {
                    _viewerPreferences.Put(key, value);
                }

                break;
            case 12: // PRINTSCALING
                if (value is PdfName
                    && isPossibleValue((PdfName)value, PrintscalingPreferences))
                {
                    _viewerPreferences.Put(key, value);
                }

                break;
            case 13: // DUPLEX
                if (value is PdfName
                    && isPossibleValue((PdfName)value, DuplexPreferences))
                {
                    _viewerPreferences.Put(key, value);
                }

                break;
            case 15: // PRINTPAGERANGE
                if (value is PdfArray)
                {
                    _viewerPreferences.Put(key, value);
                }

                break;
            case 16: // NUMCOPIES
                if (value is PdfNumber)
                {
                    _viewerPreferences.Put(key, value);
                }

                break;
        }
    }

    public static PdfViewerPreferencesImp GetViewerPreferences(PdfDictionary catalog)
    {
        if (catalog == null)
        {
            throw new ArgumentNullException(nameof(catalog));
        }

        var preferences = new PdfViewerPreferencesImp();
        var prefs = 0;
        PdfName name = null;
        // page layout
        var obj = PdfReader.GetPdfObjectRelease(catalog.Get(PdfName.Pagelayout));
        if (obj != null && obj.IsName())
        {
            name = (PdfName)obj;
            if (name.Equals(PdfName.Singlepage))
            {
                prefs |= PdfWriter.PageLayoutSinglePage;
            }
            else if (name.Equals(PdfName.Onecolumn))
            {
                prefs |= PdfWriter.PageLayoutOneColumn;
            }
            else if (name.Equals(PdfName.Twocolumnleft))
            {
                prefs |= PdfWriter.PageLayoutTwoColumnLeft;
            }
            else if (name.Equals(PdfName.Twocolumnright))
            {
                prefs |= PdfWriter.PageLayoutTwoColumnRight;
            }
            else if (name.Equals(PdfName.Twopageleft))
            {
                prefs |= PdfWriter.PageLayoutTwoPageLeft;
            }
            else if (name.Equals(PdfName.Twopageright))
            {
                prefs |= PdfWriter.PageLayoutTwoPageRight;
            }
        }

        // page mode
        obj = PdfReader.GetPdfObjectRelease(catalog.Get(PdfName.Pagemode));
        if (obj != null && obj.IsName())
        {
            name = (PdfName)obj;
            if (name.Equals(PdfName.Usenone))
            {
                prefs |= PdfWriter.PageModeUseNone;
            }
            else if (name.Equals(PdfName.Useoutlines))
            {
                prefs |= PdfWriter.PageModeUseOutlines;
            }
            else if (name.Equals(PdfName.Usethumbs))
            {
                prefs |= PdfWriter.PageModeUseThumbs;
            }
            else if (name.Equals(PdfName.Fullscreen))
            {
                prefs |= PdfWriter.PageModeFullScreen;
            }
            else if (name.Equals(PdfName.Useoc))
            {
                prefs |= PdfWriter.PageModeUseOC;
            }
            else if (name.Equals(PdfName.Useattachments))
            {
                prefs |= PdfWriter.PageModeUseAttachments;
            }
        }

        // set page layout and page mode preferences
        preferences.ViewerPreferences = prefs;
        // other preferences
        obj = PdfReader.GetPdfObjectRelease(catalog
                                                .Get(PdfName.Viewerpreferences));
        if (obj != null && obj.IsDictionary())
        {
            var vp = (PdfDictionary)obj;
            for (var i = 0; i < VIEWER_PREFERENCES.Length; i++)
            {
                obj = PdfReader.GetPdfObjectRelease(vp.Get(VIEWER_PREFERENCES[i]));
                preferences.AddViewerPreference(VIEWER_PREFERENCES[i], obj);
            }
        }

        return preferences;
    }

    /// <summary>
    ///     Adds the viewer preferences defined in the preferences parameter to a
    ///     PdfDictionary (more specifically the root or catalog of a PDF file).
    /// </summary>
    /// <param name="catalog"></param>
    public void AddToCatalog(PdfDictionary catalog)
    {
        if (catalog == null)
        {
            throw new ArgumentNullException(nameof(catalog));
        }

        // Page Layout
        catalog.Remove(PdfName.Pagelayout);
        if ((_pageLayoutAndMode & PdfWriter.PageLayoutSinglePage) != 0)
        {
            catalog.Put(PdfName.Pagelayout, PdfName.Singlepage);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageLayoutOneColumn) != 0)
        {
            catalog.Put(PdfName.Pagelayout, PdfName.Onecolumn);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageLayoutTwoColumnLeft) != 0)
        {
            catalog.Put(PdfName.Pagelayout, PdfName.Twocolumnleft);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageLayoutTwoColumnRight) != 0)
        {
            catalog.Put(PdfName.Pagelayout, PdfName.Twocolumnright);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageLayoutTwoPageLeft) != 0)
        {
            catalog.Put(PdfName.Pagelayout, PdfName.Twopageleft);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageLayoutTwoPageRight) != 0)
        {
            catalog.Put(PdfName.Pagelayout, PdfName.Twopageright);
        }

        // Page Mode
        catalog.Remove(PdfName.Pagemode);
        if ((_pageLayoutAndMode & PdfWriter.PageModeUseNone) != 0)
        {
            catalog.Put(PdfName.Pagemode, PdfName.Usenone);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageModeUseOutlines) != 0)
        {
            catalog.Put(PdfName.Pagemode, PdfName.Useoutlines);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageModeUseThumbs) != 0)
        {
            catalog.Put(PdfName.Pagemode, PdfName.Usethumbs);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageModeFullScreen) != 0)
        {
            catalog.Put(PdfName.Pagemode, PdfName.Fullscreen);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageModeUseOC) != 0)
        {
            catalog.Put(PdfName.Pagemode, PdfName.Useoc);
        }
        else if ((_pageLayoutAndMode & PdfWriter.PageModeUseAttachments) != 0)
        {
            catalog.Put(PdfName.Pagemode, PdfName.Useattachments);
        }

        // viewer preferences (Table 8.1 of the PDF Reference)
        catalog.Remove(PdfName.Viewerpreferences);
        if (_viewerPreferences.Size > 0)
        {
            catalog.Put(PdfName.Viewerpreferences, _viewerPreferences);
        }
    }

    /// <summary>
    ///     Returns the viewer preferences.
    /// </summary>
    public PdfDictionary GetViewerPreferences() => _viewerPreferences;

    /// <summary>
    ///     Given a key for a viewer preference (a PdfName object),
    ///     this method returns the index in the VIEWER_PREFERENCES array.
    /// </summary>
    /// <param name="key">a PdfName referring to a viewer preference</param>
    /// <returns>an index in the VIEWER_PREFERENCES array</returns>
    private static int getIndex(PdfName key)
    {
        for (var i = 0; i < VIEWER_PREFERENCES.Length; i++)
        {
            if (VIEWER_PREFERENCES[i].Equals(key))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    ///     Checks if some value is valid for a certain key.
    /// </summary>
    private static bool isPossibleValue(PdfName value, PdfName[] accepted)
    {
        for (var i = 0; i < accepted.Length; i++)
        {
            if (accepted[i].Equals(value))
            {
                return true;
            }
        }

        return false;
    }
}