using System.Text;
using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
///     Does all the line bidirectional processing with PdfChunk assembly.
///     @author Paulo Soares (psoares@consiste.pt)
/// </summary>
public class BidiLine
{
    private const int PieceSizeStart = 256;
    protected static NullValueDictionary<int, int> MirrorChars = new();
    protected int ArabicOptions;
    protected List<PdfChunk> Chunks = new();
    protected int CurrentChar;
    protected PdfChunk[] DetailChunks = new PdfChunk[PieceSizeStart];
    protected int[] IndexChars = new int[PieceSizeStart];
    protected int IndexChunk;
    protected int IndexChunkChar;
    protected byte[] OrderLevels = new byte[PieceSizeStart];
    protected int PieceSize = PieceSizeStart;
    protected int RunDirection;
    protected bool ShortStore;
    protected int StoredCurrentChar;
    protected PdfChunk[] StoredDetailChunks = Array.Empty<PdfChunk>();
    protected int[] StoredIndexChars = Array.Empty<int>();
    protected int StoredIndexChunk;
    protected int StoredIndexChunkChar;
    protected byte[] StoredOrderLevels = Array.Empty<byte>();
    protected int StoredRunDirection;
    protected char[] StoredText = Array.Empty<char>();
    protected int StoredTotalTextLength;
    protected char[] Text = new char[PieceSizeStart];
    protected int TotalTextLength;

