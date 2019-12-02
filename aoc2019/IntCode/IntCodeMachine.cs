using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Advent.Of.Code.IntCode
{
    class IntCodeMachine
    {
        public int pc;
        public int[] mem;

        public void Init(string path)
        {
            pc = 0;
            mem = File.ReadAllText(path).Split(',').Select(a => int.Parse(a)).ToArray();
        }

        public void Run()
        {
            while (true)
            {
                switch (mem[pc])
                {
                    case 1:
                        mem[mem[pc + 3]] = mem[mem[pc + 1]] + mem[mem[pc + 2]];
                        pc += 4;
                        break;
                    case 2:
                        mem[mem[pc + 3]] = mem[mem[pc + 1]] * mem[mem[pc + 2]];
                        pc += 4;
                        break;
                    case 99:
                        return;
                    default:
                        throw new ApplicationException($"Illegal opcode {mem[pc - 1]} @{pc - 1}");
                }
            }
        }
    }
}
