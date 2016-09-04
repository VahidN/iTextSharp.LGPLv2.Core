using System;
using System.IO;

namespace iTextSharp.text.rtf.parser
{
    public class PushbackStream : Stream
    {
        private readonly Stream _s;
        private int _buf = -1;
        public PushbackStream(Stream s)
        {
            _s = s;
        }

        public override bool CanRead
        {
            get { return _s.CanRead; }
        }
        public override bool CanSeek
        {
            get { return _s.CanSeek; }
        }
        public override bool CanWrite
        {
            get { return _s.CanWrite; }
        }
        public override long Length
        {
            get { return _s.Length; }
        }
        public override long Position
        {
            get { return _s.Position; }
            set { _s.Position = value; }
        }

        public override void Flush()
        {
            _s.Flush();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_buf != -1 && count > 0)
            {
                // TODO Can this case be made more efficient?
                buffer[offset] = (byte)_buf;
                _buf = -1;
                return 1;
            }

            return _s.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            if (_buf != -1)
            {
                int tmp = _buf;
                _buf = -1;
                return tmp;
            }

            return _s.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _s.Seek(offset, origin);
        }
        public override void SetLength(long value)
        {
            _s.SetLength(value);
        }
        public virtual void Unread(int b)
        {
            if (_buf != -1)
                throw new InvalidOperationException("Can only push back one byte");

            _buf = b & 0xFF;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _s.Write(buffer, offset, count);
        }
        public override void WriteByte(byte value)
        {
            _s.WriteByte(value);
        }
    }
}