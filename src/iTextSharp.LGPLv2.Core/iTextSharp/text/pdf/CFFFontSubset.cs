using System.util;

namespace iTextSharp.text.pdf;

/// <summary>
/// </summary>
public class CffFontSubset : CffFont
{
    /// <summary>
    ///     Operator codes for unused  CharStrings and unused local and global Subrs
    /// </summary>
    internal const byte ENDCHAR_OP = 14;

    internal const byte RETURN_OP = 11;

    /// <summary>
    ///     The Strings in this array represent Type1/Type2 escape operator names
    /// </summary>
    internal static readonly string[] SubrsEscapeFuncs =
    {
        "RESERVED_0", "RESERVED_1", "RESERVED_2", "and", "or", "not",
        "RESERVED_6",
        "RESERVED_7", "RESERVED_8", "abs", "add", "sub", "div",
        "RESERVED_13", "neg",
        "eq", "RESERVED_16", "RESERVED_17", "drop", "RESERVED_19", "put",
        "get", "ifelse",
        "random", "mul", "RESERVED_25", "sqrt", "dup", "exch", "index",
        "roll", "RESERVED_31",
        "RESERVED_32", "RESERVED_33", "hflex", "flex", "hflex1", "flex1",
        "RESERVED_REST",
    };

    /// <summary>
    ///     The Strings in this array represent Type1/Type2 operator names
    /// </summary>
    internal static readonly string[] SubrsFunctions =
    {
        "RESERVED_0", "hstem", "RESERVED_2", "vstem", "vmoveto", "rlineto",
        "hlineto", "vlineto",
        "rrcurveto", "RESERVED_9", "callsubr", "return", "escape",
        "RESERVED_13",
        "endchar", "RESERVED_15", "RESERVED_16", "RESERVED_17", "hstemhm",
        "hintmask",
        "cntrmask", "rmoveto", "hmoveto", "vstemhm", "rcurveline",
        "rlinecurve", "vvcurveto",
        "hhcurveto", "shortint", "callgsubr", "vhcurveto", "hvcurveto",
    };

    /// <summary>
    ///     A HashMap for keeping the FDArrays being used by the font
    /// </summary>
    internal readonly INullValueDictionary<int, object> FdArrayUsed = new NullValueDictionary<int, object>();

    /// <summary>
    ///     The bias for the global subroutines
    /// </summary>
    internal int GBias;

    /// <summary>
    ///     The GlyphsUsed keys as an ArrayList
    /// </summary>
    internal readonly List<int> GlyphsInList;

    /// <summary>
    ///     A HashMap containing the glyphs used in the text after being converted
    ///     to glyph number by the CMap
    /// </summary>
    internal readonly INullValueDictionary<int, int[]> GlyphsUsed;

    /// <summary>
    ///     A HashMap for keeping the Global subroutines used in the font
    /// </summary>
    internal readonly INullValueDictionary<int, int[]> HGSubrsUsed = new NullValueDictionary<int, int[]>();

    /// <summary>
    ///     A HashMaps array for keeping the subroutines used in each FontDict
    /// </summary>
    internal INullValueDictionary<int, int[]>[] HSubrsUsed;

    /// <summary>
    ///     A HashMap for keeping the subroutines used in a non-cid font
    /// </summary>
    internal readonly INullValueDictionary<int, int[]> HSubrsUsedNonCid = new NullValueDictionary<int, int[]>();

    /// <summary>
    ///     The Global SubroutinesUsed HashMaps as ArrayLists
    /// </summary>
    internal readonly List<int> LGSubrsUsed = new();

    /// <summary>
    ///     The SubroutinesUsed HashMaps as ArrayLists
    /// </summary>
    internal List<int>[] LSubrsUsed;

    /// <summary>
    ///     The SubroutinesUsed HashMap as ArrayList
    /// </summary>
    internal readonly List<int> LSubrsUsedNonCid = new();

    /// <summary>
    ///     The new CharString of the font
    /// </summary>
    internal byte[] NewCharStringsIndex;

    /// <summary>
    ///     The new global subroutines index of the font
    /// </summary>
    internal byte[] NewGSubrsIndex;

    /// <summary>
    ///     An array of the new Indexs for the local Subr. One index for each FontDict
    /// </summary>
    internal byte[][] NewLSubrsIndex;

    /// <summary>
    ///     The new subroutines index for a non-cid font
    /// </summary>
    internal byte[] NewSubrsIndexNonCid;

    /// <summary>
    ///     Number of arguments to the stem operators in a subroutine calculated recursivly
    /// </summary>
    internal int NumOfHints;

    /// <summary>
    ///     The linked list for generating the new font stream
    /// </summary>
    internal List<Item> OutputList;

    /// <summary>
    ///     C'tor for CFFFontSubset
    /// </summary>
    /// <param name="rf">- The font file</param>
    /// <param name="glyphsUsed">- a HashMap that contains the glyph used in the subset</param>
    public CffFontSubset(RandomAccessFileOrArray rf, INullValueDictionary<int, int[]> glyphsUsed) : base(rf)
    {
        // Use CFFFont c'tor in order to parse the font file.
        GlyphsUsed = glyphsUsed ?? throw new ArgumentNullException(nameof(glyphsUsed));
        //Put the glyphs into a list
        GlyphsInList = new List<int>(glyphsUsed.Keys);


        for (var i = 0; i < Fonts.Length; ++i)
        {
            // Read the number of glyphs in the font
            Seek(Fonts[i].CharstringsOffset);
            Fonts[i].Nglyphs = GetCard16();

            // Jump to the count field of the String Index
            Seek(StringIndexOffset);
            Fonts[i].Nstrings = GetCard16() + StandardStrings.Length;

            // For each font save the offset array of the charstring
            Fonts[i].CharstringsOffsets = GetIndex(Fonts[i].CharstringsOffset);

            // Proces the FDSelect if exist
            if (Fonts[i].FdselectOffset >= 0)
            {
                // Proces the FDSelect
                ReadFdSelect(i);
                // Build the FDArrayUsed hashmap
                BuildFdArrayUsed(i);
            }

            if (Fonts[i].IsCid)
                // Build the FD Array used Hash Map
            {
                ReadFdArray(i);
            }

            // compute the charset length
            Fonts[i].CharsetLength = CountCharset(Fonts[i].CharsetOffset, Fonts[i].Nglyphs);
        }
    }

    /// <summary>
    ///     The Process function extracts one font out of the CFF file and returns a
    ///     subset version of the original.
    ///     @throws IOException
    /// </summary>
    /// <param name="fontName">- The name of the font to be taken out of the CFF</param>
    /// <returns>The new font stream</returns>
    public byte[] Process(string fontName)
    {
        if (fontName == null)
        {
            throw new ArgumentNullException(nameof(fontName));
        }

        try
        {
            // Verify that the file is open
            Buf.ReOpen();
            // Find the Font that we will be dealing with
            int j;
            for (j = 0; j < Fonts.Length; j++)
            {
                if (fontName.Equals(Fonts[j].Name, StringComparison.Ordinal))
                {
                    break;
                }
            }

            if (j == Fonts.Length)
            {
                return null;
            }

            // Calc the bias for the global subrs
            if (GsubrIndexOffset >= 0)
            {
                GBias = CalcBias(GsubrIndexOffset, j);
            }

            // Prepare the new CharStrings Index
            BuildNewCharString(j);
            // Prepare the new Global and Local Subrs Indices
            BuildNewLgSubrs(j);
            // Build the new file
            var ret = BuildNewFile(j);
            return ret;
        }
        finally
        {
            try
            {
                Buf.Close();
            }
            catch
            {
                // empty on purpose
            }
        }
    }

