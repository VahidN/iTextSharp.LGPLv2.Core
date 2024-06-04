using iTextSharp.text.rtf.document.output;
using iTextSharp.text.rtf.style;

namespace iTextSharp.text.rtf.document;

/// <summary>
///     The RtfDocumentSettings contains output specific settings. These settings modify
///     how the actual document is then generated and some settings may mean that some
///     RTF readers can't read the document or render it wrongly.
///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
///     @author Thomas Bickel (tmb99@inode.at)
/// </summary>
public class RtfDocumentSettings
{
    /// <summary>
    ///     The RtfDocument this RtfDocumentSettings belongs to.
    /// </summary>
    private readonly RtfDocument _document;

    /// <summary>
    ///     Whether to always generate soft linebreaks for \n in Chunks.
    /// </summary>
    private bool _alwaysGenerateSoftLinebreaks;

    /// <summary>
    ///     Whether to always translate characters past 'z' into unicode representations.
    /// </summary>
    private bool _alwaysUseUnicode = true;

    /// <summary>
    ///     How to cache the document during generation. Defaults to RtfDataCache.CACHE_MEMORY;
    /// </summary>
    private int _dataCacheStyle = RtfDataCache.CACHE_MEMORY;

    /// <summary>
    ///     Whether images should be written in order to mimick the PDF output.
    /// </summary>
    private bool _imagePdfConformance = true;

    /// <summary>
    ///     Images are written as binary data and not hex encoded.
    ///     @since 2.1.1
    ///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
    /// </summary>
    private bool _imageWrittenAsBinary = true;

    /// <summary>
    ///     Whether to output the line breaks that make the rtf document source more readable.
    /// </summary>
    private bool _outputDebugLineBreaks;

    /// <summary>
    ///     Whether to also output the table row definition after the cell content.
    /// </summary>
    private bool _outputTableRowDefinitionAfter = true;

    /// <summary>
    ///     Document protection level password hash.
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private string _protectionHash;

    /// <summary>
    ///     Document protection level
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private int _protectionLevel = RtfProtection.LEVEL_NONE;

    /// <summary>
    ///     Document read password hash
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <summary>
    ///     private String writereservhash = null; //\*\writereservhash - not implemented
    /// </summary>
    /// <summary>
    ///     Document recommended to be opened in read only mode.
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    private bool _readOnlyRecommended;

    /// <summary>
    ///     Whether to write image scaling information. This is required for Word 2000, 97 and Word for Mac
    /// </summary>
    private bool _writeImageScalingInformation;

    /// <summary>
    ///     Constructs a new RtfDocumentSettings object.
    /// </summary>
    /// <param name="document">The RtfDocument this RtfDocumentSettings belong to.</param>
    public RtfDocumentSettings(RtfDocument document) => _document = document;

    /// <summary>
    ///     Gets the current data cache style.
    /// </summary>
    /// <returns>The current data cache style.</returns>
    public int GetDataCacheStyle() => _dataCacheStyle;

    /// <summary>
    ///     Obtain the password has as a byte array.
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <returns>The bytes of the password hash as a byte array (byte[])</returns>
    public byte[] GetProtectionHashBytes() => DocWriter.GetIsoBytes(_protectionHash);

    /// <summary>
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <returns>RTF document protection level</returns>
    public int GetProtectionLevel() => convertProtectionLevel();

    /// <summary>
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <returns>RTF document protection level as a byte array (byte[])</returns>
    public byte[] GetProtectionLevelBytes() =>
        DocWriter.GetIsoBytes(convertProtectionLevel().ToString(CultureInfo.InvariantCulture));

    /// <summary>
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <returns>RTF document protection level</returns>
    public int GetProtectionLevelRaw() => _protectionLevel;

    /// <summary>
    ///     Get the RTF flag that recommends if the the document should be opened in read only mode.
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <returns>true if flag is set, false if it is not set</returns>
    public bool GetReadOnlyRecommended() => _readOnlyRecommended;

