using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iTextSharp.LGPLv2.Core.FunctionalTests.Issues;

/// <summary>
///     https://github.com/VahidN/iTextSharp.LGPLv2.Core/issues/61
/// </summary>
[TestClass]
public class Issue61
{
    [TestMethod]
    public void Verify_Issue61_Khemer_CanBe_Processed()
    {
        var pdfDoc = new Document(PageSize.A4);

        var pdfFilePath = TestUtils.GetOutputFileName();
        var fileStream = new FileStream(pdfFilePath, FileMode.Create);
        PdfWriter.GetInstance(pdfDoc, fileStream);

        pdfDoc.AddAuthor(TestUtils.Author);
        pdfDoc.Open();

        /*var khemerFont1 = TestUtils.GetUnicodeFont(
                "Kh Battambang", TestUtils.GetFontPath("Kh-Battambang.ttf"), 20, Font.NORMAL, BaseColor.Blue);
        var khemerFont2 = TestUtils.GetUnicodeFont(
                "Khmer OS", TestUtils.GetFontPath("KhmerOS.ttf"), 20, Font.NORMAL, BaseColor.Blue);
        var khemerFont3 = TestUtils.GetUnicodeFont(
                "Battambang", TestUtils.GetFontPath("Kh-Battambang-Regular.ttf"), 20, Font.NORMAL, BaseColor.Blue);
        var khemerFont4 = TestUtils.GetUnicodeFont(
                "Khmer OS Battambang", TestUtils.GetFontPath("KhmerOSBattambang-Regular.ttf"), 20, Font.NORMAL, BaseColor.Blue);
        var khemerFont5 = TestUtils.GetUnicodeFont(
                "Khmer", TestUtils.GetFontPath("Khmer-Regular.ttf"), 20, Font.NORMAL, BaseColor.Blue);
        var khemerFont7 = TestUtils.GetUnicodeFont(
                "Nokora", TestUtils.GetFontPath("Nokora-Regular.ttf"), 20, Font.NORMAL, BaseColor.Blue);
                */
        var khemerFont6 = TestUtils.GetUnicodeFont(
                                                   "Kh System", TestUtils.GetFontPath("Kh-System.ttf"),
                                                   20, Font.NORMAL, BaseColor.Blue);

        var text = "Hello World សួស្តី​ពិភពលោក";
        //var processedData = new KhmerLigaturizer().ProcessKhmer(text, khemerFont6);

        pdfDoc.Add(new Paragraph("Original Text:  " + text, khemerFont6));
        //pdfDoc.Add(new Paragraph("Processed Text: " + processedData, khemerFont6));

        // NOTE: It requires `GlyphSubstitutionTable (GSUB)` support to work correctly.
        // https://docs.microsoft.com/en-us/typography/opentype/spec/gsub

        pdfDoc.Close();
        fileStream.Dispose();

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }

    [TestMethod]
    public void Verify_Issue61_Indic_CanBe_Processed()
    {
        var pdfDoc = new Document(PageSize.A4);

        var pdfFilePath = TestUtils.GetOutputFileName();
        var fileStream = new FileStream(pdfFilePath, FileMode.Create);
        PdfWriter.GetInstance(pdfDoc, fileStream);

        pdfDoc.AddAuthor(TestUtils.Author);
        pdfDoc.Open();

        var font = TestUtils.GetUnicodeFont(
                                            "Lohit Bengali", TestUtils.GetFontPath("Lohit-Bengali.ttf"),
                                            20, Font.NORMAL, BaseColor.Blue);

        var text = "আমি কোন পথে ক্ষীরের ষন্ড পুতুল রুপো গঙ্গা ঋষি";
        pdfDoc.Add(new Paragraph(text, font));

        pdfDoc.Close();
        fileStream.Dispose();

        TestUtils.VerifyPdfFileIsReadable(pdfFilePath);
    }
}

public class KhmerLigaturizer : BidiLine
{
    public string ProcessKhmer(string text, Font font)
    {
        var content = new KhmerUnicodeRender().Render(text);
        AddChunk(new PdfChunk(new Chunk(content, font), null));
        ArabicOptions = 0;
        GetParagraph(PdfWriter.RUN_DIRECTION_LTR);
        var arr = CreateArrayOfPdfChunks(0, TotalTextLength - 1);
        var sb = new StringBuilder();
        foreach (var ck in arr)
        {
            sb.Append(ck);
        }

        return sb.ToString();
    }
}