    static BidiLine()
    {
        MirrorChars[0x0028] = 0x0029; // LEFT PARENTHESIS
        MirrorChars[0x0029] = 0x0028; // RIGHT PARENTHESIS
        MirrorChars[0x003C] = 0x003E; // LESS-THAN SIGN
        MirrorChars[0x003E] = 0x003C; // GREATER-THAN SIGN
        MirrorChars[0x005B] = 0x005D; // LEFT SQUARE BRACKET
        MirrorChars[0x005D] = 0x005B; // RIGHT SQUARE BRACKET
        MirrorChars[0x007B] = 0x007D; // LEFT CURLY BRACKET
        MirrorChars[0x007D] = 0x007B; // RIGHT CURLY BRACKET
        MirrorChars[0x00AB] = 0x00BB; // LEFT-POINTING DOUBLE ANGLE QUOTATION MARK
        MirrorChars[0x00BB] = 0x00AB; // RIGHT-POINTING DOUBLE ANGLE QUOTATION MARK
        MirrorChars[0x2039] = 0x203A; // SINGLE LEFT-POINTING ANGLE QUOTATION MARK
        MirrorChars[0x203A] = 0x2039; // SINGLE RIGHT-POINTING ANGLE QUOTATION MARK
        MirrorChars[0x2045] = 0x2046; // LEFT SQUARE BRACKET WITH QUILL
        MirrorChars[0x2046] = 0x2045; // RIGHT SQUARE BRACKET WITH QUILL
        MirrorChars[0x207D] = 0x207E; // SUPERSCRIPT LEFT PARENTHESIS
        MirrorChars[0x207E] = 0x207D; // SUPERSCRIPT RIGHT PARENTHESIS
        MirrorChars[0x208D] = 0x208E; // SUBSCRIPT LEFT PARENTHESIS
        MirrorChars[0x208E] = 0x208D; // SUBSCRIPT RIGHT PARENTHESIS
        MirrorChars[0x2208] = 0x220B; // ELEMENT OF
        MirrorChars[0x2209] = 0x220C; // NOT AN ELEMENT OF
        MirrorChars[0x220A] = 0x220D; // SMALL ELEMENT OF
        MirrorChars[0x220B] = 0x2208; // CONTAINS AS MEMBER
        MirrorChars[0x220C] = 0x2209; // DOES NOT CONTAIN AS MEMBER
        MirrorChars[0x220D] = 0x220A; // SMALL CONTAINS AS MEMBER
        MirrorChars[0x2215] = 0x29F5; // DIVISION SLASH
        MirrorChars[0x223C] = 0x223D; // TILDE OPERATOR
        MirrorChars[0x223D] = 0x223C; // REVERSED TILDE
        MirrorChars[0x2243] = 0x22CD; // ASYMPTOTICALLY EQUAL TO
        MirrorChars[0x2252] = 0x2253; // APPROXIMATELY EQUAL TO OR THE IMAGE OF
        MirrorChars[0x2253] = 0x2252; // IMAGE OF OR APPROXIMATELY EQUAL TO
        MirrorChars[0x2254] = 0x2255; // COLON EQUALS
        MirrorChars[0x2255] = 0x2254; // EQUALS COLON
        MirrorChars[0x2264] = 0x2265; // LESS-THAN OR EQUAL TO
        MirrorChars[0x2265] = 0x2264; // GREATER-THAN OR EQUAL TO
        MirrorChars[0x2266] = 0x2267; // LESS-THAN OVER EQUAL TO
        MirrorChars[0x2267] = 0x2266; // GREATER-THAN OVER EQUAL TO
        MirrorChars[0x2268] = 0x2269; // [BEST FIT] LESS-THAN BUT NOT EQUAL TO
        MirrorChars[0x2269] = 0x2268; // [BEST FIT] GREATER-THAN BUT NOT EQUAL TO
        MirrorChars[0x226A] = 0x226B; // MUCH LESS-THAN
        MirrorChars[0x226B] = 0x226A; // MUCH GREATER-THAN
        MirrorChars[0x226E] = 0x226F; // [BEST FIT] NOT LESS-THAN
        MirrorChars[0x226F] = 0x226E; // [BEST FIT] NOT GREATER-THAN
        MirrorChars[0x2270] = 0x2271; // [BEST FIT] NEITHER LESS-THAN NOR EQUAL TO
        MirrorChars[0x2271] = 0x2270; // [BEST FIT] NEITHER GREATER-THAN NOR EQUAL TO
        MirrorChars[0x2272] = 0x2273; // [BEST FIT] LESS-THAN OR EQUIVALENT TO
        MirrorChars[0x2273] = 0x2272; // [BEST FIT] GREATER-THAN OR EQUIVALENT TO
        MirrorChars[0x2274] = 0x2275; // [BEST FIT] NEITHER LESS-THAN NOR EQUIVALENT TO
        MirrorChars[0x2275] = 0x2274; // [BEST FIT] NEITHER GREATER-THAN NOR EQUIVALENT TO
        MirrorChars[0x2276] = 0x2277; // LESS-THAN OR GREATER-THAN
        MirrorChars[0x2277] = 0x2276; // GREATER-THAN OR LESS-THAN
        MirrorChars[0x2278] = 0x2279; // NEITHER LESS-THAN NOR GREATER-THAN
        MirrorChars[0x2279] = 0x2278; // NEITHER GREATER-THAN NOR LESS-THAN
        MirrorChars[0x227A] = 0x227B; // PRECEDES
        MirrorChars[0x227B] = 0x227A; // SUCCEEDS
        MirrorChars[0x227C] = 0x227D; // PRECEDES OR EQUAL TO
        MirrorChars[0x227D] = 0x227C; // SUCCEEDS OR EQUAL TO
        MirrorChars[0x227E] = 0x227F; // [BEST FIT] PRECEDES OR EQUIVALENT TO
        MirrorChars[0x227F] = 0x227E; // [BEST FIT] SUCCEEDS OR EQUIVALENT TO
        MirrorChars[0x2280] = 0x2281; // [BEST FIT] DOES NOT PRECEDE
        MirrorChars[0x2281] = 0x2280; // [BEST FIT] DOES NOT SUCCEED
        MirrorChars[0x2282] = 0x2283; // SUBSET OF
        MirrorChars[0x2283] = 0x2282; // SUPERSET OF
        MirrorChars[0x2284] = 0x2285; // [BEST FIT] NOT A SUBSET OF
        MirrorChars[0x2285] = 0x2284; // [BEST FIT] NOT A SUPERSET OF
        MirrorChars[0x2286] = 0x2287; // SUBSET OF OR EQUAL TO
        MirrorChars[0x2287] = 0x2286; // SUPERSET OF OR EQUAL TO
        MirrorChars[0x2288] = 0x2289; // [BEST FIT] NEITHER A SUBSET OF NOR EQUAL TO
        MirrorChars[0x2289] = 0x2288; // [BEST FIT] NEITHER A SUPERSET OF NOR EQUAL TO
        MirrorChars[0x228A] = 0x228B; // [BEST FIT] SUBSET OF WITH NOT EQUAL TO
        MirrorChars[0x228B] = 0x228A; // [BEST FIT] SUPERSET OF WITH NOT EQUAL TO
        MirrorChars[0x228F] = 0x2290; // SQUARE IMAGE OF
        MirrorChars[0x2290] = 0x228F; // SQUARE ORIGINAL OF
        MirrorChars[0x2291] = 0x2292; // SQUARE IMAGE OF OR EQUAL TO
        MirrorChars[0x2292] = 0x2291; // SQUARE ORIGINAL OF OR EQUAL TO
        MirrorChars[0x2298] = 0x29B8; // CIRCLED DIVISION SLASH
        MirrorChars[0x22A2] = 0x22A3; // RIGHT TACK
        MirrorChars[0x22A3] = 0x22A2; // LEFT TACK
        MirrorChars[0x22A6] = 0x2ADE; // ASSERTION
        MirrorChars[0x22A8] = 0x2AE4; // TRUE
        MirrorChars[0x22A9] = 0x2AE3; // FORCES
        MirrorChars[0x22AB] = 0x2AE5; // DOUBLE VERTICAL BAR DOUBLE RIGHT TURNSTILE
        MirrorChars[0x22B0] = 0x22B1; // PRECEDES UNDER RELATION
        MirrorChars[0x22B1] = 0x22B0; // SUCCEEDS UNDER RELATION
        MirrorChars[0x22B2] = 0x22B3; // NORMAL SUBGROUP OF
        MirrorChars[0x22B3] = 0x22B2; // CONTAINS AS NORMAL SUBGROUP
        MirrorChars[0x22B4] = 0x22B5; // NORMAL SUBGROUP OF OR EQUAL TO
        MirrorChars[0x22B5] = 0x22B4; // CONTAINS AS NORMAL SUBGROUP OR EQUAL TO
        MirrorChars[0x22B6] = 0x22B7; // ORIGINAL OF
        MirrorChars[0x22B7] = 0x22B6; // IMAGE OF
        MirrorChars[0x22C9] = 0x22CA; // LEFT NORMAL FACTOR SEMIDIRECT PRODUCT
        MirrorChars[0x22CA] = 0x22C9; // RIGHT NORMAL FACTOR SEMIDIRECT PRODUCT
        MirrorChars[0x22CB] = 0x22CC; // LEFT SEMIDIRECT PRODUCT
        MirrorChars[0x22CC] = 0x22CB; // RIGHT SEMIDIRECT PRODUCT
        MirrorChars[0x22CD] = 0x2243; // REVERSED TILDE EQUALS
        MirrorChars[0x22D0] = 0x22D1; // DOUBLE SUBSET
        MirrorChars[0x22D1] = 0x22D0; // DOUBLE SUPERSET
        MirrorChars[0x22D6] = 0x22D7; // LESS-THAN WITH DOT
        MirrorChars[0x22D7] = 0x22D6; // GREATER-THAN WITH DOT
        MirrorChars[0x22D8] = 0x22D9; // VERY MUCH LESS-THAN
        MirrorChars[0x22D9] = 0x22D8; // VERY MUCH GREATER-THAN
        MirrorChars[0x22DA] = 0x22DB; // LESS-THAN EQUAL TO OR GREATER-THAN
        MirrorChars[0x22DB] = 0x22DA; // GREATER-THAN EQUAL TO OR LESS-THAN
        MirrorChars[0x22DC] = 0x22DD; // EQUAL TO OR LESS-THAN
        MirrorChars[0x22DD] = 0x22DC; // EQUAL TO OR GREATER-THAN
        MirrorChars[0x22DE] = 0x22DF; // EQUAL TO OR PRECEDES
        MirrorChars[0x22DF] = 0x22DE; // EQUAL TO OR SUCCEEDS
        MirrorChars[0x22E0] = 0x22E1; // [BEST FIT] DOES NOT PRECEDE OR EQUAL
        MirrorChars[0x22E1] = 0x22E0; // [BEST FIT] DOES NOT SUCCEED OR EQUAL
        MirrorChars[0x22E2] = 0x22E3; // [BEST FIT] NOT SQUARE IMAGE OF OR EQUAL TO
        MirrorChars[0x22E3] = 0x22E2; // [BEST FIT] NOT SQUARE ORIGINAL OF OR EQUAL TO
        MirrorChars[0x22E4] = 0x22E5; // [BEST FIT] SQUARE IMAGE OF OR NOT EQUAL TO
        MirrorChars[0x22E5] = 0x22E4; // [BEST FIT] SQUARE ORIGINAL OF OR NOT EQUAL TO
        MirrorChars[0x22E6] = 0x22E7; // [BEST FIT] LESS-THAN BUT NOT EQUIVALENT TO
        MirrorChars[0x22E7] = 0x22E6; // [BEST FIT] GREATER-THAN BUT NOT EQUIVALENT TO
        MirrorChars[0x22E8] = 0x22E9; // [BEST FIT] PRECEDES BUT NOT EQUIVALENT TO
        MirrorChars[0x22E9] = 0x22E8; // [BEST FIT] SUCCEEDS BUT NOT EQUIVALENT TO
        MirrorChars[0x22EA] = 0x22EB; // [BEST FIT] NOT NORMAL SUBGROUP OF
        MirrorChars[0x22EB] = 0x22EA; // [BEST FIT] DOES NOT CONTAIN AS NORMAL SUBGROUP
        MirrorChars[0x22EC] = 0x22ED; // [BEST FIT] NOT NORMAL SUBGROUP OF OR EQUAL TO
        MirrorChars[0x22ED] = 0x22EC; // [BEST FIT] DOES NOT CONTAIN AS NORMAL SUBGROUP OR EQUAL
        MirrorChars[0x22F0] = 0x22F1; // UP RIGHT DIAGONAL ELLIPSIS
        MirrorChars[0x22F1] = 0x22F0; // DOWN RIGHT DIAGONAL ELLIPSIS
        MirrorChars[0x22F2] = 0x22FA; // ELEMENT OF WITH LONG HORIZONTAL STROKE
        MirrorChars[0x22F3] = 0x22FB; // ELEMENT OF WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
        MirrorChars[0x22F4] = 0x22FC; // SMALL ELEMENT OF WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
        MirrorChars[0x22F6] = 0x22FD; // ELEMENT OF WITH OVERBAR
        MirrorChars[0x22F7] = 0x22FE; // SMALL ELEMENT OF WITH OVERBAR
        MirrorChars[0x22FA] = 0x22F2; // CONTAINS WITH LONG HORIZONTAL STROKE
        MirrorChars[0x22FB] = 0x22F3; // CONTAINS WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
        MirrorChars[0x22FC] = 0x22F4; // SMALL CONTAINS WITH VERTICAL BAR AT END OF HORIZONTAL STROKE
        MirrorChars[0x22FD] = 0x22F6; // CONTAINS WITH OVERBAR
        MirrorChars[0x22FE] = 0x22F7; // SMALL CONTAINS WITH OVERBAR
        MirrorChars[0x2308] = 0x2309; // LEFT CEILING
        MirrorChars[0x2309] = 0x2308; // RIGHT CEILING
        MirrorChars[0x230A] = 0x230B; // LEFT FLOOR
        MirrorChars[0x230B] = 0x230A; // RIGHT FLOOR
        MirrorChars[0x2329] = 0x232A; // LEFT-POINTING ANGLE BRACKET
        MirrorChars[0x232A] = 0x2329; // RIGHT-POINTING ANGLE BRACKET
        MirrorChars[0x2768] = 0x2769; // MEDIUM LEFT PARENTHESIS ORNAMENT
        MirrorChars[0x2769] = 0x2768; // MEDIUM RIGHT PARENTHESIS ORNAMENT
        MirrorChars[0x276A] = 0x276B; // MEDIUM FLATTENED LEFT PARENTHESIS ORNAMENT
        MirrorChars[0x276B] = 0x276A; // MEDIUM FLATTENED RIGHT PARENTHESIS ORNAMENT
        MirrorChars[0x276C] = 0x276D; // MEDIUM LEFT-POINTING ANGLE BRACKET ORNAMENT
        MirrorChars[0x276D] = 0x276C; // MEDIUM RIGHT-POINTING ANGLE BRACKET ORNAMENT
        MirrorChars[0x276E] = 0x276F; // HEAVY LEFT-POINTING ANGLE QUOTATION MARK ORNAMENT
        MirrorChars[0x276F] = 0x276E; // HEAVY RIGHT-POINTING ANGLE QUOTATION MARK ORNAMENT
        MirrorChars[0x2770] = 0x2771; // HEAVY LEFT-POINTING ANGLE BRACKET ORNAMENT
        MirrorChars[0x2771] = 0x2770; // HEAVY RIGHT-POINTING ANGLE BRACKET ORNAMENT
        MirrorChars[0x2772] = 0x2773; // LIGHT LEFT TORTOISE SHELL BRACKET
        MirrorChars[0x2773] = 0x2772; // LIGHT RIGHT TORTOISE SHELL BRACKET
        MirrorChars[0x2774] = 0x2775; // MEDIUM LEFT CURLY BRACKET ORNAMENT
        MirrorChars[0x2775] = 0x2774; // MEDIUM RIGHT CURLY BRACKET ORNAMENT
        MirrorChars[0x27D5] = 0x27D6; // LEFT OUTER JOIN
        MirrorChars[0x27D6] = 0x27D5; // RIGHT OUTER JOIN
        MirrorChars[0x27DD] = 0x27DE; // LONG RIGHT TACK
        MirrorChars[0x27DE] = 0x27DD; // LONG LEFT TACK
        MirrorChars[0x27E2] = 0x27E3; // WHITE CONCAVE-SIDED DIAMOND WITH LEFTWARDS TICK
        MirrorChars[0x27E3] = 0x27E2; // WHITE CONCAVE-SIDED DIAMOND WITH RIGHTWARDS TICK
        MirrorChars[0x27E4] = 0x27E5; // WHITE SQUARE WITH LEFTWARDS TICK
        MirrorChars[0x27E5] = 0x27E4; // WHITE SQUARE WITH RIGHTWARDS TICK
        MirrorChars[0x27E6] = 0x27E7; // MATHEMATICAL LEFT WHITE SQUARE BRACKET
        MirrorChars[0x27E7] = 0x27E6; // MATHEMATICAL RIGHT WHITE SQUARE BRACKET
        MirrorChars[0x27E8] = 0x27E9; // MATHEMATICAL LEFT ANGLE BRACKET
        MirrorChars[0x27E9] = 0x27E8; // MATHEMATICAL RIGHT ANGLE BRACKET
        MirrorChars[0x27EA] = 0x27EB; // MATHEMATICAL LEFT DOUBLE ANGLE BRACKET
        MirrorChars[0x27EB] = 0x27EA; // MATHEMATICAL RIGHT DOUBLE ANGLE BRACKET
        MirrorChars[0x2983] = 0x2984; // LEFT WHITE CURLY BRACKET
        MirrorChars[0x2984] = 0x2983; // RIGHT WHITE CURLY BRACKET
        MirrorChars[0x2985] = 0x2986; // LEFT WHITE PARENTHESIS
        MirrorChars[0x2986] = 0x2985; // RIGHT WHITE PARENTHESIS
        MirrorChars[0x2987] = 0x2988; // Z NOTATION LEFT IMAGE BRACKET
        MirrorChars[0x2988] = 0x2987; // Z NOTATION RIGHT IMAGE BRACKET
        MirrorChars[0x2989] = 0x298A; // Z NOTATION LEFT BINDING BRACKET
        MirrorChars[0x298A] = 0x2989; // Z NOTATION RIGHT BINDING BRACKET
        MirrorChars[0x298B] = 0x298C; // LEFT SQUARE BRACKET WITH UNDERBAR
        MirrorChars[0x298C] = 0x298B; // RIGHT SQUARE BRACKET WITH UNDERBAR
        MirrorChars[0x298D] = 0x2990; // LEFT SQUARE BRACKET WITH TICK IN TOP CORNER
        MirrorChars[0x298E] = 0x298F; // RIGHT SQUARE BRACKET WITH TICK IN BOTTOM CORNER
        MirrorChars[0x298F] = 0x298E; // LEFT SQUARE BRACKET WITH TICK IN BOTTOM CORNER
        MirrorChars[0x2990] = 0x298D; // RIGHT SQUARE BRACKET WITH TICK IN TOP CORNER
        MirrorChars[0x2991] = 0x2992; // LEFT ANGLE BRACKET WITH DOT
        MirrorChars[0x2992] = 0x2991; // RIGHT ANGLE BRACKET WITH DOT
        MirrorChars[0x2993] = 0x2994; // LEFT ARC LESS-THAN BRACKET
        MirrorChars[0x2994] = 0x2993; // RIGHT ARC GREATER-THAN BRACKET
        MirrorChars[0x2995] = 0x2996; // DOUBLE LEFT ARC GREATER-THAN BRACKET
        MirrorChars[0x2996] = 0x2995; // DOUBLE RIGHT ARC LESS-THAN BRACKET
        MirrorChars[0x2997] = 0x2998; // LEFT BLACK TORTOISE SHELL BRACKET
        MirrorChars[0x2998] = 0x2997; // RIGHT BLACK TORTOISE SHELL BRACKET
        MirrorChars[0x29B8] = 0x2298; // CIRCLED REVERSE SOLIDUS
        MirrorChars[0x29C0] = 0x29C1; // CIRCLED LESS-THAN
        MirrorChars[0x29C1] = 0x29C0; // CIRCLED GREATER-THAN
        MirrorChars[0x29C4] = 0x29C5; // SQUARED RISING DIAGONAL SLASH
        MirrorChars[0x29C5] = 0x29C4; // SQUARED FALLING DIAGONAL SLASH
        MirrorChars[0x29CF] = 0x29D0; // LEFT TRIANGLE BESIDE VERTICAL BAR
        MirrorChars[0x29D0] = 0x29CF; // VERTICAL BAR BESIDE RIGHT TRIANGLE
        MirrorChars[0x29D1] = 0x29D2; // BOWTIE WITH LEFT HALF BLACK
        MirrorChars[0x29D2] = 0x29D1; // BOWTIE WITH RIGHT HALF BLACK
        MirrorChars[0x29D4] = 0x29D5; // TIMES WITH LEFT HALF BLACK
        MirrorChars[0x29D5] = 0x29D4; // TIMES WITH RIGHT HALF BLACK
        MirrorChars[0x29D8] = 0x29D9; // LEFT WIGGLY FENCE
        MirrorChars[0x29D9] = 0x29D8; // RIGHT WIGGLY FENCE
        MirrorChars[0x29DA] = 0x29DB; // LEFT DOUBLE WIGGLY FENCE
        MirrorChars[0x29DB] = 0x29DA; // RIGHT DOUBLE WIGGLY FENCE
        MirrorChars[0x29F5] = 0x2215; // REVERSE SOLIDUS OPERATOR
        MirrorChars[0x29F8] = 0x29F9; // BIG SOLIDUS
        MirrorChars[0x29F9] = 0x29F8; // BIG REVERSE SOLIDUS
        MirrorChars[0x29FC] = 0x29FD; // LEFT-POINTING CURVED ANGLE BRACKET
        MirrorChars[0x29FD] = 0x29FC; // RIGHT-POINTING CURVED ANGLE BRACKET
        MirrorChars[0x2A2B] = 0x2A2C; // MINUS SIGN WITH FALLING DOTS
        MirrorChars[0x2A2C] = 0x2A2B; // MINUS SIGN WITH RISING DOTS
        MirrorChars[0x2A2D] = 0x2A2C; // PLUS SIGN IN LEFT HALF CIRCLE
        MirrorChars[0x2A2E] = 0x2A2D; // PLUS SIGN IN RIGHT HALF CIRCLE
        MirrorChars[0x2A34] = 0x2A35; // MULTIPLICATION SIGN IN LEFT HALF CIRCLE
        MirrorChars[0x2A35] = 0x2A34; // MULTIPLICATION SIGN IN RIGHT HALF CIRCLE
        MirrorChars[0x2A3C] = 0x2A3D; // INTERIOR PRODUCT
        MirrorChars[0x2A3D] = 0x2A3C; // RIGHTHAND INTERIOR PRODUCT
        MirrorChars[0x2A64] = 0x2A65; // Z NOTATION DOMAIN ANTIRESTRICTION
        MirrorChars[0x2A65] = 0x2A64; // Z NOTATION RANGE ANTIRESTRICTION
        MirrorChars[0x2A79] = 0x2A7A; // LESS-THAN WITH CIRCLE INSIDE
        MirrorChars[0x2A7A] = 0x2A79; // GREATER-THAN WITH CIRCLE INSIDE
        MirrorChars[0x2A7D] = 0x2A7E; // LESS-THAN OR SLANTED EQUAL TO
        MirrorChars[0x2A7E] = 0x2A7D; // GREATER-THAN OR SLANTED EQUAL TO
        MirrorChars[0x2A7F] = 0x2A80; // LESS-THAN OR SLANTED EQUAL TO WITH DOT INSIDE
        MirrorChars[0x2A80] = 0x2A7F; // GREATER-THAN OR SLANTED EQUAL TO WITH DOT INSIDE
        MirrorChars[0x2A81] = 0x2A82; // LESS-THAN OR SLANTED EQUAL TO WITH DOT ABOVE
        MirrorChars[0x2A82] = 0x2A81; // GREATER-THAN OR SLANTED EQUAL TO WITH DOT ABOVE
        MirrorChars[0x2A83] = 0x2A84; // LESS-THAN OR SLANTED EQUAL TO WITH DOT ABOVE RIGHT
        MirrorChars[0x2A84] = 0x2A83; // GREATER-THAN OR SLANTED EQUAL TO WITH DOT ABOVE LEFT
        MirrorChars[0x2A8B] = 0x2A8C; // LESS-THAN ABOVE DOUBLE-LINE EQUAL ABOVE GREATER-THAN
        MirrorChars[0x2A8C] = 0x2A8B; // GREATER-THAN ABOVE DOUBLE-LINE EQUAL ABOVE LESS-THAN
        MirrorChars[0x2A91] = 0x2A92; // LESS-THAN ABOVE GREATER-THAN ABOVE DOUBLE-LINE EQUAL
        MirrorChars[0x2A92] = 0x2A91; // GREATER-THAN ABOVE LESS-THAN ABOVE DOUBLE-LINE EQUAL
        MirrorChars[0x2A93] = 0x2A94; // LESS-THAN ABOVE SLANTED EQUAL ABOVE GREATER-THAN ABOVE SLANTED EQUAL
        MirrorChars[0x2A94] = 0x2A93; // GREATER-THAN ABOVE SLANTED EQUAL ABOVE LESS-THAN ABOVE SLANTED EQUAL
        MirrorChars[0x2A95] = 0x2A96; // SLANTED EQUAL TO OR LESS-THAN
        MirrorChars[0x2A96] = 0x2A95; // SLANTED EQUAL TO OR GREATER-THAN
        MirrorChars[0x2A97] = 0x2A98; // SLANTED EQUAL TO OR LESS-THAN WITH DOT INSIDE
        MirrorChars[0x2A98] = 0x2A97; // SLANTED EQUAL TO OR GREATER-THAN WITH DOT INSIDE
        MirrorChars[0x2A99] = 0x2A9A; // DOUBLE-LINE EQUAL TO OR LESS-THAN
        MirrorChars[0x2A9A] = 0x2A99; // DOUBLE-LINE EQUAL TO OR GREATER-THAN
        MirrorChars[0x2A9B] = 0x2A9C; // DOUBLE-LINE SLANTED EQUAL TO OR LESS-THAN
        MirrorChars[0x2A9C] = 0x2A9B; // DOUBLE-LINE SLANTED EQUAL TO OR GREATER-THAN
        MirrorChars[0x2AA1] = 0x2AA2; // DOUBLE NESTED LESS-THAN
        MirrorChars[0x2AA2] = 0x2AA1; // DOUBLE NESTED GREATER-THAN
        MirrorChars[0x2AA6] = 0x2AA7; // LESS-THAN CLOSED BY CURVE
        MirrorChars[0x2AA7] = 0x2AA6; // GREATER-THAN CLOSED BY CURVE
        MirrorChars[0x2AA8] = 0x2AA9; // LESS-THAN CLOSED BY CURVE ABOVE SLANTED EQUAL
        MirrorChars[0x2AA9] = 0x2AA8; // GREATER-THAN CLOSED BY CURVE ABOVE SLANTED EQUAL
        MirrorChars[0x2AAA] = 0x2AAB; // SMALLER THAN
        MirrorChars[0x2AAB] = 0x2AAA; // LARGER THAN
        MirrorChars[0x2AAC] = 0x2AAD; // SMALLER THAN OR EQUAL TO
        MirrorChars[0x2AAD] = 0x2AAC; // LARGER THAN OR EQUAL TO
        MirrorChars[0x2AAF] = 0x2AB0; // PRECEDES ABOVE SINGLE-LINE EQUALS SIGN
        MirrorChars[0x2AB0] = 0x2AAF; // SUCCEEDS ABOVE SINGLE-LINE EQUALS SIGN
        MirrorChars[0x2AB3] = 0x2AB4; // PRECEDES ABOVE EQUALS SIGN
        MirrorChars[0x2AB4] = 0x2AB3; // SUCCEEDS ABOVE EQUALS SIGN
        MirrorChars[0x2ABB] = 0x2ABC; // DOUBLE PRECEDES
        MirrorChars[0x2ABC] = 0x2ABB; // DOUBLE SUCCEEDS
        MirrorChars[0x2ABD] = 0x2ABE; // SUBSET WITH DOT
        MirrorChars[0x2ABE] = 0x2ABD; // SUPERSET WITH DOT
        MirrorChars[0x2ABF] = 0x2AC0; // SUBSET WITH PLUS SIGN BELOW
        MirrorChars[0x2AC0] = 0x2ABF; // SUPERSET WITH PLUS SIGN BELOW
        MirrorChars[0x2AC1] = 0x2AC2; // SUBSET WITH MULTIPLICATION SIGN BELOW
        MirrorChars[0x2AC2] = 0x2AC1; // SUPERSET WITH MULTIPLICATION SIGN BELOW
        MirrorChars[0x2AC3] = 0x2AC4; // SUBSET OF OR EQUAL TO WITH DOT ABOVE
        MirrorChars[0x2AC4] = 0x2AC3; // SUPERSET OF OR EQUAL TO WITH DOT ABOVE
        MirrorChars[0x2AC5] = 0x2AC6; // SUBSET OF ABOVE EQUALS SIGN
        MirrorChars[0x2AC6] = 0x2AC5; // SUPERSET OF ABOVE EQUALS SIGN
        MirrorChars[0x2ACD] = 0x2ACE; // SQUARE LEFT OPEN BOX OPERATOR
        MirrorChars[0x2ACE] = 0x2ACD; // SQUARE RIGHT OPEN BOX OPERATOR
        MirrorChars[0x2ACF] = 0x2AD0; // CLOSED SUBSET
        MirrorChars[0x2AD0] = 0x2ACF; // CLOSED SUPERSET
        MirrorChars[0x2AD1] = 0x2AD2; // CLOSED SUBSET OR EQUAL TO
        MirrorChars[0x2AD2] = 0x2AD1; // CLOSED SUPERSET OR EQUAL TO
        MirrorChars[0x2AD3] = 0x2AD4; // SUBSET ABOVE SUPERSET
        MirrorChars[0x2AD4] = 0x2AD3; // SUPERSET ABOVE SUBSET
        MirrorChars[0x2AD5] = 0x2AD6; // SUBSET ABOVE SUBSET
        MirrorChars[0x2AD6] = 0x2AD5; // SUPERSET ABOVE SUPERSET
        MirrorChars[0x2ADE] = 0x22A6; // SHORT LEFT TACK
        MirrorChars[0x2AE3] = 0x22A9; // DOUBLE VERTICAL BAR LEFT TURNSTILE
        MirrorChars[0x2AE4] = 0x22A8; // VERTICAL BAR DOUBLE LEFT TURNSTILE
        MirrorChars[0x2AE5] = 0x22AB; // DOUBLE VERTICAL BAR DOUBLE LEFT TURNSTILE
        MirrorChars[0x2AEC] = 0x2AED; // DOUBLE STROKE NOT SIGN
        MirrorChars[0x2AED] = 0x2AEC; // REVERSED DOUBLE STROKE NOT SIGN
        MirrorChars[0x2AF7] = 0x2AF8; // TRIPLE NESTED LESS-THAN
        MirrorChars[0x2AF8] = 0x2AF7; // TRIPLE NESTED GREATER-THAN
        MirrorChars[0x2AF9] = 0x2AFA; // DOUBLE-LINE SLANTED LESS-THAN OR EQUAL TO
        MirrorChars[0x2AFA] = 0x2AF9; // DOUBLE-LINE SLANTED GREATER-THAN OR EQUAL TO
        MirrorChars[0x3008] = 0x3009; // LEFT ANGLE BRACKET
        MirrorChars[0x3009] = 0x3008; // RIGHT ANGLE BRACKET
        MirrorChars[0x300A] = 0x300B; // LEFT DOUBLE ANGLE BRACKET
        MirrorChars[0x300B] = 0x300A; // RIGHT DOUBLE ANGLE BRACKET
        MirrorChars[0x300C] = 0x300D; // [BEST FIT] LEFT CORNER BRACKET
        MirrorChars[0x300D] = 0x300C; // [BEST FIT] RIGHT CORNER BRACKET
        MirrorChars[0x300E] = 0x300F; // [BEST FIT] LEFT WHITE CORNER BRACKET
        MirrorChars[0x300F] = 0x300E; // [BEST FIT] RIGHT WHITE CORNER BRACKET
        MirrorChars[0x3010] = 0x3011; // LEFT BLACK LENTICULAR BRACKET
        MirrorChars[0x3011] = 0x3010; // RIGHT BLACK LENTICULAR BRACKET
        MirrorChars[0x3014] = 0x3015; // LEFT TORTOISE SHELL BRACKET
        MirrorChars[0x3015] = 0x3014; // RIGHT TORTOISE SHELL BRACKET
        MirrorChars[0x3016] = 0x3017; // LEFT WHITE LENTICULAR BRACKET
        MirrorChars[0x3017] = 0x3016; // RIGHT WHITE LENTICULAR BRACKET
        MirrorChars[0x3018] = 0x3019; // LEFT WHITE TORTOISE SHELL BRACKET
        MirrorChars[0x3019] = 0x3018; // RIGHT WHITE TORTOISE SHELL BRACKET
        MirrorChars[0x301A] = 0x301B; // LEFT WHITE SQUARE BRACKET
        MirrorChars[0x301B] = 0x301A; // RIGHT WHITE SQUARE BRACKET
        MirrorChars[0xFF08] = 0xFF09; // FULLWIDTH LEFT PARENTHESIS
        MirrorChars[0xFF09] = 0xFF08; // FULLWIDTH RIGHT PARENTHESIS
        MirrorChars[0xFF1C] = 0xFF1E; // FULLWIDTH LESS-THAN SIGN
        MirrorChars[0xFF1E] = 0xFF1C; // FULLWIDTH GREATER-THAN SIGN
        MirrorChars[0xFF3B] = 0xFF3D; // FULLWIDTH LEFT SQUARE BRACKET
        MirrorChars[0xFF3D] = 0xFF3B; // FULLWIDTH RIGHT SQUARE BRACKET
        MirrorChars[0xFF5B] = 0xFF5D; // FULLWIDTH LEFT CURLY BRACKET
        MirrorChars[0xFF5D] = 0xFF5B; // FULLWIDTH RIGHT CURLY BRACKET
        MirrorChars[0xFF5F] = 0xFF60; // FULLWIDTH LEFT WHITE PARENTHESIS
        MirrorChars[0xFF60] = 0xFF5F; // FULLWIDTH RIGHT WHITE PARENTHESIS
        MirrorChars[0xFF62] = 0xFF63; // [BEST FIT] HALFWIDTH LEFT CORNER BRACKET
        MirrorChars[0xFF63] = 0xFF62; // [BEST FIT] HALFWIDTH RIGHT CORNER BRACKET
    }

