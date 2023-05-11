using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.style;
using iTextSharp.text.rtf.text;

namespace iTextSharp.text.rtf.list;

/// <summary>
///     The RtfListLevel is a listlevel object in a list.
///     @author Howard Shank (hgshank@yahoo.com)
/// </summary>
public class RtfListLevel : RtfElement, IRtfExtendedElement
{
    public const int LIST_LEVEL_FOLLOW_NOTHING = 2;

    public const int LIST_LEVEL_FOLLOW_SPACE = 1;

    public const int LIST_LEVEL_FOLLOW_TAB = 0;

    public const int LIST_TYPE_ARABIC = 1000;

    public const int LIST_TYPE_ARABIC_LEADING_ZERO = 1022;

    public const int LIST_TYPE_BASE = 1000;

    public const int LIST_TYPE_BULLET = 1023;

    public const int LIST_TYPE_CARDINAL_TEXT_NUMBER = 1006;

    public const int LIST_TYPE_LOWER_LETTERS = 3;

    public const int LIST_TYPE_LOWER_ROMAN = 5;

    public const int LIST_TYPE_LOWERCASE_LETTER = 1004;

    public const int LIST_TYPE_LOWERCASE_ROMAN_NUMERAL = 1002;

    public const int LIST_TYPE_NO_NUMBER = 1255;

    public const int LIST_TYPE_NUMBERED = 1;

    public const int LIST_TYPE_ORDINAL_NUMBER = 1005;

    public const int LIST_TYPE_ORDINAL_TEXT_NUMBER = 1007;

    public const int LIST_TYPE_UNKNOWN = -1;

    public const int LIST_TYPE_UPPER_LETTERS = 2;

    public const int LIST_TYPE_UPPER_ROMAN = 4;

    public const int LIST_TYPE_UPPERCASE_LETTER = 1003;

    public const int LIST_TYPE_UPPERCASE_ROMAN_NUMERAL = 1001;

    /// <summary>
    ///     Constant for list level
    /// </summary>
    private static readonly byte[] _listLevel = DocWriter.GetIsoBytes("\\listlevel");

    /// <summary>
    ///     Constant for list level alignment old
    /// </summary>
    private static readonly byte[] _listLevelAlignment = DocWriter.GetIsoBytes("\\leveljc");

    /// <summary>
    ///     Constant for list level alignment new
    /// </summary>
    private static readonly byte[] _listLevelAlignmentNew = DocWriter.GetIsoBytes("\\leveljcn");

    /// <summary>
    ///     Constant for the first indentation
    /// </summary>
    private static readonly byte[] _listLevelFirstIndent = DocWriter.GetIsoBytes("\\fi");

    /// <summary>
    ///     Constant which specifies which character follows the level text
    /// </summary>
    private static readonly byte[] _listLevelFolow = DocWriter.GetIsoBytes("\\levelfollow");

    /// <summary>
    ///     Constant which specifies the levelindent control word
    /// </summary>
    private static readonly byte[] _listLevelIndent = DocWriter.GetIsoBytes("\\levelindent");

    /// <summary>
    ///     Constant which specifies (1) if list numbers from previous levels should be converted
    ///     to Arabic numbers; (0) if they should be left with the format specified by their
    ///     own level's definition.
    /// </summary>
    private static readonly byte[] _listLevelLegal = DocWriter.GetIsoBytes("\\levellegal");

    /// <summary>
    ///     Constant which specifies
    ///     (1) if this level does/does not restart its count each time a super ordinate level is incremented
    ///     (0) if this level does not restart its count each time a super ordinate level is incremented.
    /// </summary>
    private static readonly byte[] _listLevelNoRestart = DocWriter.GetIsoBytes("\\levelnorestart");

    /// <summary>
    ///     Constant for the beginning of the list level numbers
    /// </summary>
    private static readonly byte[] _listLevelNumbersBegin = DocWriter.GetIsoBytes("\\levelnumbers");

    /// <summary>
    ///     Constant for the end of the list level numbers
    /// </summary>
    private static readonly byte[] _listLevelNumbersEnd = DocWriter.GetIsoBytes(";");

    /// <summary>
    ///     Constant for the list level numbers
    /// </summary>
    private static readonly byte[] _listLevelNumbersNumbered = DocWriter.GetIsoBytes("\\\'01");

