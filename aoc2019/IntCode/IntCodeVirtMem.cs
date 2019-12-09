using System;
using System.Collections.Generic;

namespace Advent.Of.Code.IntCode
{
    class IntCodeVirtMem
    {
        private const int pageSize = 1024; // One page is 1Kibi words

        // LRU page - slight speedup on sequential access
        private long lastPageNumber = -1;
        private long[] lastPage = Array.Empty<long>();

        /// <summary>
        /// Allocated pages 
        /// </summary>
        private readonly Dictionary<long, long[]> pages = new Dictionary<long, long[]>();

        public long this[long index]
        {
            get { return PageFor(index)[index % pageSize]; }
            set { PageFor(index)[index % pageSize] = value; }
        }

        public void Load(long address, long[] block)
        {
            for(var i=0; i<block.Length; i++)
            {
                this[address + i] = block[i];
            }
        }

        private long[] PageFor(long address)
        {
            if (address < 0) throw new IndexOutOfRangeException($"Negative address {address} not allowed!");

            if (lastPageNumber == address / pageSize) return lastPage; // LRU item

            // We need to fetch/create page
            lastPageNumber = address / pageSize;
            if (pages.ContainsKey(lastPageNumber))
                lastPage = pages[lastPageNumber]; // Fetch from page list
            else
            {
                lastPage = new long[pageSize]; // Allocate the page..
                pages[lastPageNumber] = lastPage; // ..and add it to the page list
            }

            return lastPage;
        }
    }
}