    /// <summary>
    ///     Creates new BidiLine
    /// </summary>
    public BidiLine()
    {
    }

    public BidiLine(BidiLine org)
    {
        if (org == null)
        {
            throw new ArgumentNullException(nameof(org));
        }

        RunDirection = org.RunDirection;
        PieceSize = org.PieceSize;
        Text = (char[])org.Text.Clone();
        DetailChunks = (PdfChunk[])org.DetailChunks.Clone();
        TotalTextLength = org.TotalTextLength;

        OrderLevels = (byte[])org.OrderLevels.Clone();
        IndexChars = (int[])org.IndexChars.Clone();

        Chunks = new List<PdfChunk>(org.Chunks);
        IndexChunk = org.IndexChunk;
        IndexChunkChar = org.IndexChunkChar;
        CurrentChar = org.CurrentChar;

        StoredRunDirection = org.StoredRunDirection;
        StoredText = (char[])org.StoredText.Clone();
        StoredDetailChunks = (PdfChunk[])org.StoredDetailChunks.Clone();
        StoredTotalTextLength = org.StoredTotalTextLength;

        StoredOrderLevels = (byte[])org.StoredOrderLevels.Clone();
        StoredIndexChars = (int[])org.StoredIndexChars.Clone();

        StoredIndexChunk = org.StoredIndexChunk;
        StoredIndexChunkChar = org.StoredIndexChunkChar;
        StoredCurrentChar = org.StoredCurrentChar;

        ShortStore = org.ShortStore;
        ArabicOptions = org.ArabicOptions;
    }

