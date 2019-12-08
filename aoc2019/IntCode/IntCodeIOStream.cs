using System.Collections.Generic;

namespace Advent.Of.Code.IntCode
{
    interface IICReader
    {
        bool CanRead { get; }
        int Read();
    }

    interface IICWriter
    {
        void Write(int value);
    }

    class IntCodeIOStream : IICReader, IICWriter
    {
        private Queue<int> buffer = new Queue<int>();

        public void Clear() { buffer.Clear(); }

        public void Write(int value)
        {
            buffer.Enqueue(value);
        }

        public bool CanRead { get { return buffer.Count > 0; } }

        public int Read()
        {
            return buffer.Dequeue();
        }
    }
}
