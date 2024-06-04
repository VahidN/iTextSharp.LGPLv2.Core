namespace iTextSharp.text.rtf.text;

/// <summary>
///     The RtfTabGroup is a convenience class if the same tabs are to be added
///     to multiple paragraphs.
///     RtfTabGroup tabs = new RtfTabGroup();
///     tabs.Add(new RtfTab(70, RtfTab.TAB_LEFT_ALIGN));
///     tabs.Add(new RtfTab(160, RtfTab.TAB_CENTER_ALIGN));
///     tabs.Add(new RtfTab(250, RtfTab.TAB_DECIMAL_ALIGN));
///     tabs.Add(new RtfTab(500, RtfTab.TAB_RIGHT_ALIGN));
///     Paragraph para = new Paragraph();
///     para.Add(tabs);
///     para.Add("\tLeft aligned\tCentre aligned\t12,45\tRight aligned");
///     @version $Revision: 1.5 $
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfTabGroup : RtfAddableElement
{
    /// <summary>
    ///     The tabs to add.
    /// </summary>
    private readonly List<RtfTab> _tabs;

    /// <summary>
    ///     Constructs an empty RtfTabGroup.
    /// </summary>
    public RtfTabGroup() => _tabs = new List<RtfTab>();

    /// <summary>
    ///     Constructs a RtfTabGroup with a set of tabs.
    /// </summary>
    /// <param name="tabs">An ArrayList with the RtfTabs to group in this RtfTabGroup.</param>
    public RtfTabGroup(IList<RtfTab> tabs)
    {
        if (tabs == null)
        {
            throw new ArgumentNullException(nameof(tabs));
        }

        _tabs = new List<RtfTab>();
        for (var i = 0; i < tabs.Count; i++)
        {
            if (tabs[i] is RtfTab)
            {
                _tabs.Add(tabs[i]);
            }
        }
    }

    /// <summary>
    ///     Adds a RtfTab to the list of grouped tabs.
    /// </summary>
    /// <param name="tab">The RtfTab to add.</param>
    public void Add(RtfTab tab)
    {
        _tabs.Add(tab);
    }

    /// <summary>
    ///     Combines the tab output form all grouped tabs.
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        foreach (var rt in _tabs)
        {
            rt.WriteContent(outp);
        }
    }
}