    /// <summary>
    ///     Calculates how many byte it took to write the offset for the subrs in a specific
    ///     private dict.
    /// </summary>
    /// <param name="offset">The Offset for the private dict</param>
    /// <param name="size">The size of the private dict</param>
    /// <returns>The size of the offset of the subrs in the private dict</returns>
    internal int CalcSubrOffsetSize(int offset, int size)
    {
        // Set the size to 0
        var offsetSize = 0;
        // Go to the beginning of the private dict
        Seek(offset);
        // Go until the end of the private dict
        while (GetPosition() < offset + size)
        {
            var p1 = GetPosition();
            GetDictItem();
            var p2 = GetPosition();
            // When reached to the subrs offset
            if (Key == "Subrs")
            {
                // The Offsize (minus the subrs key)
                offsetSize = p2 - p1 - 1;
            }
            // All other keys are ignored
        }

        // return the size
        return offsetSize;
    }

    /// <summary>
    ///     Calculates the length of the charset according to its format
    /// </summary>
    /// <param name="offset">The Charset Offset</param>
    /// <param name="numofGlyphs">Number of glyphs in the font</param>
    /// <returns>the length of the Charset</returns>
    internal int CountCharset(int offset, int numofGlyphs)
    {
        int format;
        var length = 0;
        Seek(offset);
        // Read the format
        format = GetCard8();
        // Calc according to format
        switch (format)
        {
            case 0:
                length = 1 + 2 * numofGlyphs;
                break;
            case 1:
                length = 1 + 3 * countRange(numofGlyphs, 1);
                break;
            case 2:
                length = 1 + 4 * countRange(numofGlyphs, 2);
                break;
        }

        return length;
    }

    /// <summary>
    ///     The function creates a private dict for a font that was not CID
    ///     All the keys are copied as is except for the subrs key
    /// </summary>
    /// <param name="font">the font</param>
    /// <param name="subr">The OffsetItem for the subrs of the private</param>
    internal void CreateNonCidPrivate(int font, OffsetItem subr)
    {
        // Go to the beginning of the private dict and read untill the end
        Seek(Fonts[font].PrivateOffset);
        while (GetPosition() < Fonts[font].PrivateOffset + Fonts[font].PrivateLength)
        {
            var p1 = GetPosition();
            GetDictItem();
            var p2 = GetPosition();
            // If the dictItem is the "Subrs" then,
            // use marker for offset and write operator number
            if (Key == "Subrs")
            {
                OutputList.Add(subr);
                OutputList.Add(new UInt8Item((char)19)); // Subrs
            }
            // Else copy the entire range
            else
            {
                OutputList.Add(new RangeItem(Buf, p1, p2 - p1));
            }
        }
    }

    /// <summary>
    ///     the function marks the beginning of the subrs index and adds the subsetted subrs
    ///     index to the output list.
    ///     @throws IOException
    /// </summary>
    /// <param name="font">the font</param>
    /// <param name="privateBase">IndexBaseItem for the private that's referencing to the subrs</param>
    /// <param name="subrs">OffsetItem for the subrs</param>
    internal void CreateNonCidSubrs(int font, IndexBaseItem privateBase, OffsetItem subrs)
    {
        // Mark the beginning of the Subrs index
        OutputList.Add(new SubrMarkerItem(subrs, privateBase));
        // Put the subsetted new subrs index
        OutputList.Add(new RangeItem(new RandomAccessFileOrArray(NewSubrsIndexNonCid), 0, NewSubrsIndexNonCid.Length));
    }

    /// <summary>
    ///     Function Adds the new private dicts (only for the FDs used) to the list
    ///     @throws IOException
    /// </summary>
    /// <param name="font">the font</param>
    /// <param name="fdPrivate">OffsetItem array one element for each private</param>
    /// <param name="fdPrivateBase">IndexBaseItem array one element for each private</param>
    /// <param name="fdSubrs">OffsetItem array one element for each private</param>
    internal void ReconstructPrivateDict(int font, OffsetItem[] fdPrivate, IndexBaseItem[] fdPrivateBase,
                                         OffsetItem[] fdSubrs)
    {
        // For each fdarray private dict check if that FD is used.
        // if is used build a new one by changing the subrs offset
        // Else do nothing
        for (var i = 0; i < Fonts[font].FdprivateOffsets.Length; i++)
        {
            if (FdArrayUsed.ContainsKey(i))
            {
                // Mark beginning
                OutputList.Add(new MarkerItem(fdPrivate[i]));
                fdPrivateBase[i] = new IndexBaseItem();
                OutputList.Add(fdPrivateBase[i]);
                // Goto begining of objects
                Seek(Fonts[font].FdprivateOffsets[i]);
                while (GetPosition() < Fonts[font].FdprivateOffsets[i] + Fonts[font].FdprivateLengths[i])
                {
                    var p1 = GetPosition();
                    GetDictItem();
                    var p2 = GetPosition();
                    // If the dictItem is the "Subrs" then,
                    // use marker for offset and write operator number
                    if (Key == "Subrs")
                    {
                        fdSubrs[i] = new DictOffsetItem();
                        OutputList.Add(fdSubrs[i]);
                        OutputList.Add(new UInt8Item((char)19)); // Subrs
                    }
                    // Else copy the entire range
                    else
                    {
                        OutputList.Add(new RangeItem(Buf, p1, p2 - p1));
                    }
                }
            }
        }
    }

    internal void ReconstructPrivateSubrs(int font, IndexBaseItem[] fdPrivateBase,
                                          OffsetItem[] fdSubrs)
    {
        // For each private dict
        for (var i = 0; i < Fonts[font].FdprivateLengths.Length; i++)
        {
            // If that private dict's Subrs are used insert the new LSubrs
            // computed earlier
            if (fdSubrs[i] != null && Fonts[font].PrivateSubrsOffset[i] >= 0)
            {
                OutputList.Add(new SubrMarkerItem(fdSubrs[i], fdPrivateBase[i]));
                OutputList.Add(new RangeItem(new RandomAccessFileOrArray(NewLSubrsIndex[i]), 0,
                                             NewLSubrsIndex[i].Length));
            }
        }
    }

    /// <summary>
    ///     Function creates the new index, inserting the count,offsetsize,offset array
    ///     and object array.
    /// </summary>
    /// <param name="newOffsets">the subsetted offset array</param>
    /// <param name="newObjects">the subsetted object array</param>
    /// <returns>the new index created</returns>
    protected static byte[] AssembleIndex(int[] newOffsets, byte[] newObjects)
    {
        if (newOffsets == null)
        {
            throw new ArgumentNullException(nameof(newOffsets));
        }

        if (newObjects == null)
        {
            throw new ArgumentNullException(nameof(newObjects));
        }

        // Calc the index' count field
        var count = (char)(newOffsets.Length - 1);
        // Calc the size of the object array
        var size = newOffsets[newOffsets.Length - 1];
        // Calc the Offsize
        byte offsize;
        if (size <= 0xff)
        {
            offsize = 1;
        }
        else if (size <= 0xffff)
        {
            offsize = 2;
        }
        else if (size <= 0xffffff)
        {
            offsize = 3;
        }
        else
        {
            offsize = 4;
        }

        // The byte array for the new index. The size is calc by
        // Count=2, Offsize=1, OffsetArray = Offsize*(Count+1), The object array
        var newIndex = new byte[2 + 1 + offsize * (count + 1) + newObjects.Length];
        // The counter for writing
        var place = 0;
        // Write the count field
        newIndex[place++] = (byte)((count >> 8) & 0xff);
        newIndex[place++] = (byte)((count >> 0) & 0xff);
        // Write the offsize field
        newIndex[place++] = offsize;
        // Write the offset array according to the offsize
        for (var i = 0; i < newOffsets.Length; i++)
        {
            // The value to be written
            var num = newOffsets[i] - newOffsets[0] + 1;
            // Write in bytes according to the offsize
            switch (offsize)
            {
                case 4:
                    newIndex[place++] = (byte)((num >> 24) & 0xff);
                    goto case 3;
                case 3:
                    newIndex[place++] = (byte)((num >> 16) & 0xff);
                    goto case 2;
                case 2:
                    newIndex[place++] = (byte)((num >> 8) & 0xff);
                    goto case 1;
                case 1:
                    newIndex[place++] = (byte)((num >> 0) & 0xff);
                    break;
            }
        }

        // Write the new object array one by one
        for (var i = 0; i < newObjects.Length; i++)
        {
            newIndex[place++] = newObjects[i];
        }

        // Return the new index
        return newIndex;
    }

