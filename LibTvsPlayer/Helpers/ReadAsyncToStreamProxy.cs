using LibTvsPlayer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibTvsPlayer.Helpers
{
    public class ReadAsyncToStreamProxy : Stream
    {
        private readonly ReadAsyncDelegate _readAsync;
        private readonly long _begin;
        private readonly int _size;
        private int _remain;

        public ReadAsyncToStreamProxy(ReadAsyncDelegate readAsync, long begin, int size)
        {
            _readAsync = readAsync;
            _begin = begin;
            _size = size;
            _remain = size;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _size;

        public override long Position
        {
            get => _size - _remain;
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
            // noop
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int toRead = Math.Max(0, Math.Min(count, _remain));

            _readAsync(
                buffer: buffer.AsMemory(offset, toRead),
                position: _begin + _size - _remain
            )
                .GetAwaiter()
                .GetResult();

            _remain -= toRead;
            return toRead;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seek is not supported on ReadAsyncToStreamProxy");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("SetLength is not supported on ReadAsyncToStreamProxy");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("Write is not supported on ReadAsyncToStreamProxy");
        }
    }
}
