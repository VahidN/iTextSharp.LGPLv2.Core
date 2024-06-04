namespace iTextSharp.text.rtf.document;

/// <summary>
///     The RtfProtectionSetting handles document protection elements
///     @version $Id: RtfProtectionSetting.cs,v 1.2 2008/05/13 11:25:50 psoares33 Exp $
///     @author Howard Shank (hgshank@yahoo.com)
///     @since 2.1.1
/// </summary>
public class RtfProtectionSetting : RtfElement
{
    /// <summary>
    ///     Constant for Annotation/Comment protection controlword
    ///     Mutually exclusive
    ///     @see
    ///     com.lowagie.text.rtf.document.RtfProtectionSetting#FORMPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @see com.lowagie.text.rtf.document.RtfProtectionSetting#REVPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @see
    ///     com.lowagie.text.rtf.document.RtfProtectionSetting#READPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private static readonly byte[] _annotprot = DocWriter.GetIsoBytes("\\annotprot");

    /// <summary>
    ///     Constant for enforceprot controlword
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private static readonly byte[] _enforceprot = DocWriter.GetIsoBytes("\\enforceprot");

    /// <summary>
    ///     Constant for Form protection controlword
    ///     Mutually exclusive
    ///     @see com.lowagie.text.rtf.document.RtfProtectionSetting#REVPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @see
    ///     com.lowagie.text.rtf.document.RtfProtectionSetting#ANNOTPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @see
    ///     com.lowagie.text.rtf.document.RtfProtectionSetting#READPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private static readonly byte[] _formprot = DocWriter.GetIsoBytes("\\formprot");

    /// <summary>
    ///     Constant for protlevel controlword
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private static readonly byte[] _protlevel = DocWriter.GetIsoBytes("\\protlevel");

    /// <summary>
    ///     Constant for enforceprot controlword.
    ///     Implemented in Microsoft Word 2007.
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private static readonly byte[] _readonlyrecommended = DocWriter.GetIsoBytes("\\readonlyrecommended");

    /// <summary>
    ///     Constant for read only rotection controlword
    ///     Mutually exclusive - exception, can be combined with ANNOTPROT
    ///     for backwards compatibility
    ///     @see
    ///     com.lowagie.text.rtf.document.RtfProtectionSetting#FORMPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @see com.lowagie.text.rtf.document.RtfProtectionSetting#REVPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @see
    ///     com.lowagie.text.rtf.document.RtfProtectionSetting#ANNOTPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private static readonly byte[] _readprot = DocWriter.GetIsoBytes("\\readprot");

    /// <summary>
    ///     Constant for Revision protection controlword
    ///     Mutually exclusive
    ///     @see
    ///     com.lowagie.text.rtf.document.RtfProtectionSetting#FORMPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @see
    ///     com.lowagie.text.rtf.document.RtfProtectionSetting#ANNOTPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @see
    ///     com.lowagie.text.rtf.document.RtfProtectionSetting#READPROT(com.lowagie.text.rtf.document.RtfProtectionSetting)
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private static readonly byte[] _revprot = DocWriter.GetIsoBytes("\\revprot");

    /// <summary>
    ///     Constructs a  RtfProtectionSetting  belonging to a RtfDocument
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <param name="doc">The  RtfDocument  this  RtfProtectionSetting  belongs to</param>
    public RtfProtectionSetting(RtfDocument doc) : base(doc)
    {
    }

    /// <summary>
    ///     Writes the RTF protection control words
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    /// <summary>
    ///     Writes the RTF protection control words
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    public virtual void WriteDefinition(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (Document.GetDocumentSettings().IsDocumentProtected())
        {
            switch (Document.GetDocumentSettings().GetProtectionLevelRaw())
            {
                case RtfProtection.LEVEL_FORMPROT:
                    result.Write(_formprot, 0, _formprot.Length);
                    break;
                case RtfProtection.LEVEL_ANNOTPROT:
                    result.Write(_annotprot, 0, _annotprot.Length);
                    break;
                case RtfProtection.LEVEL_REVPROT:
                    result.Write(_revprot, 0, _revprot.Length);
                    break;
                case RtfProtection.LEVEL_READPROT:
                    result.Write(_annotprot, 0, _annotprot.Length);
                    result.Write(_readprot, 0, _readprot.Length);
                    break;
            }

            result.Write(_enforceprot, 0,
                         _enforceprot.Length); // assumes one of the above protection keywords was output.
            result.WriteByte((byte)'1');
            result.Write(_protlevel, 0, _protlevel.Length);
            byte[] t;
            result.Write(t = Document.GetDocumentSettings().GetProtectionLevelBytes(), 0, t.Length);
        }

        if (Document.GetDocumentSettings().GetReadOnlyRecommended())
        {
            result.Write(_readonlyrecommended, 0, _readonlyrecommended.Length);
            result.Write(Delimiter, 0, Delimiter.Length);
        }
    }
}