    /// <summary>
    ///     Constant for the levelpictureN control word
    /// </summary>
    private static readonly byte[] _listLevelPicture = DocWriter.GetIsoBytes("\\levelpicture");

    /// <summary>
    ///     Constant which specifies the levelspace controlword
    /// </summary>
    private static readonly byte[] _listLevelSpace = DocWriter.GetIsoBytes("\\levelspace");

    /// <summary>
    ///     Constant for list level start at
    /// </summary>
    private static readonly byte[] _listLevelStartAt = DocWriter.GetIsoBytes("\\levelstartat");

    /// <summary>
    ///     Constant for the beginning of the list level bulleted style
    /// </summary>
    private static readonly byte[] _listLevelStyleBulletedBegin = DocWriter.GetIsoBytes("\\\'01");

    /// <summary>
    ///     Constant for the end of the list level bulleted style
    /// </summary>
    private static readonly byte[] _listLevelStyleBulletedEnd = DocWriter.GetIsoBytes(";");

    /// <summary>
    ///     Constant for the beginning of the list level numbered style
    /// </summary>
    private static readonly byte[] _listLevelStyleNumberedBegin = DocWriter.GetIsoBytes("\\\'02\\\'");

    /// <summary>
    ///     Constant for the end of the list level numbered style
    /// </summary>
    private static readonly byte[] _listLevelStyleNumberedEnd = DocWriter.GetIsoBytes(".;");

    /// <summary>
    ///     Constant for the symbol indentation
    /// </summary>
    private static readonly byte[] _listLevelSymbolIndent = DocWriter.GetIsoBytes("\\tx");

    /// <summary>
    ///     Constant for list level
    /// </summary>
    private static readonly byte[] _listLevelTemplateId = DocWriter.GetIsoBytes("\\leveltemplateid");

    /// <summary>
    ///     Constant for the lvltentative control word
    /// </summary>
    private static readonly byte[] _listLevelTentative = DocWriter.GetIsoBytes("\\lvltentative");

    /// <summary>
    ///     Constant for list level text
    /// </summary>
    private static readonly byte[] _listLevelText = DocWriter.GetIsoBytes("\\leveltext");

    /// <summary>
    ///     Constant for list level style old
    /// </summary>
    private static readonly byte[] _listLevelType = DocWriter.GetIsoBytes("\\levelnfc");

    /// <summary>
    ///     Constant for list level style new
    /// </summary>
    private static readonly byte[] _listLevelTypeNew = DocWriter.GetIsoBytes("\\levelnfcn");

    /* unknown type */
    /* BASE value to subtract to get RTF Value if above base*/
    /* 0 Arabic (1, 2, 3) */
    /* 1 Uppercase Roman numeral (I, II, III) */
    /* 2 Lowercase Roman numeral (i, ii, iii)*/
    /* 3 Uppercase letter (A, B, C)*/
    /* 4 Lowercase letter (a, b, c)*/
    /* 5 Ordinal number (1st, 2nd, 3rd)*/
    /* 6 Cardinal text number (One, Two Three)*/
    /* 7 Ordinal text number (First, Second, Third)*/
    /* 22   Arabic with leading zero (01, 02, 03, ..., 10, 11)*/
    /* 23   Bullet (no number at all)*/
    /*  255 No number */
    /// <summary>
    ///     Which picture bullet from the \listpicture destination should be applied
    /// </summary>
    private readonly int _levelPicture = -1;

    private readonly int _templateId = -1;

    /// <summary>
    ///     The alignment of this RtfList
    /// </summary>
    private int _alignment = Element.ALIGN_LEFT;

    /// <summary>
    ///     The text to use as the bullet character
    /// </summary>
    private string _bulletCharacter = "\u00b7";

    /// <summary>
    ///     @since 2.1.4
    /// </summary>
    private Chunk _bulletChunk;

    /// <summary>
    ///     The first indentation of this RtfList
    /// </summary>
    private int _firstIndent;

    /// <summary>
    ///     The RtfFont for bulleted lists
    /// </summary>
    private RtfFont _fontBullet;

    /// <summary>
    ///     The RtfFont for numbered lists
    /// </summary>
    private RtfFont _fontNumber;