/// <summary>
///     Ported from https://github.com/Seuksa/iTextKhmer
/// </summary>
public class KhmerUnicodeRender
{
    private const int _xx = 0;
    private const int CC_COENG = 7; // Subscript consonant combining character
    private const int CC_CONSONANT = 1; // Consonant of type 1 or independent vowel
    private const int CC_CONSONANT_SHIFTER = 5;
    private const int CC_CONSONANT2 = 2; // Consonant of type 2
    private const int CC_CONSONANT3 = 3; // Consonant of type 3
    private const int CC_DEPENDENT_VOWEL = 8;
    private const int CC_ROBAT = 6; // Khmer special diacritic accent -treated differently in state table
    private const int CC_SIGN_ABOVE = 9;
    private const int CC_SIGN_AFTER = 10;
    private const int CF_ABOVE_VOWEL = 536870912; // flag to speed up comparing
    private const int CF_CLASS_MASK = 65535;
    private const int CF_COENG = 134217728; // flag to speed up comparing
    private const int CF_CONSONANT = 16777216; // flag to speed up comparing
    private const int CF_DOTTED_CIRCLE = 67108864;

    // add a dotted circle if a character with this flag is the first in a syllable
    private const int CF_POS_ABOVE = 131072;
    private const int CF_POS_AFTER = 65536;
    private const int CF_POS_BEFORE = 524288;
    private const int CF_POS_BELOW = 262144;
    private const int CF_SHIFTER = 268435456; // flag to speed up comparing
    private const int CF_SPLIT_VOWEL = 33554432;
    private const int _c1 = CC_CONSONANT + CF_CONSONANT;
    private const int _c2 = CC_CONSONANT2 + CF_CONSONANT;
    private const int _c3 = CC_CONSONANT3 + CF_CONSONANT;
    private const int _co = CC_COENG + CF_COENG + CF_DOTTED_CIRCLE;
    private const int _cs = CC_CONSONANT_SHIFTER + CF_DOTTED_CIRCLE + CF_SHIFTER;
    private const int _da = CC_DEPENDENT_VOWEL + CF_POS_ABOVE + CF_DOTTED_CIRCLE + CF_ABOVE_VOWEL;
    private const int _db = CC_DEPENDENT_VOWEL + CF_POS_BELOW + CF_DOTTED_CIRCLE;
    private const int _dl = CC_DEPENDENT_VOWEL + CF_POS_BEFORE + CF_DOTTED_CIRCLE;
    private const int _dr = CC_DEPENDENT_VOWEL + CF_POS_AFTER + CF_DOTTED_CIRCLE;
    private const int _rb = CC_ROBAT + CF_POS_ABOVE + CF_DOTTED_CIRCLE;
    private const int _sa = CC_SIGN_ABOVE + CF_DOTTED_CIRCLE + CF_POS_ABOVE;
    private const int _sp = CC_SIGN_AFTER + CF_DOTTED_CIRCLE + CF_POS_AFTER;
    private const int _va = _da + CF_SPLIT_VOWEL;
    private const int _vr = _dr + CF_SPLIT_VOWEL;

    private const char SRAAA = '\u17B6';
    private const char SRAE = '\u17C1';
    private const char SRAOE = '\u17BE';
    private const char SRAOO = '\u17C4';
    private const char SRAYA = '\u17BF';
    private const char SRAIE = '\u17C0';
    private const char SRAAU = '\u17C5';
    private const char SRAII = '\u17B8';
    private const char SRAU = '\u17BB';
    private const char TRIISAP = '\u17CA';
    private const char NYO = '\u1789';
    private const char BA = '\u1794';
    private const char YO = '\u1799';
    private const char SA = '\u179F';
    private const char COENG = '\u17D2';
    private const string CORO = "\u17D2\u179A";
    private const string CONYO = "\u17D2\u1789";
    private const char MARK = '\u17EA';

    private readonly int[] _khmerCharClasses =
    {
        _c1, _c1, _c1, _c3, _c1, _c1, _c1, _c1, _c3, _c1, _c1, _c1, _c1,
        _c3, _c1, _c1, _c1, _c1, _c1, _c1, _c3, _c1, _c1, _c1, _c1, _c3,
        _c2, _c1, _c1, _c1, _c3, _c3, _c1, _c3, _c1, _c1, _c1, _c1, _c1,
        _c1, _c1, _c1, _c1, _c1, _c1, _c1, _c1, _c1, _c1, _c1, _c1, _c1,
        _dr, _dr, _dr, _da, _da, _da, _da, _db, _db, _db, _va, _vr, _vr,
        _dl, _dl, _dl, _vr, _vr, _sa, _sp, _sp, _cs, _cs, _sa, _rb, _sa,
        _sa, _sa, _sa, _sa, _co, _sa, _xx, _xx, _xx, _xx, _xx, _xx, _xx,
        _xx, _xx, _sa, _xx, _xx,
    };