    /// <summary>
    ///     Function reads the FDSelect and builds the FDArrayUsed HashMap According to the glyphs used
    /// </summary>
    /// <param name="font">the Number of font being processed</param>
    protected void BuildFdArrayUsed(int font)
    {
        var fdSelect = Fonts[font].FdSelect;
        // For each glyph used
        for (var i = 0; i < GlyphsInList.Count; i++)
        {
            // Pop the glyphs index
            var glyph = GlyphsInList[i];
            // Pop the glyph's FD
            var fd = fdSelect[glyph];
            // Put the FD index into the FDArrayUsed HashMap
            FdArrayUsed[fd] = null;
        }
    }

    /// <summary>
    ///     The function finds for the FD array processed the local subr offset and its
    ///     offset array.
    /// </summary>
    /// <param name="font">the font</param>
    /// <param name="fd">The FDARRAY processed</param>
    protected void BuildFdSubrsOffsets(int font, int fd)
    {
        // Initiate to -1 to indicate lsubr operator present
        Fonts[font].PrivateSubrsOffset[fd] = -1;
        // Goto begining of objects
        Seek(Fonts[font].FdprivateOffsets[fd]);
        // While in the same object:
        while (GetPosition() < Fonts[font].FdprivateOffsets[fd] + Fonts[font].FdprivateLengths[fd])
        {
            GetDictItem();
            // If the dictItem is the "Subrs" then find and store offset,
            if (Key == "Subrs")
            {
                Fonts[font].PrivateSubrsOffset[fd] = (int)Args[0] + Fonts[font].FdprivateOffsets[fd];
            }
        }

        //Read the lsub index if the lsubr was found
        if (Fonts[font].PrivateSubrsOffset[fd] >= 0)
        {
            Fonts[font].PrivateSubrsOffsetsArray[fd] = GetIndex(Fonts[font].PrivateSubrsOffset[fd]);
        }
    }

    /// <summary>
    ///     Function scans the Glsubr used ArrayList to find recursive calls
    ///     to Gsubrs and adds to Hashmap and ArrayList
    /// </summary>
    /// <param name="font">the font</param>
    protected void BuildGSubrsUsed(int font)
    {
        var lBias = 0;
        var sizeOfNonCidSubrsUsed = 0;
        if (Fonts[font].PrivateSubrs >= 0)
        {
            lBias = CalcBias(Fonts[font].PrivateSubrs, font);
            sizeOfNonCidSubrsUsed = LSubrsUsedNonCid.Count;
        }

        // For each global subr used
        for (var i = 0; i < LGSubrsUsed.Count; i++)
        {
            //Pop the value + check valid
            var subr = LGSubrsUsed[i];
            if (subr < GsubrOffsets.Length - 1 && subr >= 0)
            {
                // Read the subr and process
                var start = GsubrOffsets[subr];
                var end = GsubrOffsets[subr + 1];

                if (Fonts[font].IsCid)
                {
                    ReadASubr(start, end, GBias, 0, HGSubrsUsed, LGSubrsUsed, null);
                }
                else
                {
                    ReadASubr(start, end, GBias, lBias, HSubrsUsedNonCid, LSubrsUsedNonCid, Fonts[font].SubrsOffsets);
                    if (sizeOfNonCidSubrsUsed < LSubrsUsedNonCid.Count)
                    {
                        for (var j = sizeOfNonCidSubrsUsed; j < LSubrsUsedNonCid.Count; j++)
                        {
                            //Pop the value + check valid
                            var lSubr = LSubrsUsedNonCid[j];
                            if (lSubr < Fonts[font].SubrsOffsets.Length - 1 && lSubr >= 0)
                            {
                                // Read the subr and process
                                var lStart = Fonts[font].SubrsOffsets[lSubr];
                                var lEnd = Fonts[font].SubrsOffsets[lSubr + 1];
                                ReadASubr(lStart, lEnd, GBias, lBias, HSubrsUsedNonCid, LSubrsUsedNonCid,
                                          Fonts[font].SubrsOffsets);
                            }
                        }

                        sizeOfNonCidSubrsUsed = LSubrsUsedNonCid.Count;
                    }
                }
            }
        }
    }

    /// <summary>
    ///     Function Build the header of an index
    /// </summary>
    /// <param name="count">the count field of the index</param>
    /// <param name="offsize">the offsize field of the index</param>
    /// <param name="first">the first offset of the index</param>
    protected void BuildIndexHeader(int count, int offsize, int first)
    {
        // Add the count field
        OutputList.Add(new UInt16Item((char)count)); // count
        // Add the offsize field
        OutputList.Add(new UInt8Item((char)offsize)); // offSize
        // Add the first offset according to the offsize
        switch (offsize)
        {
            case 1:
                OutputList.Add(new UInt8Item((char)first)); // first offset
                break;
            case 2:
                OutputList.Add(new UInt16Item((char)first)); // first offset
                break;
            case 3:
                OutputList.Add(new UInt24Item((char)first)); // first offset
                break;
            case 4:
                OutputList.Add(new UInt32Item((char)first)); // first offset
                break;
        }
    }

    /// <summary>
    ///     Function uses BuildNewIndex to create the new index of the subset charstrings
    ///     @throws IOException
    /// </summary>
    /// <param name="fontIndex">the font</param>
    protected void BuildNewCharString(int fontIndex)
    {
        NewCharStringsIndex = BuildNewIndex(Fonts[fontIndex].CharstringsOffsets, GlyphsUsed, ENDCHAR_OP);
    }