    /// <summary>
    ///     Gets whether all linebreaks inside Chunks are generated as soft linebreaks.
    /// </summary>
    /// <returns> True  if soft linebreaks are generated,  false  for hard linebreaks.</returns>
    public bool IsAlwaysGenerateSoftLinebreaks() => _alwaysGenerateSoftLinebreaks;

    /// <summary>
    ///     Gets whether all characters bigger than 'z' are represented as unicode.
    /// </summary>
    /// <returns> True  if unicode representation is used,  false  otherwise.</returns>
    public bool IsAlwaysUseUnicode() => _alwaysUseUnicode;

    /// <summary>
    ///     Determine if document has protection enabled.
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <returns>true if protection is enabled, false if it is not enabled</returns>
    public bool IsDocumentProtected() => !(_protectionHash == null);

    /// <summary>
    ///     Gets the current setting on image PDF conformance.
    /// </summary>
    /// <returns>The current image PDF conformance.</returns>
    public bool IsImagePdfConformance() => _imagePdfConformance;

    /// <summary>
    ///     Gets whether images are written as binary data or are hex encoded. Defaults to  true .
    ///     @since 2.1.1
    ///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
    /// </summary>
    /// <returns> True  if images are written as binary data,  false  if hex encoded.</returns>
    public bool IsImageWrittenAsBinary() => _imageWrittenAsBinary;

    /// <summary>
    ///     Gets whether to output the line breaks for increased rtf document readability.
    /// </summary>
    /// <returns>Whether to output line breaks.</returns>
    public bool IsOutputDebugLineBreaks() => _outputDebugLineBreaks;

    /// <summary>
    ///     Gets whether the table row definition should also be written after the cell content.
    /// </summary>
    /// <returns>Returns the outputTableRowDefinitionAfter.</returns>
    public bool IsOutputTableRowDefinitionAfter() => _outputTableRowDefinitionAfter;

    /// <summary>
    ///     Gets whether to write scaling information for images.
    /// </summary>
    /// <returns>Whether to write scaling information for images.</returns>
    public bool IsWriteImageScalingInformation() => _writeImageScalingInformation;

    /// <summary>
    ///     Registers the RtfParagraphStyle for further use in the document. This does not need to be
    ///     done for the default styles in the RtfParagraphStyle object. Those are added automatically.
    /// </summary>
    /// <param name="rtfParagraphStyle">The RtfParagraphStyle to register.</param>
    public void RegisterParagraphStyle(RtfParagraphStyle rtfParagraphStyle)
    {
        _document.GetDocumentHeader().RegisterParagraphStyle(rtfParagraphStyle);
    }

    /// <summary>
    ///     Sets whether to always generate soft linebreaks.
    /// </summary>
    /// <param name="alwaysGenerateSoftLinebreaks">Whether to always generate soft linebreaks.</param>
    public void SetAlwaysGenerateSoftLinebreaks(bool alwaysGenerateSoftLinebreaks)
    {
        _alwaysGenerateSoftLinebreaks = alwaysGenerateSoftLinebreaks;
    }

    /// <summary>
    ///     Sets whether to represent all characters bigger than 'z' as unicode.
    /// </summary>
    /// <param name="alwaysUseUnicode"> True  to use unicode representation,  false  otherwise.</param>
    public void SetAlwaysUseUnicode(bool alwaysUseUnicode)
    {
        _alwaysUseUnicode = alwaysUseUnicode;
    }

    /// <summary>
    ///     Sets the data cache style. This controls where the document is cached during
    ///     generation. Two cache styles are supported:
    ///     RtfDataCache.CACHE_MEMORY: The document is cached in memory. This is fast,
    ///     but places a limit on how big the document can get before causing
    ///     OutOfMemoryExceptions.
    ///     RtfDataCache.CACHE_DISK: The document is cached on disk. This is slower
    ///     than the CACHE_MEMORY setting, but the document size is now only constrained
    ///     by the amount of free disk space.
    ///     in RtfDataCache.
    ///     @see com.lowagie.text.rtf.document.output.output.RtfDataCache.
    /// </summary>
    /// <param name="dataCacheStyle">The data cache style to set. Valid constants can be found</param>
    public void SetDataCacheStyle(int dataCacheStyle)
    {
        switch (dataCacheStyle)
        {
            case RtfDataCache.CACHE_MEMORY_EFFICIENT:
                _dataCacheStyle = RtfDataCache.CACHE_MEMORY_EFFICIENT;
                break;
            case RtfDataCache.CACHE_DISK:
                _dataCacheStyle = RtfDataCache.CACHE_DISK;
                break;
            default:
                //case RtfDataCache.CACHE_MEMORY:
                _dataCacheStyle = RtfDataCache.CACHE_MEMORY;
                break;
        }
    }

