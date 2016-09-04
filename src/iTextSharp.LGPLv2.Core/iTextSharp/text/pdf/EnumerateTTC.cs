using System.Collections;

namespace iTextSharp.text.pdf
{

    /// <summary>
    /// Enumerates all the fonts inside a True Type Collection.
    /// @author  Paulo Soares (psoares@consiste.pt)
    /// </summary>
    internal class EnumerateTtc : TrueTypeFont
    {
        protected string[] names;

        internal EnumerateTtc(string ttcFile)
        {
            FileName = ttcFile;
            Rf = new RandomAccessFileOrArray(ttcFile);
            FindNames();
        }

        internal EnumerateTtc(byte[] ttcArray)
        {
            FileName = "Byte array TTC";
            Rf = new RandomAccessFileOrArray(ttcArray);
            FindNames();
        }

        internal string[] Names
        {
            get
            {
                return names;
            }
        }

        internal void FindNames()
        {
            Tables = new Hashtable();

            try
            {
                string mainTag = ReadStandardString(4);
                if (!mainTag.Equals("ttcf"))
                    throw new DocumentException(FileName + " is not a valid TTC file.");
                Rf.SkipBytes(4);
                int dirCount = Rf.ReadInt();
                names = new string[dirCount];
                int dirPos = Rf.FilePointer;
                for (int dirIdx = 0; dirIdx < dirCount; ++dirIdx)
                {
                    Tables.Clear();
                    Rf.Seek(dirPos);
                    Rf.SkipBytes(dirIdx * 4);
                    DirectoryOffset = Rf.ReadInt();
                    Rf.Seek(DirectoryOffset);
                    if (Rf.ReadInt() != 0x00010000)
                        throw new DocumentException(FileName + " is not a valid TTF file.");
                    int numTables = Rf.ReadUnsignedShort();
                    Rf.SkipBytes(6);
                    for (int k = 0; k < numTables; ++k)
                    {
                        string tag = ReadStandardString(4);
                        Rf.SkipBytes(4);
                        int[] tableLocation = new int[2];
                        tableLocation[0] = Rf.ReadInt();
                        tableLocation[1] = Rf.ReadInt();
                        Tables[tag] = tableLocation;
                    }
                    names[dirIdx] = BaseFont;
                }
            }
            finally
            {
                if (Rf != null)
                    Rf.Close();
            }
        }
    }
}