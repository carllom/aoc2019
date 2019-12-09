using System.Collections.Generic;

namespace Advent.Of.Code.IntCode
{
    interface IICReader
    {
        bool CanRead { get; }
        long Read();
    }

    interface IICWriter
    {
        void Write(long value);
    }

    class IntCodeIOStream : IICReader, IICWriter
    {
        private Queue<long> buffer = new Queue<long>();

        public void Clear() { buffer.Clear(); }

        public void Write(long value)
        {
            buffer.Enqueue(value);
        }

        public bool CanRead { get { return buffer.Count > 0; } }

        public long Read()
        {
            return buffer.Dequeue();
        }
    }
}