    /// <summary>
    ///     Sets the image PDF conformance setting. By default images will be added
    ///     as if they were displayed with 72dpi. Set this to  false
    ///     if images should be generated with the Word default DPI setting.
    ///     for the default Word display.
    /// </summary>
    /// <param name="imagePdfConformance"> True  if PDF equivalence is desired,  false </param>
    public void SetImagePdfConformance(bool imagePdfConformance)
    {
        _imagePdfConformance = imagePdfConformance;
    }

    /// <summary>
    ///     Set whether images are written as binary data or are hex encoded.
    ///     @since 2.1.1
    ///     @author Mark Hall (Mark.Hall@mail.room3b.eu)
    /// </summary>
    /// <param name="imageWrittenAsBinary"> True  to write images as binary data,  false  for hex encoding.</param>
    public void SetImageWrittenAsBinary(bool imageWrittenAsBinary)
    {
        _imageWrittenAsBinary = imageWrittenAsBinary;
    }

    /// <summary>
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <param name="oldPwd">Old password - clear text</param>
    /// <param name="newPwd">New password - clear text</param>
    /// <returns>true if password set, false if password not set</returns>
    public bool SetNewPassword(string oldPwd, string newPwd)
    {
        var result = false;
        if (_protectionHash.Equals(RtfProtection.GenerateHash(oldPwd), StringComparison.Ordinal))
        {
            _protectionHash = RtfProtection.GenerateHash(newPwd);
            result = true;
        }

        return result;
    }

    /// <summary>
    ///     Set the options required for RTF documents to display correctly in MS Word 2000
    ///     and MS Word 97.
    ///     Sets  outputTableRowDefinitionAfter = true  and  writeImageScalingInformation = true .
    /// </summary>
    public void SetOptionsForMsWord2000And97()
    {
        SetOutputTableRowDefinitionAfter(true);
        SetWriteImageScalingInformation(true);
    }

    /// <summary>
    ///     Set the options required for RTF documents to display correctly in MS Word for Mac.
    ///     Sets  writeImageScalingInformation = true .
    /// </summary>
    public void SetOptionsForMsWordForMac()
    {
        SetWriteImageScalingInformation(true);
    }

    /// <summary>
    ///     Set the options required for RTF documents to display correctly in MS Word XP (2002).
    ///     Sets  writeImageScalingInformation = false .
    /// </summary>
    public void SetOptionsForMsWordXp()
    {
        SetWriteImageScalingInformation(false);
    }

    /// <summary>
    ///     Set the options required for RTF documents to display correctly in OpenOffice.Org
    ///     Writer.
    ///     Sets  outputTableRowDefinitionAfter = false .
    /// </summary>
    public void SetOptionsForOpenOfficeOrg()
    {
        SetOutputTableRowDefinitionAfter(false);
    }

    /// <summary>
    ///     Sets whether to output the line breaks for increased rtf document readability.
    ///     Some line breaks may be added where the rtf specification demands it.
    /// </summary>
    /// <param name="outputDebugLineBreaks">The outputDebugLineBreaks to set.</param>
    public void SetOutputDebugLineBreaks(bool outputDebugLineBreaks)
    {
        _outputDebugLineBreaks = outputDebugLineBreaks;
    }

    /// <summary>
    ///     Sets whether the table row definition should also be written after the cell content.
    ///     This is recommended to be set to  true  if you need Word2000 compatiblity and
    ///     false  if the document should be opened in OpenOffice.org Writer.
    /// </summary>
    /// <param name="outputTableRowDefinitionAfter">The outputTableRowDefinitionAfter to set.</param>
    public void SetOutputTableRowDefinitionAfter(
        bool outputTableRowDefinitionAfter)
    {
        _outputTableRowDefinitionAfter = outputTableRowDefinitionAfter;
    }