    /// <summary>
    ///     The function builds the new output stream according to the subset process
    ///     @throws IOException
    /// </summary>
    /// <param name="font">the font</param>
    /// <returns>the subseted font stream</returns>
    protected byte[] BuildNewFile(int font)
    {
        // Prepare linked list for new font components
        OutputList = new List<Item>();

        // copy the header of the font
        CopyHeader();

        // create a name index
        BuildIndexHeader(1, 1, 1);
        OutputList.Add(new UInt8Item((char)(1 + Fonts[font].Name.Length)));
        OutputList.Add(new StringItem(Fonts[font].Name));

        // create the topdict Index
        BuildIndexHeader(1, 2, 1);
        OffsetItem topdictIndex1Ref = new IndexOffsetItem(2);
        OutputList.Add(topdictIndex1Ref);
        var topdictBase = new IndexBaseItem();
        OutputList.Add(topdictBase);

        // Initialise the Dict Items for later use
        OffsetItem charsetRef = new DictOffsetItem();
        OffsetItem charstringsRef = new DictOffsetItem();
        OffsetItem fdarrayRef = new DictOffsetItem();
        OffsetItem fdselectRef = new DictOffsetItem();
        OffsetItem privateRef = new DictOffsetItem();

        // If the font is not CID create the following keys
        if (!Fonts[font].IsCid)
        {
            // create a ROS key
            OutputList.Add(new DictNumberItem(Fonts[font].Nstrings));
            OutputList.Add(new DictNumberItem(Fonts[font].Nstrings + 1));
            OutputList.Add(new DictNumberItem(0));
            OutputList.Add(new UInt8Item((char)12));
            OutputList.Add(new UInt8Item((char)30));
            // create a CIDCount key
            OutputList.Add(new DictNumberItem(Fonts[font].Nglyphs));
            OutputList.Add(new UInt8Item((char)12));
            OutputList.Add(new UInt8Item((char)34));
            // Sivan's comments
            // What about UIDBase (12,35)? Don't know what is it.
            // I don't think we need FontName; the font I looked at didn't have it.
        }

        // Go to the TopDict of the font being processed
        Seek(TopdictOffsets[font]);
        // Run untill the end of the TopDict
        while (GetPosition() < TopdictOffsets[font + 1])
        {
            var p1 = GetPosition();
            GetDictItem();
            var p2 = GetPosition();
            // The encoding key is disregarded since CID has no encoding
            if (Key == "Encoding"
                // These keys will be added manualy by the process.
                || Key == "Private"
                || Key == "FDSelect"
                || Key == "FDArray"
                || Key == "charset"
                || Key == "CharStrings"
               )
            {
            }
            else
            {
                //OtherWise copy key "as is" to the output list
                OutputList.Add(new RangeItem(Buf, p1, p2 - p1));
            }
        }

        // Create the FDArray, FDSelect, Charset and CharStrings Keys
        CreateKeys(fdarrayRef, fdselectRef, charsetRef, charstringsRef);

        // Mark the end of the top dict area
        OutputList.Add(new IndexMarkerItem(topdictIndex1Ref, topdictBase));

        // Copy the string index

        if (Fonts[font].IsCid)
        {
            OutputList.Add(GetEntireIndexRange(StringIndexOffset));
        }
        // If the font is not CID we need to append new strings.
        // We need 3 more strings: Registry, Ordering, and a FontName for one FD.
        // The total length is at most "Adobe"+"Identity"+63 = 76
        else
        {
            CreateNewStringIndex(font);
        }

        // copy the new subsetted global subroutine index
        OutputList.Add(new RangeItem(new RandomAccessFileOrArray(NewGSubrsIndex), 0, NewGSubrsIndex.Length));

        // deal with fdarray, fdselect, and the font descriptors
        // If the font is CID:
        if (Fonts[font].IsCid)
        {
            // copy the FDArray, FDSelect, charset

            // Copy FDSelect
            // Mark the beginning
            OutputList.Add(new MarkerItem(fdselectRef));
            // If an FDSelect exists copy it
            if (Fonts[font].FdselectOffset >= 0)
            {
                OutputList.Add(new RangeItem(Buf, Fonts[font].FdselectOffset, Fonts[font].FdSelectLength));
            }
            // Else create a new one
            else
            {
                CreateFdSelect(fdselectRef, Fonts[font].Nglyphs);
            }

            // Copy the Charset
            // Mark the beginning and copy entirly
            OutputList.Add(new MarkerItem(charsetRef));
            OutputList.Add(new RangeItem(Buf, Fonts[font].CharsetOffset, Fonts[font].CharsetLength));

            // Copy the FDArray
            // If an FDArray exists
            if (Fonts[font].FdarrayOffset >= 0)
            {
                // Mark the beginning
                OutputList.Add(new MarkerItem(fdarrayRef));
                // Build a new FDArray with its private dicts and their LSubrs
                reconstruct(font);
            }
            else
                // Else create a new one
            {
                CreateFdArray(fdarrayRef, privateRef, font);
            }
        }
        // If the font is not CID
        else
        {
            // create FDSelect
            CreateFdSelect(fdselectRef, Fonts[font].Nglyphs);
            // recreate a new charset
            CreateCharset(charsetRef, Fonts[font].Nglyphs);
            // create a font dict index (fdarray)
            CreateFdArray(fdarrayRef, privateRef, font);
        }

        // if a private dict exists insert its subsetted version
        if (Fonts[font].PrivateOffset >= 0)
        {
            // Mark the beginning of the private dict
            var privateBase = new IndexBaseItem();
            OutputList.Add(privateBase);
            OutputList.Add(new MarkerItem(privateRef));

            OffsetItem subr = new DictOffsetItem();
            // Build and copy the new private dict
            CreateNonCidPrivate(font, subr);
            // Copy the new LSubrs index
            CreateNonCidSubrs(font, privateBase, subr);
        }

        // copy the charstring index
        OutputList.Add(new MarkerItem(charstringsRef));

        // Add the subsetted charstring
        OutputList.Add(new RangeItem(new RandomAccessFileOrArray(NewCharStringsIndex), 0, NewCharStringsIndex.Length));

        // now create the new CFF font
        var currentOffset = new int[1];
        currentOffset[0] = 0;
        // Count and save the offset for each item
        foreach (var item in OutputList)
        {
            item.Increment(currentOffset);
        }

        // Compute the Xref for each of the offset items
        foreach (var item in OutputList)
        {
            item.Xref();
        }

        var size = currentOffset[0];
        var b = new byte[size];

        // Emit all the items into the new byte array
        foreach (var item in OutputList)
        {
            item.Emit(b);
        }

        // Return the new stream
        return b;
    }

    /// <summary>
    ///     Function builds the new offset array, object array and assembles the index.
    ///     used for creating the glyph and subrs subsetted index
    ///     @throws IOException
    /// </summary>
    /// <param name="offsets">the offset array of the original index</param>
    /// <param name="used">the hashmap of the used objects</param>
    /// <param name="operatorForUnusedEntries">the operator inserted into the data stream for unused entries</param>
    /// <returns>the new index subset version</returns>
    protected byte[] BuildNewIndex(int[] offsets, INullValueDictionary<int, int[]> used,
                                   byte operatorForUnusedEntries)
    {
        if (offsets == null)
        {
            throw new ArgumentNullException(nameof(offsets));
        }

        if (used == null)
        {
            throw new ArgumentNullException(nameof(used));
        }

        var unusedCount = 0;
        var offset = 0;
        var newOffsets = new int[offsets.Length];
        // Build the Offsets Array for the Subset
        for (var i = 0; i < offsets.Length; ++i)
        {
            newOffsets[i] = offset;
            // If the object in the offset is also present in the used
            // HashMap then increment the offset var by its size
            if (used.ContainsKey(i))
            {
                offset += offsets[i + 1] - offsets[i];
            }
            else
            {
                // Else the same offset is kept in i+1.
                unusedCount++;
            }
        }

        // Offset var determines the size of the object array
        var newObjects = new byte[offset + unusedCount];
        // Build the new Object array
        var unusedOffset = 0;
        for (var i = 0; i < offsets.Length - 1; ++i)
        {
            var start = newOffsets[i];
            var end = newOffsets[i + 1];
            newOffsets[i] = start + unusedOffset;
            // If start != End then the Object is used
            // So, we will copy the object data from the font file
            if (start != end)
            {
                // All offsets are Global Offsets relative to the begining of the font file.
                // Jump the file pointer to the start address to read from.
                Buf.Seek(offsets[i]);
                // Read from the buffer and write into the array at start.
                Buf.ReadFully(newObjects, start + unusedOffset, end - start);
            }
            else
            {
                newObjects[start + unusedOffset] = operatorForUnusedEntries;
                unusedOffset++;
            }
        }

        newOffsets[offsets.Length - 1] += unusedOffset;
        // Use AssembleIndex to build the index from the offset & object arrays
        return AssembleIndex(newOffsets, newObjects);
    }

