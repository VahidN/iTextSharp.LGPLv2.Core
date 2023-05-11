using iTextSharp.text.rtf.document;

namespace iTextSharp.text.rtf.list;

/// <summary>
///     The RtfPictureList2 manages the pictures for lists.
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.1.3
/// </summary>
public class RtfPictureList : RtfElement, IRtfExtendedElement
{
    /// <summary>
    ///     Constant for determining which picture bullet from the \listpicture destination that should be applied.
    /// </summary>
    private static readonly byte[] _listLevelPicture = DocWriter.GetIsoBytes("\\*\\listpicture");

    public RtfPictureList(RtfDocument doc) : base(doc)
    {
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.RtfElement#writeContent(java.io.OutputStream)
    /// </summary>
    public override void WriteContent(Stream outp)
    {
        // TODO Auto-generated method stub
    }

    /// <summary>
    ///     (non-Javadoc)
    ///     @see com.lowagie.text.rtf.RtfExtendedElement#writeDefinition(java.io.OutputStream)
    /// </summary>
    public void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        // TODO Auto-generated method stub
        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_listLevelPicture, 0, _listLevelPicture.Length);
        // if there are elements, write the \shppictlist here
        outp.Write(CloseGroup, 0, CloseGroup.Length);
    }
}