    /// <summary>
    ///     This function is not intended for general use. Please see 'public bool SetProtection(int level, String pwd)'
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <param name="pwd">Password HASH to set the document password hash to.</param>
    public void SetPasswordHash(string pwd)
    {
        if (pwd != null && pwd.Length != 8)
        {
            return;
        }

        _protectionHash = pwd;
    }

    /// <summary>
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <param name="level">Document protecton level</param>
    /// <param name="pwd">Document password - clear text</param>
    public bool SetProtection(int level, string pwd)
    {
        var result = false;
        if (_protectionHash == null)
        {
            if (!SetProtectionLevel(level))
            {
                result = false;
            }
            else
            {
                _protectionHash = RtfProtection.GenerateHash(pwd);
                result = true;
            }
        }
        else
        {
            if (_protectionHash.Equals(RtfProtection.GenerateHash(pwd), StringComparison.Ordinal))
            {
                if (!SetProtectionLevel(level))
                {
                    result = false;
                }
                else
                {
                    _protectionHash = RtfProtection.GenerateHash(pwd);
                    result = true;
                }
            }
        }

        return result;
    }

    /// <summary>
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <param name="level">Document protection level</param>
    public bool SetProtectionLevel(int level)
    {
        var result = false;
        switch (level)
        {
            case RtfProtection.LEVEL_NONE:
                if (_protectionHash == null)
                {
                    break;
                }

                goto case RtfProtection.LEVEL_ANNOTPROT;
            case RtfProtection.LEVEL_ANNOTPROT:
            case RtfProtection.LEVEL_FORMPROT:
            case RtfProtection.LEVEL_REVPROT:
            case RtfProtection.LEVEL_READPROT:
                _protectionLevel = level;
                result = true;
                break;
        }

        return result;
    }

    /// <summary>
    ///     Set the RTF flag that recommends the document be opened in read only mode.
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <param name="value">true if the flag is to be set, false if it is NOT to be set</param>
    public void SetReadOnlyRecommended(bool value)
    {
        _readOnlyRecommended = value;
    }

    /// <summary>
    ///     Sets whether image scaling information should be written. This needs to be set to  true
    ///     MS Word 2000, MS Word 97 and Word for Mac.
    /// </summary>
    /// <param name="writeImageScalingInformation">Whether to write image scaling information.</param>
    public void SetWriteImageScalingInformation(bool writeImageScalingInformation)
    {
        _writeImageScalingInformation = writeImageScalingInformation;
    }

    /// <summary>
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <param name="pwd">Document password - clear text</param>
    /// <returns>true if document unprotected, false if protection is not removed.</returns>
    public bool UnprotectDocument(string pwd)
    {
        var result = false;
        if (_protectionHash.Equals(RtfProtection.GenerateHash(pwd), StringComparison.Ordinal))
        {
            _protectionLevel = RtfProtection.LEVEL_NONE;
            _protectionHash = null;
            result = true;
        }

        return result;
    }

    /// <summary>
    ///     Converts protection level from internal bitmap value to protlevel output value
    ///     0 = Revision protection
    ///     1 = Annotation/Comment protection
    ///     2 = Form protection
    ///     3 = Read only protection
    ///     @since 2.1.1
    ///     @author Howard Shank (hgshank@yahoo.com)
    /// </summary>
    /// <returns> </returns>
    private int convertProtectionLevel()
    {
        var level = 0;
        switch (_protectionLevel)
        {
            case RtfProtection.LEVEL_NONE:
                break;
            case RtfProtection.LEVEL_REVPROT:
                level = 0;
                break;
            case RtfProtection.LEVEL_ANNOTPROT:
                level = 1;
                break;
            case RtfProtection.LEVEL_FORMPROT:
                level = 2;
                break;
            case RtfProtection.LEVEL_READPROT:
                level = 3;
                break;
        }

        return level;
    }
}