    /// <summary>
    ///     Function builds the new local and global subsrs indices. IF CID then All of
    ///     the FD Array lsubrs will be subsetted.
    ///     @throws IOException
    /// </summary>
    /// <param name="font">the font</param>
    protected void BuildNewLgSubrs(int font)
    {
        // If the font is CID then the lsubrs are divided into FontDicts.
        // for each FD array the lsubrs will be subsetted.
        if (Fonts[font].IsCid)
        {
            // Init the hasmap-array and the arraylist-array to hold the subrs used
            // in each private dict.
            HSubrsUsed = new NullValueDictionary<int, int[]>[Fonts[font].FdprivateOffsets.Length];
            LSubrsUsed = new List<int>[Fonts[font].FdprivateOffsets.Length];
            // A [][] which will store the byte array for each new FD Array lsubs index
            NewLSubrsIndex = new byte[Fonts[font].FdprivateOffsets.Length][];
            // An array to hold the offset for each Lsubr index
            Fonts[font].PrivateSubrsOffset = new int[Fonts[font].FdprivateOffsets.Length];
            // A [][] which will store the offset array for each lsubr index
            Fonts[font].PrivateSubrsOffsetsArray = new int[Fonts[font].FdprivateOffsets.Length][];

            // Put the FDarrayUsed into a list
            var fdInList = new List<int>(FdArrayUsed.Keys);
            // For each FD array which is used subset the lsubr
            for (var j = 0; j < fdInList.Count; j++)
            {
                // The FDArray index, Hash Map, Arrat List to work on
                var fd = fdInList[j];
                HSubrsUsed[fd] = new NullValueDictionary<int, int[]>();
                LSubrsUsed[fd] = new List<int>();
                //Reads the private dicts looking for the subr operator and
                // store both the offest for the index and its offset array
                BuildFdSubrsOffsets(font, fd);
                // Verify that FDPrivate has a LSubrs index
                if (Fonts[font].PrivateSubrsOffset[fd] >= 0)
                {
                    //Scans the Charsting data storing the used Local and Global subroutines
                    // by the glyphs. Scans the Subrs recursivley.
                    BuildSubrUsed(font, fd, Fonts[font].PrivateSubrsOffset[fd],
                                  Fonts[font].PrivateSubrsOffsetsArray[fd], HSubrsUsed[fd], LSubrsUsed[fd]);
                    // Builds the New Local Subrs index
                    NewLSubrsIndex[fd] =
                        BuildNewIndex(Fonts[font].PrivateSubrsOffsetsArray[fd], HSubrsUsed[fd], RETURN_OP);
                }
            }
        }
        // If the font is not CID && the Private Subr exists then subset:
        else if (Fonts[font].PrivateSubrs >= 0)
        {
            // Build the subrs offsets;
            Fonts[font].SubrsOffsets = GetIndex(Fonts[font].PrivateSubrs);
            //Scans the Charsting data storing the used Local and Global subroutines
            // by the glyphs. Scans the Subrs recursivley.
            BuildSubrUsed(font, -1, Fonts[font].PrivateSubrs, Fonts[font].SubrsOffsets, HSubrsUsedNonCid,
                          LSubrsUsedNonCid);
        }

        // For all fonts susbset the Global Subroutines
        // Scan the Global Subr Hashmap recursivly on the Gsubrs
        BuildGSubrsUsed(font);
        if (Fonts[font].PrivateSubrs >= 0)
            // Builds the New Local Subrs index
        {
            NewSubrsIndexNonCid = BuildNewIndex(Fonts[font].SubrsOffsets, HSubrsUsedNonCid, RETURN_OP);
        }

        //Builds the New Global Subrs index
        NewGSubrsIndex = BuildNewIndex(GsubrOffsets, HGSubrsUsed, RETURN_OP);
    }

    /// <summary>
    ///     Function uses ReadAsubr on the glyph used to build the LSubr and Gsubr HashMap.
    ///     The HashMap (of the lsub only) is then scaned recursivly for Lsubr and Gsubrs
    ///     calls.
    /// </summary>
    /// <param name="font">the font</param>
    /// <param name="fd">FD array processed. 0 indicates function was called by non CID font</param>
    /// <param name="subrOffset">the offset to the subr index to calc the bias</param>
    /// <param name="subrsOffsets">the offset array of the subr index</param>
    /// <param name="hSubr">HashMap of the subrs used</param>
    /// <param name="lSubr">ArrayList of the subrs used</param>
    protected void BuildSubrUsed(int font, int fd, int subrOffset, int[] subrsOffsets,
                                 INullValueDictionary<int, int[]> hSubr, IList<int> lSubr)
    {
        if (subrsOffsets == null)
        {
            throw new ArgumentNullException(nameof(subrsOffsets));
        }

        if (hSubr == null)
        {
            throw new ArgumentNullException(nameof(hSubr));
        }

        if (lSubr == null)
        {
            throw new ArgumentNullException(nameof(lSubr));
        }

        // Calc the Bias for the subr index
        var lBias = CalcBias(subrOffset, font);

        // For each glyph used find its GID, start & end pos
        for (var i = 0; i < GlyphsInList.Count; i++)
        {
            var glyph = GlyphsInList[i];
            var start = Fonts[font].CharstringsOffsets[glyph];
            var end = Fonts[font].CharstringsOffsets[glyph + 1];

            // IF CID:
            if (fd >= 0)
            {
                EmptyStack();
                NumOfHints = 0;
                // Using FDSELECT find the FD Array the glyph belongs to.
                var glyphFd = Fonts[font].FdSelect[glyph];
                // If the Glyph is part of the FD being processed
                if (glyphFd == fd)
                    // Find the Subrs called by the glyph and insert to hash:
                {
                    ReadASubr(start, end, GBias, lBias, hSubr, lSubr, subrsOffsets);
                }
            }
            else
                // If the font is not CID
                //Find the Subrs called by the glyph and insert to hash:
            {
                ReadASubr(start, end, GBias, lBias, hSubr, lSubr, subrsOffsets);
            }
        }

        // For all Lsubrs used, check recusrivly for Lsubr & Gsubr used
        for (var i = 0; i < lSubr.Count; i++)
        {
            // Pop the subr value from the hash
            var subr = lSubr[i];
            // Ensure the Lsubr call is valid
            if (subr < subrsOffsets.Length - 1 && subr >= 0)
            {
                // Read and process the subr
                var start = subrsOffsets[subr];
                var end = subrsOffsets[subr + 1];
                ReadASubr(start, end, GBias, lBias, hSubr, lSubr, subrsOffsets);
            }
        }
    }

    /// <summary>
    ///     Function calcs bias according to the CharString type and the count
    ///     of the subrs
    /// </summary>
    /// <param name="offset">The offset to the relevent subrs index</param>
    /// <param name="font">the font</param>
    /// <returns>The calculated Bias</returns>
    protected int CalcBias(int offset, int font)
    {
        Seek(offset);
        int nSubrs = GetCard16();
        // If type==1 -> bias=0
        if (Fonts[font].CharstringType == 1)
        {
            return 0;
        }
        // else calc according to the count

        if (nSubrs < 1240)
        {
            return 107;
        }

        if (nSubrs < 33900)
        {
            return 1131;
        }

        return 32768;
    }

    /// <summary>
    ///     The function reads the subroutine and returns the number of the hint in it.
    ///     If a call to another subroutine is found the function calls recursively.
    /// </summary>
    /// <param name="begin">the start point of the subr</param>
    /// <param name="end">the end point of the subr</param>
    /// <param name="lBias">the bias of the Local Subrs</param>
    /// <param name="gBias">the bias of the Global Subrs</param>
    /// <param name="lSubrsOffsets">The Offsets array of the subroutines</param>
    /// <returns>The number of hints in the subroutine read.</returns>
    protected int CalcHints(int begin, int end, int lBias, int gBias, int[] lSubrsOffsets)
    {
        if (lSubrsOffsets == null)
        {
            throw new ArgumentNullException(nameof(lSubrsOffsets));
        }

        // Goto begining of the subr
        Seek(begin);
        while (GetPosition() < end)
        {
            // Read the next command
            ReadCommand();
            var pos = GetPosition();
            object topElement = null;
            if (ArgCount > 0)
            {
                topElement = Args[ArgCount - 1];
            }

            var numOfArgs = ArgCount;
            //Check the modification needed on the Argument Stack according to key;
            HandelStack();
            // a call to a Lsubr
            if (Key == "callsubr")
            {
                if (numOfArgs > 0)
                {
                    var subr = (int)topElement + lBias;
                    CalcHints(lSubrsOffsets[subr], lSubrsOffsets[subr + 1], lBias, gBias, lSubrsOffsets);
                    Seek(pos);
                }
            }
            // a call to a Gsubr
            else if (Key == "callgsubr")
            {
                if (numOfArgs > 0)
                {
                    var subr = (int)topElement + gBias;
                    CalcHints(GsubrOffsets[subr], GsubrOffsets[subr + 1], lBias, gBias, lSubrsOffsets);
                    Seek(pos);
                }
            }
            // A call to "stem"
            else if (Key == "hstem" || Key == "vstem" || Key == "hstemhm" || Key == "vstemhm")
                // Increment the NumOfHints by the number couples of of arguments
            {
                NumOfHints += numOfArgs / 2;
            }
            // A call to "mask"
            else if (Key == "hintmask" || Key == "cntrmask")
            {
                // Compute the size of the mask
                var sizeOfMask = NumOfHints / 8;
                if (NumOfHints % 8 != 0 || sizeOfMask == 0)
                {
                    sizeOfMask++;
                }

                // Continue the pointer in SizeOfMask steps
                for (var i = 0; i < sizeOfMask; i++)
                {
                    GetCard8();
                }
            }
        }

        return NumOfHints;
    }