    /// <summary>
    ///     Flag to indicate if the levellegal control word should be emitted.
    ///     true  if any list numbers from previous levels should be converted to Arabic numbers;
    ///     false if they should be left with the format specified by their own level definition.
    /// </summary>
    private bool _isLegal;

    /// <summary>
    ///     Flag to indicate if the tentative control word should be emitted.
    /// </summary>
    private bool _isTentative = true;

    /// <summary>
    ///     The left indentation of this RtfList
    /// </summary>
    private int _leftIndent;

    private int _levelFollowValue = LIST_LEVEL_FOLLOW_TAB;

    private int _levelTextNumber;

    private RtfListLevel _listLevelParent;

    /// <summary>
    ///     Does the list restart numbering each time a super ordinate level is incremented
    /// </summary>
    private int _listNoRestart;

    /// <summary>
    ///     The number to start counting at
    /// </summary>
    private int _listStartAt = 1;

    /// <summary>
    ///     10  Kanji numbering without the digit character (*dbnum1)
    ///     11  Kanji numbering with the digit character (*dbnum2)
    ///     12  46 phonetic katakana characters in "aiueo" order (*aiueo)
    ///     13  46 phonetic katakana characters in "iroha" order (*iroha)
    ///     14  Double-byte character
    ///     15  Single-byte character
    ///     16  Kanji numbering 3 (*dbnum3)
    ///     17  Kanji numbering 4 (*dbnum4)
    ///     18  Circle numbering (*circlenum)
    ///     19  Double-byte Arabic numbering
    ///     20  46 phonetic double-byte katakana characters (*aiueo*dbchar)
    ///     21  46 phonetic double-byte katakana characters (*iroha*dbchar)
    ///     22  Arabic with leading zero (01, 02, 03, ..., 10, 11)
    ///     24  Korean numbering 2 (*ganada)
    ///     25  Korean numbering 1 (*chosung)
    ///     26  Chinese numbering 1 (*gb1)
    ///     27  Chinese numbering 2 (*gb2)
    ///     28  Chinese numbering 3 (*gb3)
    ///     29  Chinese numbering 4 (*gb4)
    ///     30  Chinese Zodiac numbering 1 (* zodiac1)
    ///     31  Chinese Zodiac numbering 2 (* zodiac2)
    ///     32  Chinese Zodiac numbering 3 (* zodiac3)
    ///     33  Taiwanese double-byte numbering 1
    ///     34  Taiwanese double-byte numbering 2
    ///     35  Taiwanese double-byte numbering 3
    ///     36  Taiwanese double-byte numbering 4
    ///     37  Chinese double-byte numbering 1
    ///     38  Chinese double-byte numbering 2
    ///     39  Chinese double-byte numbering 3
    ///     40  Chinese double-byte numbering 4
    ///     41  Korean double-byte numbering 1
    ///     42  Korean double-byte numbering 2
    ///     43  Korean double-byte numbering 3
    ///     44  Korean double-byte numbering 4
    ///     45  Hebrew non-standard decimal
    ///     46  Arabic Alif Ba Tah
    ///     47  Hebrew Biblical standard
    ///     48  Arabic Abjad style
    ///     255 No number
    /// </summary>
    /// <summary>
    ///     Whether this RtfList is numbered
    /// </summary>
    private int _listType = LIST_TYPE_UNKNOWN;

    /// <summary>
    ///     Parent list object
    /// </summary>
    private RtfList _parent;

    /// <summary>
    ///     The right indentation of this RtfList
    /// </summary>
    private int _rightIndent;

    /// <summary>
    ///     The symbol indentation of this RtfList
    /// </summary>
    private int _symbolIndent;

    /// <summary>
    ///     The level of this RtfListLevel
    /// </summary>
    private int listLevel;

    public RtfListLevel(RtfDocument doc) : base(doc)
    {
        _templateId = Document.GetRandomInt();
        SetFontNumber(new RtfFont(Document, new Font(Font.TIMES_ROMAN, 10, Font.NORMAL, new BaseColor(0, 0, 0))));
        SetBulletFont(new Font(Font.SYMBOL, 10, Font.NORMAL, new BaseColor(0, 0, 0)));
    }

