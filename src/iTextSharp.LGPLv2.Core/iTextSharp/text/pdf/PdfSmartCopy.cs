using System;
using System.IO;
using System.Collections;
using System.Security.Cryptography;

namespace iTextSharp.text.pdf
{

    /// <summary>
    /// PdfSmartCopy has the same functionality as PdfCopy,
    /// but when resources (such as fonts, images,...) are
    /// encountered, a reference to these resources is saved
    /// in a cache, so that they can be reused.
    /// This requires more memory, but reduces the file size
    /// of the resulting PDF document.
    /// </summary>
    public class PdfSmartCopy : PdfCopy
    {

        /// <summary>
        /// the cache with the streams and references.
        /// </summary>
        private readonly Hashtable _streamMap;

        /// <summary>
        /// Creates a PdfSmartCopy instance.
        /// </summary>
        public PdfSmartCopy(Document document, Stream os) : base(document, os)
        {
            _streamMap = new Hashtable();
        }
        /// <summary>
        /// Translate a PRIndirectReference to a PdfIndirectReference
        /// In addition, translates the object numbers, and copies the
        /// referenced object to the output file if it wasn't available
        /// in the cache yet. If it's in the cache, the reference to
        /// the already used stream is returned.
        /// NB: PRIndirectReferences (and PRIndirectObjects) really need to know what
        /// file they came from, because each file has its own namespace. The translation
        /// we do from their namespace to ours is *at best* heuristic, and guaranteed to
        /// fail under some circumstances.
        /// </summary>
        protected override PdfIndirectReference CopyIndirect(PrIndirectReference inp)
        {
            PdfObject srcObj = PdfReader.GetPdfObjectRelease(inp);
            ByteStore streamKey = null;
            bool validStream = false;
            if (srcObj.IsStream())
            {
                streamKey = new ByteStore((PrStream)srcObj);
                validStream = true;
                PdfIndirectReference streamRef = (PdfIndirectReference)_streamMap[streamKey];
                if (streamRef != null)
                {
                    return streamRef;
                }
            }

            PdfIndirectReference theRef;
            RefKey key = new RefKey(inp);
            IndirectReferences iRef = (IndirectReferences)Indirects[key];
            if (iRef != null)
            {
                theRef = iRef.Ref;
                if (iRef.Copied)
                {
                    return theRef;
                }
            }
            else
            {
                theRef = Body.PdfIndirectReference;
                iRef = new IndirectReferences(theRef);
                Indirects[key] = iRef;
            }
            if (srcObj != null && srcObj.IsDictionary())
            {
                PdfObject type = PdfReader.GetPdfObjectRelease(((PdfDictionary)srcObj).Get(PdfName.TYPE));
                if (type != null && PdfName.Page.Equals(type))
                {
                    return theRef;
                }
            }
            iRef.SetCopied();

            if (validStream)
            {
                _streamMap[streamKey] = theRef;
            }

            PdfObject obj = CopyObject(srcObj);
            AddToBody(obj, theRef);
            return theRef;
        }

        internal class ByteStore
        {
            private readonly byte[] _b;

            internal ByteStore(PrStream str)
            {
                ByteBuffer bb = new ByteBuffer();
                int level = 100;
                serObject(str, level, bb);
                _b = bb.ToByteArray();
            }

            public override bool Equals(object obj)
            {
                if (obj == null || !(obj is ByteStore))
                    return false;
                if (GetHashCode() != obj.GetHashCode())
                    return false;
                byte[] b2 = ((ByteStore)obj)._b;
                if (b2.Length != _b.Length)
                    return false;
                int len = _b.Length;
                for (int k = 0; k < len; ++k)
                {
                    if (_b[k] != b2[k])
                        return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                var hash = 0;
                int len = _b.Length;
                for (int k = 0; k < len; ++k)
                {
                    hash = hash * 31 + _b[k];
                }
                return hash;
            }

            private void serArray(PdfArray array, int level, ByteBuffer bb)
            {
                bb.Append("$A");
                if (level <= 0)
                    return;
                for (int k = 0; k < array.Size; ++k)
                {
                    serObject(array[k], level, bb);
                }
            }

            private void serDic(PdfDictionary dic, int level, ByteBuffer bb)
            {
                bb.Append("$D");
                if (level <= 0)
                    return;
                object[] keys = new object[dic.Size];
                dic.Keys.CopyTo(keys, 0);
                Array.Sort(keys);
                for (int k = 0; k < keys.Length; ++k)
                {
                    serObject((PdfObject)keys[k], level, bb);
                    serObject(dic.Get((PdfName)keys[k]), level, bb);
                }
            }

            private void serObject(PdfObject obj, int level, ByteBuffer bb)
            {
                if (level <= 0)
                    return;
                if (obj == null)
                {
                    bb.Append("$Lnull");
                    return;
                }
                obj = PdfReader.GetPdfObject(obj);
                if (obj.IsStream())
                {
                    bb.Append("$B");
                    serDic((PdfDictionary)obj, level - 1, bb);
                    if (level > 0)
                    {
                        using (var md5 = MD5BouncyCastle.Create())
                        {
                            bb.Append(md5.ComputeHash(PdfReader.GetStreamBytesRaw((PrStream)obj)));
                        }
                    }
                }
                else if (obj.IsDictionary())
                {
                    serDic((PdfDictionary)obj, level - 1, bb);
                }
                else if (obj.IsArray())
                {
                    serArray((PdfArray)obj, level - 1, bb);
                }
                else if (obj.IsString())
                {
                    bb.Append("$S").Append(obj.ToString());
                }
                else if (obj.IsName())
                {
                    bb.Append("$N").Append(obj.ToString());
                }
                else
                    bb.Append("$L").Append(obj.ToString());
            }
        }
    }
}