    /// <summary>
    ///     Function Copies the header from the original fileto the output list
    /// </summary>
    protected void CopyHeader()
    {
        Seek(0);
        int major = GetCard8();
        int minor = GetCard8();
        int hdrSize = GetCard8();
        int offSize = GetCard8();
        NextIndexOffset = hdrSize;
        OutputList.Add(new RangeItem(Buf, 0, hdrSize));
    }

    /// <summary>
    ///     Function computes the size of an index
    /// </summary>
    /// <param name="indexOffset">The offset for the computed index</param>
    /// <returns>The size of the index</returns>
    protected int CountEntireIndexRange(int indexOffset)
    {
        // Go to the beginning of the index
        Seek(indexOffset);
        // Read the count field
        int count = GetCard16();
        // If count==0 -> size=2
        if (count == 0)
        {
            return 2;
        }

        // Read the offsize field
        int indexOffSize = GetCard8();
        // Go to the last element of the offset array
        Seek(indexOffset + 2 + 1 + count * indexOffSize);
        // The size of the object array is the value of the last element-1
        var size = GetOffset(indexOffSize) - 1;
        // Return the size of the entire index
        return 2 + 1 + (count + 1) * indexOffSize + size;
    }

    /// <summary>
    ///     Function creates new CharSet for non-CID fonts.
    ///     The CharSet built uses a single range for all glyphs
    /// </summary>
    /// <param name="charsetRef">OffsetItem for the CharSet</param>
    /// <param name="nglyphs">the number of glyphs in the font</param>
    protected void CreateCharset(OffsetItem charsetRef, int nglyphs)
    {
        OutputList.Add(new MarkerItem(charsetRef));
        OutputList.Add(new UInt8Item((char)2)); // format identifier
        OutputList.Add(new UInt16Item((char)1)); // first glyph in range (ignore .notdef)
        OutputList.Add(new UInt16Item((char)(nglyphs - 1))); // nLeft
    }

    /// <summary>
    ///     Function creates new FDArray for non-CID fonts.
    ///     The FDArray built has only the "Private" operator that points to the font's
    ///     original private dict
    /// </summary>
    /// <param name="fdarrayRef">OffsetItem for the FDArray</param>
    /// <param name="privateRef">OffsetItem for the Private Dict</param>
    /// <param name="font">the font</param>
    protected void CreateFdArray(OffsetItem fdarrayRef, OffsetItem privateRef, int font)
    {
        OutputList.Add(new MarkerItem(fdarrayRef));
        // Build the header (count=offsize=first=1)
        BuildIndexHeader(1, 1, 1);

        // Mark
        OffsetItem privateIndex1Ref = new IndexOffsetItem(1);
        OutputList.Add(privateIndex1Ref);
        var privateBase = new IndexBaseItem();
        // Insert the private operands and operator
        OutputList.Add(privateBase);
        // Calc the new size of the private after subsetting
        // Origianl size
        var newSize = Fonts[font].PrivateLength;
        // Calc the original size of the Subr offset in the private
        var orgSubrsOffsetSize = CalcSubrOffsetSize(Fonts[font].PrivateOffset, Fonts[font].PrivateLength);
        // Increase the ptivate's size
        if (orgSubrsOffsetSize != 0)
        {
            newSize += 5 - orgSubrsOffsetSize;
        }

        OutputList.Add(new DictNumberItem(newSize));
        OutputList.Add(privateRef);
        OutputList.Add(new UInt8Item((char)18)); // Private

        OutputList.Add(new IndexMarkerItem(privateIndex1Ref, privateBase));
    }

    /// <summary>
    ///     Function creates new FDSelect for non-CID fonts.
    ///     The FDSelect built uses a single range for all glyphs
    /// </summary>
    /// <param name="fdselectRef">OffsetItem for the FDSelect</param>
    /// <param name="nglyphs">the number of glyphs in the font</param>
    protected void CreateFdSelect(OffsetItem fdselectRef, int nglyphs)
    {
        OutputList.Add(new MarkerItem(fdselectRef));
        OutputList.Add(new UInt8Item((char)3)); // format identifier
        OutputList.Add(new UInt16Item((char)1)); // nRanges

        OutputList.Add(new UInt16Item((char)0)); // Range[0].firstGlyph
        OutputList.Add(new UInt8Item((char)0)); // Range[0].fd

        OutputList.Add(new UInt16Item((char)nglyphs)); // sentinel
    }

    /// <summary>
    ///     Function adds the keys into the TopDict
    /// </summary>
    /// <param name="fdarrayRef">OffsetItem for the FDArray</param>
    /// <param name="fdselectRef">OffsetItem for the FDSelect</param>
    /// <param name="charsetRef">OffsetItem for the CharSet</param>
    /// <param name="charstringsRef">OffsetItem for the CharString</param>
    protected void CreateKeys(OffsetItem fdarrayRef, OffsetItem fdselectRef, OffsetItem charsetRef,
                              OffsetItem charstringsRef)
    {
        // create an FDArray key
        OutputList.Add(fdarrayRef);
        OutputList.Add(new UInt8Item((char)12));
        OutputList.Add(new UInt8Item((char)36));
        // create an FDSelect key
        OutputList.Add(fdselectRef);
        OutputList.Add(new UInt8Item((char)12));
        OutputList.Add(new UInt8Item((char)37));
        // create an charset key
        OutputList.Add(charsetRef);
        OutputList.Add(new UInt8Item((char)15));
        // create a CharStrings key
        OutputList.Add(charstringsRef);
        OutputList.Add(new UInt8Item((char)17));
    }

    /// <summary>
    ///     Function takes the original string item and adds the new strings
    ///     to accomodate the CID rules
    /// </summary>
    /// <param name="font">the font</param>
    protected void CreateNewStringIndex(int font)
    {
        var fdFontName = Fonts[font].Name + "-OneRange";
        if (fdFontName.Length > 127)
        {
            fdFontName = fdFontName.Substring(0, 127);
        }

        var extraStrings = "Adobe" + "Identity" + fdFontName;

        var origStringsLen = StringOffsets[StringOffsets.Length - 1]
                             - StringOffsets[0];
        var stringsBaseOffset = StringOffsets[0] - 1;

        byte stringsIndexOffSize;
        if (origStringsLen + extraStrings.Length <= 0xff)
        {
            stringsIndexOffSize = 1;
        }
        else if (origStringsLen + extraStrings.Length <= 0xffff)
        {
            stringsIndexOffSize = 2;
        }
        else if (origStringsLen + extraStrings.Length <= 0xffffff)
        {
            stringsIndexOffSize = 3;
        }
        else
        {
            stringsIndexOffSize = 4;
        }

        OutputList.Add(new UInt16Item((char)(StringOffsets.Length - 1 + 3))); // count
        OutputList.Add(new UInt8Item((char)stringsIndexOffSize)); // offSize
        for (var i = 0; i < StringOffsets.Length; i++)
        {
            OutputList.Add(new IndexOffsetItem(stringsIndexOffSize,
                                               StringOffsets[i] - stringsBaseOffset));
        }

        var currentStringsOffset = StringOffsets[StringOffsets.Length - 1]
                                   - stringsBaseOffset;
        //l.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
        currentStringsOffset += "Adobe".Length;
        OutputList.Add(new IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
        currentStringsOffset += "Identity".Length;
        OutputList.Add(new IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));
        currentStringsOffset += fdFontName.Length;
        OutputList.Add(new IndexOffsetItem(stringsIndexOffSize, currentStringsOffset));

        OutputList.Add(new RangeItem(Buf, StringOffsets[0], origStringsLen));
        OutputList.Add(new StringItem(extraStrings));
    }