    public RtfListLevel(RtfDocument doc, RtfList parent) : base(doc)
    {
        _parent = parent;
        _templateId = Document.GetRandomInt();
        SetFontNumber(new RtfFont(Document, new Font(Font.TIMES_ROMAN, 10, Font.NORMAL, new BaseColor(0, 0, 0))));
        SetBulletFont(new Font(Font.SYMBOL, 10, Font.NORMAL, new BaseColor(0, 0, 0)));
    }

    public RtfListLevel(RtfListLevel ll) : base(ll?.Document ?? throw new ArgumentNullException(nameof(ll)))
    {
        _templateId = Document.GetRandomInt();
        _alignment = ll._alignment;
        _bulletCharacter = ll._bulletCharacter;
        _firstIndent = ll._firstIndent;
        _fontBullet = ll._fontBullet;
        _fontNumber = ll._fontNumber;
        InHeader = ll.InHeader;
        InTable = ll.InTable;
        _leftIndent = ll._leftIndent;
        listLevel = ll.listLevel;
        _listNoRestart = ll._listNoRestart;
        _listStartAt = ll._listStartAt;
        _listType = ll._listType;
        _parent = ll._parent;
        _rightIndent = ll._rightIndent;
        _symbolIndent = ll._symbolIndent;
    }

    /// <summary>
    ///     unused
    /// </summary>
    public override void WriteContent(Stream outp)
    {
    }

