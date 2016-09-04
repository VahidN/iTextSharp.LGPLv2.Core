using System;
using System.Text;
using System.Collections;
using System.IO;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Summary description for ICC_Profile.
    /// </summary>
    public class IccProfile
    {
        protected byte[] data;
        protected int numComponents;
        private static readonly Hashtable _cstags = new Hashtable();

        static IccProfile()
        {
            _cstags["XYZ "] = 3;
            _cstags["Lab "] = 3;
            _cstags["Luv "] = 3;
            _cstags["YCbr"] = 3;
            _cstags["Yxy "] = 3;
            _cstags["RGB "] = 3;
            _cstags["GRAY"] = 1;
            _cstags["HSV "] = 3;
            _cstags["HLS "] = 3;
            _cstags["CMYK"] = 4;
            _cstags["CMY "] = 3;
            _cstags["2CLR"] = 2;
            _cstags["3CLR"] = 3;
            _cstags["4CLR"] = 4;
            _cstags["5CLR"] = 5;
            _cstags["6CLR"] = 6;
            _cstags["7CLR"] = 7;
            _cstags["8CLR"] = 8;
            _cstags["9CLR"] = 9;
            _cstags["ACLR"] = 10;
            _cstags["BCLR"] = 11;
            _cstags["CCLR"] = 12;
            _cstags["DCLR"] = 13;
            _cstags["ECLR"] = 14;
            _cstags["FCLR"] = 15;
        }

        protected IccProfile()
        {
        }

        public byte[] Data
        {
            get
            {
                return data;
            }
        }

        public int NumComponents
        {
            get
            {
                return numComponents;
            }
        }

        public static IccProfile GetInstance(byte[] data)
        {
            if (data.Length < 128 | data[36] != 0x61 || data[37] != 0x63
                || data[38] != 0x73 || data[39] != 0x70)
                throw new ArgumentException("Invalid ICC profile");
            IccProfile icc = new IccProfile();
            icc.data = data;
            object cs = _cstags[Encoding.ASCII.GetString(data, 16, 4)];
            icc.numComponents = (cs == null ? 0 : (int)cs);
            return icc;
        }

        public static IccProfile GetInstance(Stream file)
        {
            byte[] head = new byte[128];
            int remain = head.Length;
            int ptr = 0;
            while (remain > 0)
            {
                int n = file.Read(head, ptr, remain);
                if (n <= 0)
                    throw new ArgumentException("Invalid ICC profile");
                remain -= n;
                ptr += n;
            }
            if (head[36] != 0x61 || head[37] != 0x63
                || head[38] != 0x73 || head[39] != 0x70)
                throw new ArgumentException("Invalid ICC profile");
            remain = ((head[0] & 0xff) << 24) | ((head[1] & 0xff) << 16)
                      | ((head[2] & 0xff) << 8) | (head[3] & 0xff);
            byte[] icc = new byte[remain];
            Array.Copy(head, 0, icc, 0, head.Length);
            remain -= head.Length;
            ptr = head.Length;
            while (remain > 0)
            {
                int n = file.Read(icc, ptr, remain);
                if (n <= 0)
                    throw new ArgumentException("Invalid ICC profile");
                remain -= n;
                ptr += n;
            }
            return GetInstance(icc);
        }

        public static IccProfile GetInstance(string fname)
        {
            FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read);
            IccProfile icc = GetInstance(fs);
            fs.Dispose();
            return icc;
        }
    }
}