    private readonly short[,] _khmerStateTable =
    {
        {
            1, 2, 2, 2, 1, 1, 1, 6, 1, 1, 1, 2,
        },
        {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        },
        {
            -1, -1, -1, -1, 3, 4, 5, 6, 16, 17, 1, -1,
        },
        {
            -1, -1, -1, -1, -1, 4, -1, -1, 16, -1, -1, -1,
        },
        {
            -1, -1, -1, -1, 15, -1, -1, 6, 16, 17, 1, 14,
        },
        {
            -1, -1, -1, -1, -1, -1, -1, -1, 20, -1, 1, -1,
        },
        {
            -1, 7, 8, 9, -1, -1, -1, -1, -1, -1, -1, -1,
        },
        {
            -1, -1, -1, -1, 12, 13, -1, 10, 16, 17, 1, 14,
        },
        {
            -1, -1, -1, -1, 12, 13, -1, -1, 16, 17, 1, 14,
        },
        {
            -1, -1, -1, -1, 12, 13, -1, 10, 16, 17, 1, 14,
        },
        {
            -1, 11, 11, 11, -1, -1, -1, -1, -1, -1, -1, -1,
        },
        {
            -1, -1, -1, -1, 15, -1, -1, -1, 16, 17, 1, 14,
        },
        {
            -1, -1, -1, -1, -1, 13, -1, -1, 16, -1, -1, -1,
        },
        {
            -1, -1, -1, -1, 15, -1, -1, -1, 16, 17, 1, 14,
        },
        {
            -1, -1, -1, -1, -1, -1, -1, -1, 16, -1, -1, -1,
        },
        {
            -1, -1, -1, -1, -1, -1, -1, -1, 16, -1, -1, -1,
        },
        {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, 17, 1, 18,
        },
        {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 18,
        },
        {
            -1, -1, -1, -1, -1, -1, -1, 19, -1, -1, -1, -1,
        },
        {
            -1, 1, -1, 1, -1, -1, -1, -1, -1, -1, -1, -1,
        },
        {
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, -1,
        },
    };

    private char strEcombining(char chrInput)
    {
        if (chrInput == SRAOE)
        {
            return SRAII;
        }

        if (chrInput == SRAYA)
        {
            return SRAYA;
        }

        if (chrInput == SRAIE)
        {
            return SRAIE;
        }

        if (chrInput == SRAOO)
        {
            return SRAAA;
        }

        if (chrInput == SRAAU)
        {
            return SRAAU;
        }

        return ' ';
    }

    // Gets the charactor class.
    private int getCharClass(char uniChar)
    {
        var retValue = 0;
        var ch = uniChar;
        if (ch > 255 && ch >= '\u1780')
        {
            ch -= '\u1780';
            if (ch < _khmerCharClasses.Length)
            {
                retValue = _khmerCharClasses[ch];
            }
        }

        return retValue;
    }