    public void WriteDefinition(Stream outp)
    {
        if (outp == null)
        {
            throw new ArgumentNullException(nameof(outp));
        }

        byte[] t;
        outp.Write(OpenGroup, 0, OpenGroup.Length);
        outp.Write(_listLevel, 0, _listLevel.Length);
        outp.Write(_listLevelType, 0, _listLevelType.Length);
        switch (_listType)
        {
            case LIST_TYPE_BULLET:
                outp.Write(t = IntToByteArray(23), 0, t.Length);
                break;
            case LIST_TYPE_NUMBERED:
                outp.Write(t = IntToByteArray(0), 0, t.Length);
                break;
            case LIST_TYPE_UPPER_LETTERS:
                outp.Write(t = IntToByteArray(3), 0, t.Length);
                break;
            case LIST_TYPE_LOWER_LETTERS:
                outp.Write(t = IntToByteArray(4), 0, t.Length);
                break;
            case LIST_TYPE_UPPER_ROMAN:
                outp.Write(t = IntToByteArray(1), 0, t.Length);
                break;
            case LIST_TYPE_LOWER_ROMAN:
                outp.Write(t = IntToByteArray(2), 0, t.Length);
                break;
            /* New types */
            case LIST_TYPE_ARABIC:
                outp.Write(t = IntToByteArray(0), 0, t.Length);
                break;
            case LIST_TYPE_UPPERCASE_ROMAN_NUMERAL:
                outp.Write(t = IntToByteArray(1), 0, t.Length);
                break;
            case LIST_TYPE_LOWERCASE_ROMAN_NUMERAL:
                outp.Write(t = IntToByteArray(2), 0, t.Length);
                break;
            case LIST_TYPE_UPPERCASE_LETTER:
                outp.Write(t = IntToByteArray(3), 0, t.Length);
                break;
            case LIST_TYPE_ORDINAL_NUMBER:
                outp.Write(t = IntToByteArray(4), 0, t.Length);
                break;
            case LIST_TYPE_CARDINAL_TEXT_NUMBER:
                outp.Write(t = IntToByteArray(5), 0, t.Length);
                break;
            case LIST_TYPE_ORDINAL_TEXT_NUMBER:
                outp.Write(t = IntToByteArray(6), 0, t.Length);
                break;
            case LIST_TYPE_LOWERCASE_LETTER:
                outp.Write(t = IntToByteArray(7), 0, t.Length);
                break;
            case LIST_TYPE_ARABIC_LEADING_ZERO:
                outp.Write(t = IntToByteArray(22), 0, t.Length);
                break;
            case LIST_TYPE_NO_NUMBER:
                outp.Write(t = IntToByteArray(255), 0, t.Length);
                break;
            default: // catch all for other unsupported types
                if (_listType >= LIST_TYPE_BASE)
                {
                    outp.Write(t = IntToByteArray(_listType - LIST_TYPE_BASE), 0, t.Length);
                }

                break;
        }

        outp.Write(_listLevelTypeNew, 0, _listLevelTypeNew.Length);
        switch (_listType)
        {
            case LIST_TYPE_BULLET:
                outp.Write(t = IntToByteArray(23), 0, t.Length);
                break;
            case LIST_TYPE_NUMBERED:
                outp.Write(t = IntToByteArray(0), 0, t.Length);
                break;
            case LIST_TYPE_UPPER_LETTERS:
                outp.Write(t = IntToByteArray(3), 0, t.Length);
                break;
            case LIST_TYPE_LOWER_LETTERS:
                outp.Write(t = IntToByteArray(4), 0, t.Length);
                break;
            case LIST_TYPE_UPPER_ROMAN:
                outp.Write(t = IntToByteArray(1), 0, t.Length);
                break;
            case LIST_TYPE_LOWER_ROMAN:
                outp.Write(t = IntToByteArray(2), 0, t.Length);
                break;
            /* New types */
            case LIST_TYPE_ARABIC:
                outp.Write(t = IntToByteArray(0), 0, t.Length);
                break;
            case LIST_TYPE_UPPERCASE_ROMAN_NUMERAL:
                outp.Write(t = IntToByteArray(1), 0, t.Length);
                break;
            case LIST_TYPE_LOWERCASE_ROMAN_NUMERAL:
                outp.Write(t = IntToByteArray(2), 0, t.Length);
                break;
            case LIST_TYPE_UPPERCASE_LETTER:
                outp.Write(t = IntToByteArray(3), 0, t.Length);
                break;
            case LIST_TYPE_ORDINAL_NUMBER:
                outp.Write(t = IntToByteArray(4), 0, t.Length);
                break;
            case LIST_TYPE_CARDINAL_TEXT_NUMBER:
                outp.Write(t = IntToByteArray(5), 0, t.Length);
                break;
            case LIST_TYPE_ORDINAL_TEXT_NUMBER:
                outp.Write(t = IntToByteArray(6), 0, t.Length);
                break;
            case LIST_TYPE_LOWERCASE_LETTER:
                outp.Write(t = IntToByteArray(7), 0, t.Length);
                break;
            case LIST_TYPE_ARABIC_LEADING_ZERO:
                outp.Write(t = IntToByteArray(22), 0, t.Length);
                break;
            case LIST_TYPE_NO_NUMBER:
                outp.Write(t = IntToByteArray(255), 0, t.Length);
                break;
            default: // catch all for other unsupported types
                if (_listType >= LIST_TYPE_BASE)
                {
                    outp.Write(t = IntToByteArray(_listType - LIST_TYPE_BASE), 0, t.Length);
                }

                break;
        }

        outp.Write(_listLevelAlignment, 0, _listLevelAlignment.Length);
        outp.Write(t = IntToByteArray(0), 0, t.Length);
        outp.Write(_listLevelAlignmentNew, 0, _listLevelAlignmentNew.Length);
        outp.Write(t = IntToByteArray(0), 0, t.Length);
        outp.Write(_listLevelFolow, 0, _listLevelFolow.Length);
        outp.Write(t = IntToByteArray(_levelFollowValue), 0, t.Length);
        outp.Write(_listLevelStartAt, 0, _listLevelStartAt.Length);
        outp.Write(t = IntToByteArray(_listStartAt), 0, t.Length);
        if (_isTentative)
        {
            outp.Write(_listLevelTentative, 0, _listLevelTentative.Length);
        }

        if (_isLegal)
        {
            outp.Write(_listLevelLegal, 0, _listLevelLegal.Length);
        }

        outp.Write(_listLevelSpace, 0, _listLevelSpace.Length);
        outp.Write(t = IntToByteArray(0), 0, t.Length);
        outp.Write(_listLevelIndent, 0, _listLevelIndent.Length);
        outp.Write(t = IntToByteArray(0), 0, t.Length);
        if (_levelPicture != -1)
        {
            outp.Write(_listLevelPicture, 0, _listLevelPicture.Length);
            outp.Write(t = IntToByteArray(_levelPicture), 0, t.Length);
        }

        outp.Write(OpenGroup, 0, OpenGroup.Length); // { leveltext
        outp.Write(_listLevelText, 0, _listLevelText.Length);
        outp.Write(_listLevelTemplateId, 0, _listLevelTemplateId.Length);
        outp.Write(t = IntToByteArray(_templateId), 0, t.Length);
        /* NEVER seperate the LEVELTEXT elements with a return in between
        * them or it will not fuction correctly!
        */
        // TODO Needs to be rewritten to support 1-9 levels, not just simple single level
        if (_listType != LIST_TYPE_BULLET)
        {
            outp.Write(_listLevelStyleNumberedBegin, 0, _listLevelStyleNumberedBegin.Length);
            if (_levelTextNumber < 10)
            {
                outp.Write(t = IntToByteArray(0), 0, t.Length);
            }

            outp.Write(t = IntToByteArray(_levelTextNumber), 0, t.Length);
            outp.Write(_listLevelStyleNumberedEnd, 0, _listLevelStyleNumberedEnd.Length);
        }
        else
        {
            outp.Write(_listLevelStyleBulletedBegin, 0, _listLevelStyleBulletedBegin.Length);
            Document.FilterSpecialChar(outp, _bulletCharacter, false, false);
            outp.Write(_listLevelStyleBulletedEnd, 0, _listLevelStyleBulletedEnd.Length);
        }

        outp.Write(CloseGroup, 0, CloseGroup.Length); // } leveltext

        outp.Write(OpenGroup, 0, OpenGroup.Length); // { levelnumbers
        outp.Write(_listLevelNumbersBegin, 0, _listLevelNumbersBegin.Length);
        if (_listType != LIST_TYPE_BULLET)
        {
            outp.Write(_listLevelNumbersNumbered, 0, _listLevelNumbersNumbered.Length);
        }

        outp.Write(_listLevelNumbersEnd, 0, _listLevelNumbersEnd.Length);
        outp.Write(CloseGroup, 0, CloseGroup.Length); // { levelnumbers

        // write properties now
        outp.Write(RtfFontList.FontNumber, 0, RtfFontList.FontNumber.Length);
        if (_listType != LIST_TYPE_BULLET)
        {
            outp.Write(t = IntToByteArray(_fontNumber.GetFontNumber()), 0, t.Length);
        }
        else
        {
            outp.Write(t = IntToByteArray(_fontBullet.GetFontNumber()), 0, t.Length);
        }

        outp.Write(t = DocWriter.GetIsoBytes("\\cf"), 0, t.Length);
        //        document.GetDocumentHeader().GetColorNumber(new RtfColor(this.document,this.GetFontNumber().GetColor()));
        outp.Write(t = IntToByteArray(Document.GetDocumentHeader().GetColorNumber(new RtfColor(Document, GetFontNumber().Color))),
                   0, t.Length);

        WriteIndentation(outp);
        outp.Write(CloseGroup, 0, CloseGroup.Length);
        Document.OutputDebugLinebreak(outp);
    }

