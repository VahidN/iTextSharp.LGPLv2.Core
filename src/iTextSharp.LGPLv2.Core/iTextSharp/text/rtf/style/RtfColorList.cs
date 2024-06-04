using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.style;

/// <summary>
///     The RtfColorList stores all colours that appear in the document. Black
///     and White are always added
///     @version $Version:$
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
/// </summary>
public class RtfColorList : RtfElement, IRtfExtendedElement
{
    /// <summary>
    ///     Constant for the beginning of the colour table
    /// </summary>
    private static readonly byte[] _colorTable = DocWriter.GetIsoBytes("\\colortbl");

    /// <summary>
    ///     ArrayList containing all colours of this RtfColorList
    /// </summary>
    private readonly List<RtfColor> _colorList = new();

    /// <summary>
    ///     Constructs a new RtfColorList for the RtfDocument. Will add the default
    ///     black and white colours.
    /// </summary>
    /// <param name="doc">The RtfDocument this RtfColorList belongs to</param>
    public RtfColorList(RtfDocument doc) : base(doc)
    {
        _colorList.Add(new RtfColor(doc, 0, 0, 0, 0));
        _colorList.Add(new RtfColor(doc, 255, 255, 255, 1));
    }

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Write the definition part of the colour list. Calls the writeDefinition
    ///     methods of the RtfColors in the colour list.
    /// </summary>
    public virtual void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_colorTable, 0, _colorTable.Length);
        for (var i = 0; i < _colorList.Count; i++)
        {
            var color = _colorList[i];
            color.WriteDefinition(outp);
        }

        outp.Write(CloseGroup, 0, CloseGroup.Length);
        Document.OutputDebugLinebreak(outp);
    }

    /// <summary>
    ///     Returns the index of the given RtfColor in the colour list. If the RtfColor
    ///     is not in the list of colours, then it is added.
    /// </summary>
    /// <param name="color">The RtfColor for which to get the index</param>
    /// <returns>The index of the RtfColor</returns>
    public int GetColorNumber(RtfColor color)
    {
        var colorIndex = -1;
        for (var i = 0; i < _colorList.Count; i++)
        {
            if (_colorList[i].Equals(color))
            {
                colorIndex = i;
            }
        }

        if (colorIndex == -1)
        {
            colorIndex = _colorList.Count;
            _colorList.Add(color);
        }

        return colorIndex;
    }
}