    /**
     * Re-order Khmer unicode for display with Khmer.ttf file on Android.
     * @param strInput Khmer unicode string.
     * @return string after render.
     */
    public string Render(string strInput)
    {
        //Given an input string of unicode cluster to reorder.
        //The return is the visual based cluster (legacy style) String.

        var cursor = 0;
        short state = 0;
        var charCount = strInput.Length;
        var result = new StringBuilder();

        while (cursor < charCount)
        {
            var _reserved = "";
            var _signAbove = "";
            var _signAfter = "";
            var _base = "";
            var _robat = "";
            var _shifter = "";
            var _vowelBefore = "";
            var _vowelBelow = "";
            var _vowelAbove = "";
            var _vowelAfter = "";
            var _coeng = false;
            string _cluster;

            var _coeng1 = "";
            var _coeng2 = "";

            var _shifterAfterCoeng = false;

            while (cursor < charCount)
            {
                var curChar = strInput[cursor];
                var kChar = getCharClass(curChar);
                var charClass = kChar & CF_CLASS_MASK;
                try
                {
                    state = _khmerStateTable[state, charClass];
                }
                catch
                {
                    state = -1;
                }

                if (state < 0)
                {
                    break;
                }

                //collect variable for cluster here

                switch (kChar)
                {
                    case _xx:
                        _reserved = curChar.ToString();
                        break;
                    case _sa:
                        _signAbove = curChar.ToString();
                        break;
                    case _sp:
                        _signAfter = curChar.ToString();
                        break;
                    case _c1:
                    case _c2:
                    case _c3:
                        if (_coeng)
                        {
                            if ("" == _coeng1)
                            {
                                _coeng1 = string.Concat(COENG.ToString(), curChar.ToString());
                            }
                            else
                            {
                                _coeng2 = string.Concat(COENG.ToString(), curChar.ToString());
                            }

                            _coeng = false;
                        }
                        else
                        {
                            _base = curChar.ToString();
                        }

                        break;
                    case _rb:
                        _robat = curChar.ToString();
                        break;
                    case _cs:
                        if ("" != _coeng1)
                        {
                            _shifterAfterCoeng = true;
                        }

                        _shifter = curChar.ToString();
                        break;
                    case _dl:
                        _vowelBefore = curChar.ToString();
                        break;
                    case _db:
                        _vowelBelow = curChar.ToString();
                        break;
                    case _da:
                        _vowelAbove = curChar.ToString();
                        break;
                    case _dr:
                        _vowelAfter = curChar.ToString();
                        break;
                    case _co:
                        _coeng = true;
                        break;
                    case _va:
                        _vowelBefore = SRAE.ToString();
                        _vowelAbove = strEcombining(curChar).ToString();
                        break;
                    case _vr:
                        _vowelBefore = SRAE.ToString();
                        _vowelAfter = strEcombining(curChar).ToString();
                        break;
                }

                cursor++;
            }
            // end of while (a cluster has found)

            // logic when cluster has coeng
            // should coeng be located on left side
            var _coengBefore = "";
            if (CORO.EqualsIgnoreCase(_coeng1))
            {
                _coengBefore = _coeng1;
                _coeng1 = "";
            }
            else if (CORO.EqualsIgnoreCase(_coeng2))
            {
                _coengBefore = _coeng2;
                _coeng2 = "";
            }

            if (!"".EqualsIgnoreCase(_coeng1) || !"".EqualsIgnoreCase(_coeng2))
            {
                // NYO must change to other form when there is coeng
                if (Character.ToString(NYO).EqualsIgnoreCase(_base))
                {
                    _base = "\uF0AE";

                    if (_coeng1.EqualsIgnoreCase(CONYO))
                    {
                        _coeng1 = "\uF0CB";
                    }
                }
            }

            //logic of shifter with base character
            if (!"".EqualsIgnoreCase(_base) && !"".EqualsIgnoreCase(_shifter))
            {
                //special case apply to BA only
                if (!"".EqualsIgnoreCase(_vowelAbove) && Character.ToString(BA).EqualsIgnoreCase(_base) &&
                    Character.ToString(TRIISAP).EqualsIgnoreCase(_shifter))
                {
                    _vowelAbove = _vowelAbove.GetVowelAbove();
                }
                else if (!"".EqualsIgnoreCase(_vowelAbove))
                {
                    _shifter = "";
                    _vowelBelow = Character.ToString(SRAU);
                }
            }

            // uncomplete coeng
            if (_coeng && "".EqualsIgnoreCase(_coeng1))
            {
                _coeng1 = Character.ToString(COENG);
            }
            else if (_coeng && "".EqualsIgnoreCase(_coeng2))
            {
                _coeng2 = Character.ToString(MARK).Concat(Character.ToString(COENG));
            }

            //render DOTCIRCLE for standalone sign or vowel
            if ("".EqualsIgnoreCase(_base) && ("".EqualsIgnoreCase(_vowelBefore) || "".EqualsIgnoreCase(_coengBefore) ||
                                               !"".EqualsIgnoreCase(_robat) || !"".EqualsIgnoreCase(_shifter) ||
                                               !"".EqualsIgnoreCase(_coeng1) || !"".EqualsIgnoreCase(_coeng2) ||
                                               !"".EqualsIgnoreCase(_vowelAfter) || !"".EqualsIgnoreCase(_vowelBelow) ||
                                               !"".EqualsIgnoreCase(_vowelAbove) || !"".EqualsIgnoreCase(_signAbove) ||
                                               !"".EqualsIgnoreCase(_signAfter)))
            {
                //_base = ""; //DOTCIRCLE
            }

            //place of shifter
            var _shifter1 = "";
            var _shifter2 = "";
            _shifter = _shifter.GetConsonantShifter();

            if (_shifterAfterCoeng)
            {
                _shifter2 = _shifter;
            }
            else
            {
                _shifter1 = _shifter;
            }

            var _specialCaseBA = false;
            var strMARK_SRAAA = Character.ToString(MARK).Concat(Character.ToString(SRAAA));
            var strMARK_SRAAU = Character.ToString(MARK).Concat(Character.ToString(SRAAU));

            if (Character.ToString(BA).EqualsIgnoreCase(_base) &&
                (Character.ToString(SRAAA).EqualsIgnoreCase(_vowelAfter) ||
                 Character.ToString(SRAAU).EqualsIgnoreCase(_vowelAfter) ||
                 strMARK_SRAAA.EqualsIgnoreCase(_vowelAfter) || strMARK_SRAAU.EqualsIgnoreCase(_vowelAfter)))
            {
                // SRAAA or SRAAU will get a MARK if there is coeng, redefine to last char
                _base = "\uF0AF";
                _specialCaseBA = true;

                if (!"".EqualsIgnoreCase(_coeng1))
                {
                    var _coeng1Complete = _coeng1[..^1];
                    if (Character.ToString(BA).EqualsIgnoreCase(_coeng1Complete) ||
                        Character.ToString(YO).EqualsIgnoreCase(_coeng1Complete) ||
                        Character.ToString(SA).EqualsIgnoreCase(_coeng1Complete))
                    {
                        _specialCaseBA = false;
                    }
                }
            }

            _coengBefore = _coengBefore.GetSubConsonant();
            _coeng1 = _coeng1.GetSubConsonant();
            _coeng2 = _coeng2.GetSubConsonant();
            _vowelAfter = _vowelAfter.GetChangeVowel();
            _signAbove = _signAbove.GetChangeVowel();

            if (!_robat.EqualsIgnoreCase(""))
            {
                _vowelAbove = _vowelAbove.GetVowelAbove();
            }
            else
            {
                _vowelAbove = _vowelAbove.GetChangeVowel();
            }

            if (!_coeng1.EqualsIgnoreCase("") || !_coeng2.EqualsIgnoreCase(""))
            {
                _vowelBelow = _vowelBelow.GetVowelBelow();
            }
            else
            {
                _vowelBelow = _vowelBelow.GetChangeVowel();
            }

            // cluster formation
            if (_specialCaseBA)
            {
                _cluster =
                    $"{_vowelBefore}{_coengBefore}{_base}{_vowelAfter}{_robat}{_shifter1}{_coeng1}{_coeng2}{_shifter2}{_vowelBelow}{_vowelAbove}{_signAbove}{_signAfter}";
            }
            else
            {
                _cluster =
                    $"{_vowelBefore}{_coengBefore}{_base}{_robat}{_shifter1}{_coeng1}{_coeng2}{_shifter2}{_vowelBelow}{_vowelAbove}{_vowelAfter}{_signAbove}{_signAfter}";
            }

            //            + "\u200B")
            result.Append(_cluster).Append(_reserved);
            state = 0;
            //end of while
        }

        return result.ToString();
    }
}

