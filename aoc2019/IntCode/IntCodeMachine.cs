using System;
using System.IO;
using System.Linq;

namespace Advent.Of.Code.IntCode
{
    class IntCodeMachine
    {
        public int pc;
        public int[] mem;
        public IICWriter Input => input;
        public IICReader Output => output;

        private readonly IntCodeIOStream input;
        private readonly IntCodeIOStream output;

        public IntCodeMachine() : this(new IntCodeIOStream(), new IntCodeIOStream()) { }

        public IntCodeMachine(IntCodeIOStream input, IntCodeIOStream output)
        {
            this.input = input;
            this.output = output;
        }

        public void Init(string path)
        {
            pc = 0;
            mem = File.ReadAllText(path).Split(',').Select(a => int.Parse(a)).ToArray();
            input.Clear();
            output.Clear();
        }

        public void Run()
        {
            while (true)
            {
                if (!Step()) return;
            }
        }

        public bool Step()
        {
            var opcode = Instr;
            switch (opcode % 100)
            {
                case 1: // ADD A + B => C
                    mem[Op3] = Op1Val + Op2Val;
                    pc += 4;
                    break;
                case 2: // MUL A * B => C
                    mem[Op3] = Op1Val * Op2Val;
                    pc += 4;
                    break;
                case 3: // INP => A
                    if (!input.CanRead) return true; // Do not step if we are missing input
                    var s = input.Read();
                    Console.WriteLine($"? {s}");
                    mem[Op1] = s;
                    pc += 2;
                    break;
                case 4: // OUT <= A
                    Console.WriteLine($"! {Op1Val}");
                    output.Write(Op1Val);
                    pc += 2;
                    break;
                case 5: // JT => A!=0 ? pc=B
                    if (Op1Val != 0)
                        pc = Op2Val;
                    else
                        pc += 3;
                    break;
                case 6: // JF => A==0 ? pc=B
                    if (Op1Val == 0)
                        pc = Op2Val;
                    else
                        pc += 3;
                    break;
                case 7: // LT A<B => C
                    mem[Op3] = (Op1Val < Op2Val) ? 1 : 0;
                    pc += 4;
                    break;
                case 8: // EQ A==B => C
                    mem[Op3] = (Op1Val == Op2Val) ? 1 : 0;
                    pc += 4;
                    break;
                case 99:
                    return false;
                default:
                    throw new ApplicationException($"Illegal opcode {mem[pc - 1]} @{pc - 1}");
            }
            return true;
        }

        private enum AddrMode
        {
            Memory = 0,
            Immediate = 1
        }

        private int Instr => mem[pc];
        private int Op1 => mem[pc + 1];
        private int Op2 => mem[pc + 2];
        private int Op3 => mem[pc + 3];

        private AddrMode AddressMode(int opcode, int opnum)
        {
            return (AddrMode)((opcode / Convert.ToInt32(Math.Pow(10, opnum+1)))%10);
        }

        private int Op1Val => OpVal(Op1, AddressMode(Instr, 1));
        private int Op2Val => OpVal(Op2, AddressMode(Instr, 2));
        private int Op3Val => OpVal(Op3, AddressMode(Instr, 3));


        private int OpVal(int op, AddrMode mode) {
            switch(mode)
            {
                case AddrMode.Memory:
                    return mem[op];
                case AddrMode.Immediate:
                    return op;
                default:
                    throw new NotImplementedException($"Address mode {mode}");
            }
        }
    }
}