    public static bool IsWs(char c) => c <= ' ';

    public void AddChunk(PdfChunk chunk)
    {
        Chunks.Add(chunk);
    }

    public void AddChunks(IList<PdfChunk> chunks)
    {
        Chunks.AddRange(chunks);
    }

    public void AddPiece(char c, PdfChunk chunk)
    {
        if (TotalTextLength >= PieceSize)
        {
            var tempText = Text;
            var tempDetailChunks = DetailChunks;
            PieceSize *= 2;
            Text = new char[PieceSize];
            DetailChunks = new PdfChunk[PieceSize];
            Array.Copy(tempText, 0, Text, 0, TotalTextLength);
            Array.Copy(tempDetailChunks, 0, DetailChunks, 0, TotalTextLength);
        }

        Text[TotalTextLength] = c;
        DetailChunks[TotalTextLength++] = chunk;
    }

    public void ClearChunks()
    {
        Chunks.Clear();
        TotalTextLength = 0;
        CurrentChar = 0;
    }

    public IList<PdfChunk> CreateArrayOfPdfChunks(int startIdx, int endIdx) =>
        CreateArrayOfPdfChunks(startIdx, endIdx, null);

    public IList<PdfChunk> CreateArrayOfPdfChunks(int startIdx, int endIdx, PdfChunk extraPdfChunk)
    {
        var bidi = RunDirection == PdfWriter.RUN_DIRECTION_LTR || RunDirection == PdfWriter.RUN_DIRECTION_RTL;
        if (bidi)
        {
            Reorder(startIdx, endIdx);
        }

        List<PdfChunk> ar = new();
        var refCk = DetailChunks[startIdx];
        PdfChunk ck = null;
        var buf = new StringBuilder();
        char c;
        var idx = 0;
        for (; startIdx <= endIdx; ++startIdx)
        {
            idx = bidi ? IndexChars[startIdx] : startIdx;
            c = Text[idx];
            ck = DetailChunks[idx];
            if (PdfChunk.NoPrint(ck.GetUnicodeEquivalent(c)))
            {
                continue;
            }

            if (ck.IsImage() || ck.IsSeparator() || ck.IsTab())
            {
                if (buf.Length > 0)
                {
                    ar.Add(new PdfChunk(buf.ToString(), refCk));
                    buf = new StringBuilder();
                }

                ar.Add(ck);
            }
            else if (ck == refCk)
            {
                buf.Append(c);
            }
            else
            {
                if (buf.Length > 0)
                {
                    ar.Add(new PdfChunk(buf.ToString(), refCk));
                    buf = new StringBuilder();
                }

                if (!ck.IsImage() && !ck.IsSeparator() && !ck.IsTab())
                {
                    buf.Append(c);
                }

                refCk = ck;
            }
        }

        if (buf.Length > 0)
        {
            ar.Add(new PdfChunk(buf.ToString(), refCk));
        }

        if (extraPdfChunk != null)
        {
            ar.Add(extraPdfChunk);
        }

        return ar;
    }