public static class Character
{
    public static string ToString(this char chr) => chr.ToString();
}

public static class Exts
{
    public static bool EqualsIgnoreCase(this string text1, string text2) =>
        text1.Equals(text2, StringComparison.OrdinalIgnoreCase);

    public static string Concat(this string text1, string text2) => string.Concat(text1, text2);
}

/// <summary>
///     Ported from https://github.com/Seuksa/iTextKhmer
/// </summary>
public static class KhmerUnicodeUtil
{
    private static readonly Dictionary<string, string> subConsonant = new()
                                                                      {
                                                                          { "\u17D2\u1780", "\uF000" },
                                                                          { "\u17D2\u1781", "\uF001" },
                                                                          { "\u17D2\u1782", "\uF002" },
                                                                          { "\u17D2\u1783", "\uF003" },
                                                                          { "\u17D2\u1784", "\uF004" },
                                                                          { "\u17D2\u1785", "\uF005" },
                                                                          { "\u17D2\u1786", "\uF006" },
                                                                          { "\u17D2\u1787", "\uF007" },
                                                                          { "\u17D2\u1788", "\uF008" },
                                                                          { "\u17D2\u1789", "\uF009" },
                                                                          { "\u17D2\u178A", "\uF00A" },
                                                                          { "\u17D2\u178B", "\uF00B" },
                                                                          { "\u17D2\u178C", "\uF00C" },
                                                                          { "\u17D2\u178D", "\uF00D" },
                                                                          { "\u17D2\u178E", "\uF00E" },
                                                                          { "\u17D2\u178F", "\uF00F" },
                                                                          { "\u17D2\u1790", "\uF010" },
                                                                          { "\u17D2\u1791", "\uF011" },
                                                                          { "\u17D2\u1792", "\uF012" },
                                                                          { "\u17D2\u1793", "\uF013" },
                                                                          { "\u17D2\u1794", "\uF014" },
                                                                          { "\u17D2\u1795", "\uF015" },
                                                                          { "\u17D2\u1796", "\uF016" },
                                                                          { "\u17D2\u1797", "\uF017" },
                                                                          { "\u17D2\u1798", "\uF018" },
                                                                          { "\u17D2\u1799", "\uF019" },
                                                                          { "\u17D2\u179A", "\uF01A" },
                                                                          { "\u17D2\u179B", "\uF01B" },
                                                                          { "\u17D2\u179C", "\uF01C" },
                                                                          { "\u17D2\u179F", "\uF01F" },
                                                                          { "\u17D2\u17A0", "\uF0A0" },
                                                                          { "\u17D2\u17A2", "\uF0A2" },
                                                                      };