    /// <summary>
    /// </summary>
    /// <returns>the alignment</returns>
    public int GetAlignment() => _alignment;

    public string GetBulletCharacter() => _bulletCharacter;

    /// <summary>
    /// </summary>
    /// <returns>the firstIndent</returns>
    public int GetFirstIndent() => _firstIndent;

    /// <summary>
    /// </summary>
    /// <returns>the fontBullet</returns>
    public RtfFont GetFontBullet() => _fontBullet;

    /// <summary>
    /// </summary>
    /// <returns>the fontNumber</returns>
    public RtfFont GetFontNumber() => _fontNumber;

    /// <summary>
    /// </summary>
    /// <returns>the leftIndent</returns>
    public int GetLeftIndent() => _leftIndent;

    /// <summary>
    /// </summary>
    /// <returns>the levelFollowValue</returns>
    public int GetLevelFollowValue() => _levelFollowValue;

    /// <summary>
    /// </summary>
    /// <returns>the levelTextNumber</returns>
    public int GetLevelTextNumber() => _levelTextNumber;

    /// <summary>
    ///     Gets the list level of this RtfList
    /// </summary>
    /// <returns>Returns the list level.</returns>
    public int GetListLevel() => listLevel;

    /// <summary>
    /// </summary>
    /// <returns>the listLevelParent</returns>
    public RtfListLevel GetListLevelParent() => _listLevelParent;

    /// <summary>
    /// </summary>
    /// <returns>the listNoRestart</returns>
    public int GetListNoRestart() => _listNoRestart;

    /// <summary>
    /// </summary>
    /// <returns>the listStartAt</returns>
    public int GetListStartAt() => _listStartAt;