    public void DoArabicShapping()
    {
        var src = 0;
        var dest = 0;
        for (;;)
        {
            while (src < TotalTextLength)
            {
                var c = Text[src];
                if (c >= 0x0600 && c <= 0x06ff)
                {
                    break;
                }

                if (src != dest)
                {
                    Text[dest] = Text[src];
                    DetailChunks[dest] = DetailChunks[src];
                    OrderLevels[dest] = OrderLevels[src];
                }

                ++src;
                ++dest;
            }

            if (src >= TotalTextLength)
            {
                TotalTextLength = dest;
                return;
            }

            var startArabicIdx = src;
            ++src;
            while (src < TotalTextLength)
            {
                var c = Text[src];
                if (c < 0x0600 || c > 0x06ff)
                {
                    break;
                }

                ++src;
            }

            var arabicWordSize = src - startArabicIdx;
            var size = ArabicLigaturizer.Arabic_shape(Text, startArabicIdx, arabicWordSize, Text, dest, arabicWordSize,
                                                      ArabicOptions);
            if (startArabicIdx != dest)
            {
                for (var k = 0; k < size; ++k)
                {
                    DetailChunks[dest] = DetailChunks[startArabicIdx];
                    OrderLevels[dest++] = OrderLevels[startArabicIdx++];
                }
            }
            else
            {
                dest += size;
            }
        }
    }