    private static readonly Dictionary<string, string> vowelBelow = new()
                                                                    {
                                                                        { "\u17BB", "\uF0A3" },
                                                                        { "\u17BC", "\uF0A4" },
                                                                        { "\u17BD", "\uF0A5" },
                                                                    };

    private static readonly Dictionary<string, string> vowelAbove = new()
                                                                    {
                                                                        { "\u17B7", "\uF0A6" },
                                                                        { "\u17B8", "\uF0A7" },
                                                                        { "\u17B9", "\uF0A8" },
                                                                        { "\u17BA", "\uF0A9" },
                                                                    };

    private static readonly Dictionary<string, string> changeVowel = new()
                                                                     {
                                                                         { "\u17B7", "\uF0CD" }, //áž·
                                                                         { "\u17B8", "\uF0CE" }, //áž¸
                                                                         { "\u17B9", "\uF0CF" }, //áž¹
                                                                         { "\u17BA", "\uF0D0" }, //ážº
                                                                         { "\u17BB", "\uF0DC" }, //áž»
                                                                         { "\u17BC", "\uF0DD" }, //áž¼
                                                                         { "\u17BD", "\uF0DE" }, //áž½
                                                                         { "\u17C5", "\uF0CC" }, //áŸ…
                                                                         { "\u17C6", "\uF0D3" }, //áŸ†
                                                                         { "\u17BF", "\uF0D1" }, //áŸ€
                                                                         { "\u17C0", "\uF0D2" }, //áž¿

                                                                         //Sign Above
                                                                         { "\u17C9", "\uF0D4" }, //áŸ‰
                                                                         { "\u17CB", "\uF0D5" }, //áŸ‹
                                                                         { "\u17CC", "\uF0D6" }, //áŸŒ
                                                                         { "\u17CD", "\uF0D7" }, //áŸ�
                                                                         { "\u17CE", "\uF0D8" }, //
                                                                         { "\u17CF", "\uF0D9" }, //áŸ�
                                                                         { "\u17D0", "\uF0DA" }, //áŸ�
                                                                         { "\u17CA", "\uF0DB" }, //áŸŠ
                                                                     };

    private static readonly Dictionary<string, string> consonantShifter = new()
                                                                          {
                                                                              { "\u17C9", "\uF0D4" },
                                                                              { "\u17CA", "\uF0DB" },
                                                                          };

    public static string GetVowelBelow(this string key) => vowelBelow.TryGetValue(key, out var value) ? value : key;

    public static string GetVowelAbove(this string key) => vowelAbove.TryGetValue(key, out var value) ? value : key;

    public static string GetChangeVowel(this string key) => changeVowel.TryGetValue(key, out var value) ? value : key;

    public static string GetConsonantShifter(this string key) =>
        consonantShifter.TryGetValue(key, out var value) ? value : key;

    public static string GetSubConsonant(this string key) => subConsonant.TryGetValue(key, out var value) ? value : key;
}