    /// <summary>
    ///     Empty the Type2 Stack
    /// </summary>
    protected void EmptyStack()
    {
        // Null the arguments
        for (var i = 0; i < ArgCount; i++)
        {
            Args[i] = null;
        }

        ArgCount = 0;
    }

    /// <summary>
    ///     Function Checks how the current operator effects the run time stack after being run
    ///     An operator may increase or decrease the stack size
    /// </summary>
    protected void HandelStack()
    {
        // Findout what the operator does to the stack
        var stackHandel = StackOpp();
        if (stackHandel < 2)
        {
            // The operators that enlarge the stack by one
            if (stackHandel == 1)
            {
                PushStack();
            }
            // The operators that pop the stack
            else
            {
                // Abs value for the for loop
                stackHandel *= -1;
                for (var i = 0; i < stackHandel; i++)
                {
                    PopStack();
                }
            }
        }
        // All other flush the stack
        else
        {
            EmptyStack();
        }
    }

    /// <summary>
    ///     Pop one element from the stack
    /// </summary>
    protected void PopStack()
    {
        if (ArgCount > 0)
        {
            Args[ArgCount - 1] = null;
            ArgCount--;
        }
    }

    /// <summary>
    ///     Add an item to the stack
    /// </summary>
    protected void PushStack()
    {
        ArgCount++;
    }

    /// <summary>
    ///     The function reads a subrs (glyph info) between begin and end.
    ///     Adds calls to a Lsubr to the hSubr and lSubrs.
    ///     Adds calls to a Gsubr to the hGSubr and lGSubrs.
    /// </summary>
    /// <param name="begin">the start point of the subr</param>
    /// <param name="end">the end point of the subr</param>
    /// <param name="gBias">the bias of the Global Subrs</param>
    /// <param name="lBias">the bias of the Local Subrs</param>
    /// <param name="hSubr">the HashMap for the lSubrs</param>
    /// <param name="lSubr"></param>
    /// <param name="lSubrsOffsets">the ArrayList for the lSubrs</param>
    protected void ReadASubr(int begin, int end, int gBias, int lBias,
                             INullValueDictionary<int, int[]> hSubr, IList<int> lSubr, int[] lSubrsOffsets)
    {
        if (hSubr == null)
        {
            throw new ArgumentNullException(nameof(hSubr));
        }

        if (lSubr == null)
        {
            throw new ArgumentNullException(nameof(lSubr));
        }

        if (lSubrsOffsets == null)
        {
            throw new ArgumentNullException(nameof(lSubrsOffsets));
        }

        // Clear the stack for the subrs
        EmptyStack();
        NumOfHints = 0;
        // Goto begining of the subr
        Seek(begin);
        while (GetPosition() < end)
        {
            // Read the next command
            ReadCommand();
            var pos = GetPosition();
            object topElement = null;
            if (ArgCount > 0)
            {
                topElement = Args[ArgCount - 1];
            }

            var numOfArgs = ArgCount;
            // Check the modification needed on the Argument Stack according to key;
            HandelStack();
            // a call to a Lsubr
            if (Key == "callsubr")
            {
                // Verify that arguments are passed
                if (numOfArgs > 0)
                {
                    // Calc the index of the Subrs
                    var subr = (int)topElement + lBias;
                    // If the subr isn't in the HashMap -> Put in
                    if (!hSubr.ContainsKey(subr))
                    {
                        hSubr[subr] = null;
                        lSubr.Add(subr);
                    }

                    CalcHints(lSubrsOffsets[subr], lSubrsOffsets[subr + 1], lBias, gBias, lSubrsOffsets);
                    Seek(pos);
                }
            }
            // a call to a Gsubr
            else if (Key == "callgsubr")
            {
                // Verify that arguments are passed
                if (numOfArgs > 0)
                {
                    // Calc the index of the Subrs
                    var subr = (int)topElement + gBias;
                    // If the subr isn't in the HashMap -> Put in
                    if (!HGSubrsUsed.ContainsKey(subr))
                    {
                        HGSubrsUsed[subr] = null;
                        LGSubrsUsed.Add(subr);
                    }

                    CalcHints(GsubrOffsets[subr], GsubrOffsets[subr + 1], lBias, gBias, lSubrsOffsets);
                    Seek(pos);
                }
            }
            // A call to "stem"
            else if (Key == "hstem" || Key == "vstem" || Key == "hstemhm" || Key == "vstemhm")
                // Increment the NumOfHints by the number couples of of arguments
            {
                NumOfHints += numOfArgs / 2;
            }
            // A call to "mask"
            else if (Key == "hintmask" || Key == "cntrmask")
            {
                // Compute the size of the mask
                var sizeOfMask = NumOfHints / 8;
                if (NumOfHints % 8 != 0 || sizeOfMask == 0)
                {
                    sizeOfMask++;
                }

                // Continue the pointer in SizeOfMask steps
                for (var i = 0; i < sizeOfMask; i++)
                {
                    GetCard8();
                }
            }
        }
    }

    /// <summary>
    ///     The function reads the next command after the file pointer is set
    /// </summary>
    protected void ReadCommand()
    {
        Key = null;
        var gotKey = false;
        // Until a key is found
        while (!gotKey)
        {
            // Read the first Char
            var b0 = GetCard8();
            // decode according to the type1/type2 format
            if (b0 == 28) // the two next bytes represent a short int;
            {
                int first = GetCard8();
                int second = GetCard8();
                Args[ArgCount] = (first << 8) | second;
                ArgCount++;
                continue;
            }

            if (b0 >= 32 && b0 <= 246) // The byte read is the byte;
            {
                Args[ArgCount] = b0 - 139;
                ArgCount++;
                continue;
            }

            if (b0 >= 247 && b0 <= 250) // The byte read and the next byte constetute a short int
            {
                int w = GetCard8();
                Args[ArgCount] = (b0 - 247) * 256 + w + 108;
                ArgCount++;
                continue;
            }

            if (b0 >= 251 && b0 <= 254) // Same as above except negative
            {
                int w = GetCard8();
                Args[ArgCount] = -(b0 - 251) * 256 - w - 108;
                ArgCount++;
                continue;
            }

            if (b0 == 255) // The next for bytes represent a double.
            {
                int first = GetCard8();
                int second = GetCard8();
                int third = GetCard8();
                int fourth = GetCard8();
                Args[ArgCount] = (first << 24) | (second << 16) | (third << 8) | fourth;
                ArgCount++;
                continue;
            }

            if (b0 <= 31 && b0 != 28) // An operator was found.. Set Key.
            {
                gotKey = true;
                // 12 is an escape command therefor the next byte is a part
                // of this command
                if (b0 == 12)
                {
                    int b1 = GetCard8();
                    if (b1 > SubrsEscapeFuncs.Length - 1)
                    {
                        b1 = SubrsEscapeFuncs.Length - 1;
                    }

                    Key = SubrsEscapeFuncs[b1];
                }
                else
                {
                    Key = SubrsFunctions[b0];
                }
            }
        }
    }