    public void Flip(int start, int end)
    {
        var mid = (start + end) / 2;
        --end;
        for (; start < mid; ++start, --end)
        {
            var temp = IndexChars[start];
            IndexChars[start] = IndexChars[end];
            IndexChars[end] = temp;
        }
    }

    public bool GetParagraph(int runDirection)
    {
        RunDirection = runDirection;
        CurrentChar = 0;
        TotalTextLength = 0;
        var hasText = false;
        char c;
        char uniC;
        BaseFont bf;
        for (; IndexChunk < Chunks.Count; ++IndexChunk)
        {
            var ck = Chunks[IndexChunk];
            bf = ck.Font.Font;
            var s = ck.ToString();
            var len = s.Length;
            for (; IndexChunkChar < len; ++IndexChunkChar)
            {
                c = s[IndexChunkChar];
                uniC = (char)bf.GetUnicodeEquivalent(c);
                if (uniC == '\r' || uniC == '\n')
                {
                    // next condition is never true for CID
                    if (uniC == '\r' && IndexChunkChar + 1 < len && s[IndexChunkChar + 1] == '\n')
                    {
                        ++IndexChunkChar;
                    }

                    ++IndexChunkChar;
                    if (IndexChunkChar >= len)
                    {
                        IndexChunkChar = 0;
                        ++IndexChunk;
                    }

                    hasText = true;
                    if (TotalTextLength == 0)
                    {
                        DetailChunks[0] = ck;
                    }

                    break;
                }

                AddPiece(c, ck);
            }

            if (hasText)
            {
                break;
            }

            IndexChunkChar = 0;
        }

        if (TotalTextLength == 0)
        {
            return hasText;
        }

        // remove trailing WS
        TotalTextLength = TrimRight(0, TotalTextLength - 1) + 1;
        if (TotalTextLength == 0)
        {
            return true;
        }

        if (runDirection == PdfWriter.RUN_DIRECTION_LTR || runDirection == PdfWriter.RUN_DIRECTION_RTL)
        {
            if (OrderLevels.Length < TotalTextLength)
            {
                OrderLevels = new byte[PieceSize];
                IndexChars = new int[PieceSize];
            }

            ArabicLigaturizer.ProcessNumbers(Text, 0, TotalTextLength, ArabicOptions);
            var order = new BidiOrder(Text, 0, TotalTextLength,
                                      (sbyte)(runDirection == PdfWriter.RUN_DIRECTION_RTL ? 1 : 0));
            var od = order.GetLevels();
            for (var k = 0; k < TotalTextLength; ++k)
            {
                OrderLevels[k] = od[k];
                IndexChars[k] = k;
            }

            DoArabicShapping();
            MirrorGlyphs();
        }

        TotalTextLength = TrimRightEx(0, TotalTextLength - 1) + 1;
        return true;
    }