    /// <summary>
    /// </summary>
    /// <returns>the listType</returns>
    public int GetListType() => _listType;

    /// <summary>
    /// </summary>
    /// <returns>the parent</returns>
    public RtfList GetParent() => _parent;

    /// <summary>
    /// </summary>
    /// <returns>the rightIndent</returns>
    public int GetRightIndent() => _rightIndent;

    /// <summary>
    /// </summary>
    /// <returns>the symbolIndent</returns>
    public int GetSymbolIndent() => _symbolIndent;

    /// <summary>
    /// </summary>
    /// <returns>the isLegal</returns>
    public bool IsLegal() => _isLegal;

    /// <summary>
    /// </summary>
    /// <returns>the isTentative</returns>
    public bool IsTentative() => _isTentative;

    /// <summary>
    /// </summary>
    /// <param name="alignment">the alignment to set</param>
    public void SetAlignment(int alignment)
    {
        _alignment = alignment;
    }

    /// <summary>
    /// </summary>
    /// <param name="bulletCharacter">the bulletCharacter to set</param>
    public void SetBulletCharacter(string bulletCharacter)
    {
        _bulletCharacter = bulletCharacter;
    }

    /// <summary>
    ///     @since 2.1.4
    /// </summary>
    /// <param name="bulletCharacter"></param>
    public void SetBulletChunk(Chunk bulletCharacter)
    {
        _bulletChunk = bulletCharacter;
    }

    /// <summary>
    ///     set the bullet font
    /// </summary>
    /// <param name="f"></param>
    public void SetBulletFont(Font f)
    {
        _fontBullet = new RtfFont(Document, f);
    }

    /// <summary>
    /// </summary>
    /// <param name="firstIndent">the firstIndent to set</param>
    public void SetFirstIndent(int firstIndent)
    {
        _firstIndent = firstIndent;
    }

    /// <summary>
    /// </summary>
    /// <param name="fontBullet">the fontBullet to set</param>
    public void SetFontBullet(RtfFont fontBullet)
    {
        _fontBullet = fontBullet;
    }

    /// <summary>
    /// </summary>
    /// <param name="fontNumber">the fontNumber to set</param>
    public void SetFontNumber(RtfFont fontNumber)
    {
        _fontNumber = fontNumber;
    }

    /// <summary>
    /// </summary>
    /// <param name="leftIndent">the leftIndent to set</param>
    public void SetLeftIndent(int leftIndent)
    {
        _leftIndent = leftIndent;
    }

    /// <summary>
    /// </summary>
    /// <param name="isLegal">the isLegal to set</param>
    public void SetLegal(bool isLegal)
    {
        _isLegal = isLegal;
    }

    /// <summary>
    /// </summary>
    /// <param name="levelFollowValue">the levelFollowValue to set</param>
    public void SetLevelFollowValue(int levelFollowValue)
    {
        _levelFollowValue = levelFollowValue;
    }

    /// <summary>
    /// </summary>
    /// <param name="levelTextNumber">the levelTextNumber to set</param>
    public void SetLevelTextNumber(int levelTextNumber)
    {
        _levelTextNumber = levelTextNumber;
    }

    /// <summary>
    ///     Sets the list level of this RtfList.
    /// </summary>
    /// <param name="listLevel">The list level to set.</param>
    public void SetListLevel(int listLevel)
    {
        this.listLevel = listLevel;
    }

    /// <summary>
    /// </summary>
    /// <param name="listLevelParent">the listLevelParent to set</param>
    public void SetListLevelParent(RtfListLevel listLevelParent)
    {
        _listLevelParent = listLevelParent;
    }

    /// <summary>
    /// </summary>
    /// <param name="listNoRestart">the listNoRestart to set</param>
    public void SetListNoRestart(int listNoRestart)
    {
        _listNoRestart = listNoRestart;
    }

    /// <summary>
    /// </summary>
    /// <param name="listStartAt">the listStartAt to set</param>
    public void SetListStartAt(int listStartAt)
    {
        _listStartAt = listStartAt;
    }

    /// <summary>
    /// </summary>
    /// <param name="listType">the listType to set</param>
    public void SetListType(int listType)
    {
        _listType = listType;
    }