    /// <summary>
    ///     Read the FDArray count, offsize and Offset array
    /// </summary>
    /// <param name="font"></param>
    protected void ReadFdArray(int font)
    {
        Seek(Fonts[font].FdarrayOffset);
        Fonts[font].FdArrayCount = GetCard16();
        Fonts[font].FdArrayOffsize = GetCard8();
        // Since we will change values inside the FDArray objects
        // We increase its offsize to prevent errors
        if (Fonts[font].FdArrayOffsize < 4)
        {
            Fonts[font].FdArrayOffsize++;
        }

        Fonts[font].FdArrayOffsets = GetIndex(Fonts[font].FdarrayOffset);
    }

    /// <summary>
    ///     Read the FDSelect of the font and compute the array and its length
    /// </summary>
    /// <param name="font">The index of the font being processed</param>
    /// <returns>The Processed FDSelect of the font</returns>
    protected void ReadFdSelect(int font)
    {
        // Restore the number of glyphs
        var numOfGlyphs = Fonts[font].Nglyphs;
        var fdSelect = new int[numOfGlyphs];
        // Go to the beginning of the FDSelect
        Seek(Fonts[font].FdselectOffset);
        // Read the FDSelect's format
        Fonts[font].FdSelectFormat = GetCard8();

        switch (Fonts[font].FdSelectFormat)
        {
            // Format==0 means each glyph has an entry that indicated
            // its FD.
            case 0:
                for (var i = 0; i < numOfGlyphs; i++)
                {
                    fdSelect[i] = GetCard8();
                }

                // The FDSelect's Length is one for each glyph + the format
                // for later use
                Fonts[font].FdSelectLength = Fonts[font].Nglyphs + 1;
                break;
            case 3:
                // Format==3 means the ranges version
                // The number of ranges
                int nRanges = GetCard16();
                var l = 0;
                // Read the first in the first range
                int first = GetCard16();
                for (var i = 0; i < nRanges; i++)
                {
                    // Read the FD index
                    int fd = GetCard8();
                    // Read the first of the next range
                    int last = GetCard16();
                    // Calc the steps and write to the array
                    var steps = last - first;
                    for (var k = 0; k < steps; k++)
                    {
                        fdSelect[l] = fd;
                        l++;
                    }

                    // The last from this iteration is the first of the next
                    first = last;
                }

                // Store the length for later use
                Fonts[font].FdSelectLength = 1 + 2 + nRanges * 3 + 2;
                break;
        }

        // Save the FDSelect of the font
        Fonts[font].FdSelect = fdSelect;
    }

    /// <summary>
    ///     Function checks the key and return the change to the stack after the operator
    /// </summary>
    /// <returns>The change in the stack. 2-> flush the stack</returns>
    protected int StackOpp()
    {
        if (Key == "ifelse")
        {
            return -3;
        }

        if (Key == "roll" || Key == "put")
        {
            return -2;
        }

        if (Key == "callsubr" || Key == "callgsubr" || Key == "add" || Key == "sub" ||
            Key == "div" || Key == "mul" || Key == "drop" || Key == "and" ||
            Key == "or" || Key == "eq")
        {
            return -1;
        }

        if (Key == "abs" || Key == "neg" || Key == "sqrt" || Key == "exch" ||
            Key == "index" || Key == "get" || Key == "not" || Key == "return")
        {
            return 0;
        }

        if (Key == "random" || Key == "dup")
        {
            return 1;
        }

        return 2;
    }

    /// <summary>
    ///     Function calculates the number of ranges in the Charset
    /// </summary>
    /// <param name="numofGlyphs">The number of glyphs in the font</param>
    /// <param name="type">The format of the Charset</param>
    /// <returns>The number of ranges in the Charset data structure</returns>
    private int countRange(int numofGlyphs, int type)
    {
        var num = 0;
        char sid;
        int i = 1, nLeft;
        while (i < numofGlyphs)
        {
            num++;
            sid = GetCard16();
            if (type == 1)
            {
                nLeft = GetCard8();
            }
            else
            {
                nLeft = GetCard16();
            }

            i += nLeft + 1;
        }

        return num;
    }

    /// <summary>
    ///     Function reconstructs the FDArray, PrivateDict and LSubr for CID fonts
    ///     @throws IOException
    /// </summary>
    /// <param name="font">the font</param>
    private void reconstruct(int font)
    {
        // Init for later use
        OffsetItem[] fdPrivate = new DictOffsetItem[Fonts[font].FdArrayOffsets.Length - 1];
        var fdPrivateBase = new IndexBaseItem[Fonts[font].FdprivateOffsets.Length];
        OffsetItem[] fdSubrs = new DictOffsetItem[Fonts[font].FdprivateOffsets.Length];
        // Reconstruct each type
        reconstructFdArray(font, fdPrivate);
        ReconstructPrivateDict(font, fdPrivate, fdPrivateBase, fdSubrs);
        ReconstructPrivateSubrs(font, fdPrivateBase, fdSubrs);
    }

    /// <summary>
    ///     Function subsets the FDArray and builds the new one with new offsets
    ///     @throws IOException
    /// </summary>
    /// <param name="font">The font</param>
    /// <param name="fdPrivate">OffsetItem Array (one for each FDArray)</param>
    private void reconstructFdArray(int font, OffsetItem[] fdPrivate)
    {
        // Build the header of the index
        BuildIndexHeader(Fonts[font].FdArrayCount, Fonts[font].FdArrayOffsize, 1);

        // For each offset create an Offset Item
        OffsetItem[] fdOffsets = new IndexOffsetItem[Fonts[font].FdArrayOffsets.Length - 1];
        for (var i = 0; i < Fonts[font].FdArrayOffsets.Length - 1; i++)
        {
            fdOffsets[i] = new IndexOffsetItem(Fonts[font].FdArrayOffsize);
            OutputList.Add(fdOffsets[i]);
        }

        // Declare beginning of the object array
        var fdArrayBase = new IndexBaseItem();
        OutputList.Add(fdArrayBase);

        // For each object check if that FD is used.
        // if is used build a new one by changing the private object
        // Else do nothing
        // At the end of each object mark its ending (Even if wasn't written)
        for (var k = 0; k < Fonts[font].FdArrayOffsets.Length - 1; k++)
        {
            if (FdArrayUsed.ContainsKey(k))
            {
                // Goto begining of objects
                Seek(Fonts[font].FdArrayOffsets[k]);
                while (GetPosition() < Fonts[font].FdArrayOffsets[k + 1])
                {
                    var p1 = GetPosition();
                    GetDictItem();
                    var p2 = GetPosition();
                    // If the dictItem is the "Private" then compute and copy length,
                    // use marker for offset and write operator number
                    if (Key == "Private")
                    {
                        // Save the original length of the private dict
                        var newSize = (int)Args[0];
                        // Save the size of the offset to the subrs in that private
                        var orgSubrsOffsetSize =
                            CalcSubrOffsetSize(Fonts[font].FdprivateOffsets[k], Fonts[font].FdprivateLengths[k]);
                        // Increase the private's length accordingly
                        if (orgSubrsOffsetSize != 0)
                        {
                            newSize += 5 - orgSubrsOffsetSize;
                        }

                        // Insert the new size, OffsetItem and operator key number
                        OutputList.Add(new DictNumberItem(newSize));
                        fdPrivate[k] = new DictOffsetItem();
                        OutputList.Add(fdPrivate[k]);
                        OutputList.Add(new UInt8Item((char)18)); // Private
                        // Go back to place
                        Seek(p2);
                    }
                    // Else copy the entire range
                    else // other than private
                    {
                        OutputList.Add(new RangeItem(Buf, p1, p2 - p1));
                    }
                }
            }

            // Mark the ending of the object (even if wasn't written)
            OutputList.Add(new IndexMarkerItem(fdOffsets[k], fdArrayBase));
        }
    }
}