    /// <summary>
    ///     Gets the width of a range of characters.
    /// </summary>
    /// <param name="startIdx">the first index to calculate</param>
    /// <param name="lastIdx">the last inclusive index to calculate</param>
    /// <returns>the sum of all widths</returns>
    public float GetWidth(int startIdx, int lastIdx)
    {
        var c = (char)0;
        PdfChunk ck = null;
        float width = 0;
        for (; startIdx <= lastIdx; ++startIdx)
        {
            var surrogate = Utilities.IsSurrogatePair(Text, startIdx);
            if (surrogate)
            {
                width += DetailChunks[startIdx].GetCharWidth(Utilities.ConvertToUtf32(Text, startIdx));
                ++startIdx;
            }
            else
            {
                c = Text[startIdx];
                ck = DetailChunks[startIdx];
                if (PdfChunk.NoPrint(ck.GetUnicodeEquivalent(c)))
                {
                    continue;
                }

                width += DetailChunks[startIdx].GetCharWidth(c);
            }
        }

        return width;
    }

    public int[] GetWord(int startIdx, int idx)
    {
        var last = idx;
        var first = idx;
        // forward
        for (; last < TotalTextLength; ++last)
        {
            if (!char.IsLetter(Text[last]))
            {
                break;
            }
        }

        if (last == idx)
        {
            return null;
        }

        // backward
        for (; first >= startIdx; --first)
        {
            if (!char.IsLetter(Text[first]))
            {
                break;
            }
        }

        ++first;
        return new[] { first, last };
    }

    public bool IsEmpty() => CurrentChar >= TotalTextLength && IndexChunk >= Chunks.Count;

    public void MirrorGlyphs()
    {
        for (var k = 0; k < TotalTextLength; ++k)
        {
            if ((OrderLevels[k] & 1) == 1)
            {
                var mirror = MirrorChars[Text[k]];
                if (mirror != 0)
                {
                    Text[k] = (char)mirror;
                }
            }
        }
    }

    public PdfLine ProcessLine(float leftX, float width, int alignment, int runDirection, int arabicOptions)
    {
        ArabicOptions = arabicOptions;
        Save();
        var isRtl = runDirection == PdfWriter.RUN_DIRECTION_RTL;
        if (CurrentChar >= TotalTextLength)
        {
            var hasText = GetParagraph(runDirection);
            if (!hasText)
            {
                return null;
            }

            if (TotalTextLength == 0)
            {
                List<PdfChunk> ar = new();
                var ckx = new PdfChunk("", DetailChunks[0]);
                ar.Add(ckx);
                return new PdfLine(0, 0, 0, alignment, true, ar, isRtl);
            }
        }

        var originalWidth = width;
        var lastSplit = -1;
        if (CurrentChar != 0)
        {
            CurrentChar = TrimLeftEx(CurrentChar, TotalTextLength - 1);
        }

        var oldCurrentChar = CurrentChar;
        var uniC = 0;
        PdfChunk ck = null;
        float charWidth = 0;
        PdfChunk lastValidChunk = null;
        var splitChar = false;
        var surrogate = false;
        for (; CurrentChar < TotalTextLength; ++CurrentChar)
        {
            ck = DetailChunks[CurrentChar];
            surrogate = Utilities.IsSurrogatePair(Text, CurrentChar);
            if (surrogate)
            {
                uniC = ck.GetUnicodeEquivalent(Utilities.ConvertToUtf32(Text, CurrentChar));
            }
            else
            {
                uniC = ck.GetUnicodeEquivalent(Text[CurrentChar]);
            }

            if (PdfChunk.NoPrint(uniC))
            {
                continue;
            }

            if (surrogate)
            {
                charWidth = ck.GetCharWidth(uniC);
            }
            else
            {
                charWidth = ck.GetCharWidth(Text[CurrentChar]);
            }

            splitChar = ck.IsExtSplitCharacter(oldCurrentChar, CurrentChar, TotalTextLength, Text, DetailChunks);
            if (splitChar && char.IsWhiteSpace((char)uniC))
            {
                lastSplit = CurrentChar;
            }

            if (width - charWidth < 0)
            {
                break;
            }

            if (splitChar)
            {
                lastSplit = CurrentChar;
            }

            width -= charWidth;
            lastValidChunk = ck;
            if (surrogate)
            {
                ++CurrentChar;
            }

            if (ck.IsTab())
            {
                var tab = (object[])ck.GetAttribute(Chunk.TAB);
                var tabPosition = (float)tab[1];
                var newLine = (bool)tab[2];
                if (newLine && tabPosition < originalWidth - width)
                {
                    return new PdfLine(0, originalWidth, width, alignment, true,
                                       CreateArrayOfPdfChunks(oldCurrentChar, CurrentChar - 1), isRtl);
                }

                DetailChunks[CurrentChar].AdjustLeft(leftX);
                width = originalWidth - tabPosition;
            }
        }

        if (lastValidChunk == null)
        {
            // not even a single char fit; must output the first char
            ++CurrentChar;
            if (surrogate)
            {
                ++CurrentChar;
            }

            return new PdfLine(0, originalWidth, 0, alignment, false,
                               CreateArrayOfPdfChunks(CurrentChar - 1, CurrentChar - 1), isRtl);
        }

        if (CurrentChar >= TotalTextLength)
        {
            // there was more line than text
            return new PdfLine(0, originalWidth, width, alignment, true,
                               CreateArrayOfPdfChunks(oldCurrentChar, TotalTextLength - 1), isRtl);
        }

        var newCurrentChar = TrimRightEx(oldCurrentChar, CurrentChar - 1);
        if (newCurrentChar < oldCurrentChar)
        {
            // only WS
            return new PdfLine(0, originalWidth, width, alignment, false,
                               CreateArrayOfPdfChunks(oldCurrentChar, CurrentChar - 1), isRtl);
        }

        if (newCurrentChar == CurrentChar - 1)
        {
            // middle of word
            var he = (IHyphenationEvent)lastValidChunk.GetAttribute(Chunk.HYPHENATION);
            if (he != null)
            {
                var word = GetWord(oldCurrentChar, newCurrentChar);
                if (word != null)
                {
                    var testWidth = width + GetWidth(word[0], CurrentChar - 1);
                    var pre = he.GetHyphenatedWordPre(new string(Text, word[0], word[1] - word[0]),
                                                      lastValidChunk.Font.Font, lastValidChunk.Font.Size, testWidth);
                    var post = he.HyphenatedWordPost;
                    if (pre.Length > 0)
                    {
                        var extra = new PdfChunk(pre, lastValidChunk);
                        CurrentChar = word[1] - post.Length;
                        return new PdfLine(0, originalWidth, testWidth - lastValidChunk.Font.Width(pre), alignment,
                                           false, CreateArrayOfPdfChunks(oldCurrentChar, word[0] - 1, extra), isRtl);
                    }
                }
            }
        }

        if (lastSplit == -1 || lastSplit >= newCurrentChar)
        {
            // no split point or split point ahead of end
            return new PdfLine(0, originalWidth, width + GetWidth(newCurrentChar + 1, CurrentChar - 1), alignment,
                               false, CreateArrayOfPdfChunks(oldCurrentChar, newCurrentChar), isRtl);
        }

        // standard split
        CurrentChar = lastSplit + 1;
        newCurrentChar = TrimRightEx(oldCurrentChar, lastSplit);
        if (newCurrentChar < oldCurrentChar)
        {
            // only WS again
            newCurrentChar = CurrentChar - 1;
        }

        return new PdfLine(0, originalWidth, originalWidth - GetWidth(oldCurrentChar, newCurrentChar), alignment, false,
                           CreateArrayOfPdfChunks(oldCurrentChar, newCurrentChar), isRtl);
    }