    /// <summary>
    /// </summary>
    /// <param name="parent">the parent to set</param>
    public void SetParent(RtfList parent)
    {
        _parent = parent;
    }

    /// <summary>
    /// </summary>
    /// <param name="rightIndent">the rightIndent to set</param>
    public void SetRightIndent(int rightIndent)
    {
        _rightIndent = rightIndent;
    }

    /// <summary>
    /// </summary>
    /// <param name="symbolIndent">the symbolIndent to set</param>
    public void SetSymbolIndent(int symbolIndent)
    {
        _symbolIndent = symbolIndent;
    }

    /// <summary>
    /// </summary>
    /// <param name="isTentative">the isTentative to set</param>
    public void SetTentative(bool isTentative)
    {
        _isTentative = isTentative;
    }

    /// <summary>
    ///     Write the indentation values for this  RtfList .
    ///     @throws IOException On i/o errors.
    /// </summary>
    /// <param name="result">The  Stream  to write to.</param>
    public void WriteIndentation(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        byte[] t;
        result.Write(_listLevelFirstIndent, 0, _listLevelFirstIndent.Length);
        result.Write(t = IntToByteArray(_firstIndent), 0, t.Length);
        result.Write(RtfParagraphStyle.IndentLeft, 0, RtfParagraphStyle.IndentLeft.Length);
        result.Write(t = IntToByteArray(_leftIndent), 0, t.Length);
        result.Write(RtfParagraphStyle.IndentRight, 0, RtfParagraphStyle.IndentRight.Length);
        result.Write(t = IntToByteArray(_rightIndent), 0, t.Length);
        result.Write(_listLevelSymbolIndent, 0, _listLevelSymbolIndent.Length);
        result.Write(t = IntToByteArray(_leftIndent), 0, t.Length);
    }

    /// <summary>
    ///     Writes the initialization part of the RtfList
    ///     @throws IOException On i/o errors.
    /// </summary>
    /// <param name="result">The  Stream  to write to</param>
    public void WriteListBeginning(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        byte[] t;
        result.Write(RtfPhrase.ParagraphDefaults, 0, RtfPhrase.ParagraphDefaults.Length);
        if (InTable)
        {
            result.Write(RtfPhrase.InTable, 0, RtfPhrase.InTable.Length);
        }

        switch (_alignment)
        {
            case Element.ALIGN_LEFT:
                result.Write(RtfParagraphStyle.AlignLeft, 0, RtfParagraphStyle.AlignLeft.Length);
                break;
            case Element.ALIGN_RIGHT:
                result.Write(RtfParagraphStyle.AlignRight, 0, RtfParagraphStyle.AlignRight.Length);
                break;
            case Element.ALIGN_CENTER:
                result.Write(RtfParagraphStyle.AlignCenter, 0, RtfParagraphStyle.AlignCenter.Length);
                break;
            case Element.ALIGN_JUSTIFIED:
            case Element.ALIGN_JUSTIFIED_ALL:
                result.Write(RtfParagraphStyle.AlignJustify, 0, RtfParagraphStyle.AlignJustify.Length);
                break;
        }

        WriteIndentation(result);
        result.Write(RtfFont.FontSize, 0, RtfFont.FontSize.Length);
        result.Write(t = IntToByteArray(_fontNumber.GetFontSize() * 2), 0, t.Length);
        if (_symbolIndent > 0)
        {
            result.Write(_listLevelSymbolIndent, 0, _listLevelSymbolIndent.Length);
            result.Write(t = IntToByteArray(_leftIndent), 0, t.Length);
        }
    }

    /// <summary>
    ///     Correct the indentation of this level
    /// </summary>
    protected internal void CorrectIndentation()
    {
        if (_listLevelParent != null)
        {
            _leftIndent = _leftIndent + _listLevelParent.GetLeftIndent() + _listLevelParent.GetFirstIndent();
        }
    }

    /// <summary>
    ///     Writes only the list number and list level number.
    ///     @throws IOException On i/o errors.
    /// </summary>
    /// <param name="result">The  Stream  to write to</param>
    protected void WriteListNumbers(Stream result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        byte[] t;
        if (listLevel > 0)
        {
            result.Write(RtfList.ListLevelNumber, 0, RtfList.ListLevelNumber.Length);
            result.Write(t = IntToByteArray(listLevel), 0, t.Length);
        }
    }
}