    public void Reorder(int start, int end)
    {
        var maxLevel = OrderLevels[start];
        var minLevel = maxLevel;
        var onlyOddLevels = maxLevel;
        var onlyEvenLevels = maxLevel;
        for (var k = start + 1; k <= end; ++k)
        {
            var b = OrderLevels[k];
            if (b > maxLevel)
            {
                maxLevel = b;
            }
            else if (b < minLevel)
            {
                minLevel = b;
            }

            onlyOddLevels &= b;
            onlyEvenLevels |= b;
        }

        if ((onlyEvenLevels & 1) == 0) // nothing to do
        {
            return;
        }

        if ((onlyOddLevels & 1) == 1)
        {
            // single inversion
            Flip(start, end + 1);
            return;
        }

        minLevel |= 1;
        for (; maxLevel >= minLevel; --maxLevel)
        {
            var pstart = start;
            for (;;)
            {
                for (; pstart <= end; ++pstart)
                {
                    if (OrderLevels[pstart] >= maxLevel)
                    {
                        break;
                    }
                }

                if (pstart > end)
                {
                    break;
                }

                var pend = pstart + 1;
                for (; pend <= end; ++pend)
                {
                    if (OrderLevels[pend] < maxLevel)
                    {
                        break;
                    }
                }

                Flip(pstart, pend);
                pstart = pend + 1;
            }
        }
    }

    public void Restore()
    {
        RunDirection = StoredRunDirection;
        TotalTextLength = StoredTotalTextLength;
        IndexChunk = StoredIndexChunk;
        IndexChunkChar = StoredIndexChunkChar;
        CurrentChar = StoredCurrentChar;
        if (!ShortStore)
        {
            // long restore
            Array.Copy(StoredText, 0, Text, 0, TotalTextLength);
            Array.Copy(StoredDetailChunks, 0, DetailChunks, 0, TotalTextLength);
        }

        if (RunDirection == PdfWriter.RUN_DIRECTION_LTR || RunDirection == PdfWriter.RUN_DIRECTION_RTL)
        {
            Array.Copy(StoredOrderLevels, CurrentChar, OrderLevels, CurrentChar, TotalTextLength - CurrentChar);
            Array.Copy(StoredIndexChars, CurrentChar, IndexChars, CurrentChar, TotalTextLength - CurrentChar);
        }
    }

    public void Save()
    {
        if (IndexChunk > 0)
        {
            if (IndexChunk >= Chunks.Count)
            {
                Chunks.Clear();
            }
            else
            {
                for (--IndexChunk; IndexChunk >= 0; --IndexChunk)
                {
                    Chunks.RemoveAt(IndexChunk);
                }
            }

            IndexChunk = 0;
        }

        StoredRunDirection = RunDirection;
        StoredTotalTextLength = TotalTextLength;
        StoredIndexChunk = IndexChunk;
        StoredIndexChunkChar = IndexChunkChar;
        StoredCurrentChar = CurrentChar;
        ShortStore = CurrentChar < TotalTextLength;
        if (!ShortStore)
        {
            // long save
            if (StoredText.Length < TotalTextLength)
            {
                StoredText = new char[TotalTextLength];
                StoredDetailChunks = new PdfChunk[TotalTextLength];
            }

            Array.Copy(Text, 0, StoredText, 0, TotalTextLength);
            Array.Copy(DetailChunks, 0, StoredDetailChunks, 0, TotalTextLength);
        }

        if (RunDirection == PdfWriter.RUN_DIRECTION_LTR || RunDirection == PdfWriter.RUN_DIRECTION_RTL)
        {
            if (StoredOrderLevels.Length < TotalTextLength)
            {
                StoredOrderLevels = new byte[TotalTextLength];
                StoredIndexChars = new int[TotalTextLength];
            }

            Array.Copy(OrderLevels, CurrentChar, StoredOrderLevels, CurrentChar, TotalTextLength - CurrentChar);
            Array.Copy(IndexChars, CurrentChar, StoredIndexChars, CurrentChar, TotalTextLength - CurrentChar);
        }
    }

    public int TrimLeft(int startIdx, int endIdx)
    {
        var idx = startIdx;
        char c;
        for (; idx <= endIdx; ++idx)
        {
            c = (char)DetailChunks[idx].GetUnicodeEquivalent(Text[idx]);
            if (!IsWs(c))
            {
                break;
            }
        }

        return idx;
    }

    public int TrimLeftEx(int startIdx, int endIdx)
    {
        var idx = startIdx;
        var c = (char)0;
        for (; idx <= endIdx; ++idx)
        {
            c = (char)DetailChunks[idx].GetUnicodeEquivalent(Text[idx]);
            if (!IsWs(c) && !PdfChunk.NoPrint(c))
            {
                break;
            }
        }

        return idx;
    }

    public int TrimRight(int startIdx, int endIdx)
    {
        var idx = endIdx;
        char c;
        for (; idx >= startIdx; --idx)
        {
            c = (char)DetailChunks[idx].GetUnicodeEquivalent(Text[idx]);
            if (!IsWs(c))
            {
                break;
            }
        }

        return idx;
    }

    public int TrimRightEx(int startIdx, int endIdx)
    {
        var idx = endIdx;
        var c = (char)0;
        for (; idx >= startIdx; --idx)
        {
            c = (char)DetailChunks[idx].GetUnicodeEquivalent(Text[idx]);
            if (!IsWs(c) && !PdfChunk.NoPrint(c))
            {
                break;
            }
        }

        return idx;
    }
}