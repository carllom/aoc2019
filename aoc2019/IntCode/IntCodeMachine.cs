using System;
using System.IO;
using System.Linq;

namespace Advent.Of.Code.IntCode
{
    class IntCodeMachine
    {
        public bool Trace;
        public long pc; // Program counter
        public long bp; // Base pointer
        public IntCodeVirtMem mem = new IntCodeVirtMem();

        public bool WantInput { get; private set; }
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

        public void Init(long[] data)
        {
            pc = 0; bp = 0;
            mem.Load(0, data);
            input.Clear();
            output.Clear();
        }

        public void Init(string path)
        {
            Init(File.ReadAllText(path).Split(',').Select(a => long.Parse(a)).ToArray());
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
                    DAsm($"@{Oper(3)} = {Op1} + {Op2} ({Instr} {Oper(1)} {Oper(2)} {Oper(3)})");
                    Op3 = Op1 + Op2;
                    pc += 4;
                    break;
                case 2: // MUL A * B => C
                    DAsm($"@{Oper(3)} = {Op1} * {Op2} ({Instr} {Oper(1)} {Oper(2)} {Oper(3)})");
                    Op3 = Op1 * Op2;
                    pc += 4;
                    break;
                case 3: // INP => A
                    DAsm($"@{Oper(1)} <= input ({Instr} {Oper(1)})");
                    if (!input.CanRead)
                    {
                        WantInput = true;
                        return true; // Do not step if we are missing input
                    }
                    WantInput = false;
                    var s = input.Read();
                    //Console.WriteLine($"? {s}");
                    Op1 = s;
                    pc += 2;
                    break;
                case 4: // OUT <= A
                    DAsm($"{Op1} => output ({Instr} {Oper(1)})");
                    //Console.WriteLine($"! {Op1Val}");
                    output.Write(Op1);
                    pc += 2;
                    break;
                case 5: // JT => A!=0 ? pc=B
                    DAsm($"PC = {Op1}? {Op2} ({Instr} {Oper(1)} {Oper(2)})");
                    if (Op1 != 0)
                        pc = Op2;
                    else
                        pc += 3;
                    break;
                case 6: // JF => A==0 ? pc=B
                    DAsm($"PC = !{Op1}? {Op2} ({Instr} {Oper(1)} {Oper(2)})");
                    if (Op1 == 0)
                        pc = Op2;
                    else
                        pc += 3;
                    break;
                case 7: // LT A<B => C
                    DAsm($"@{Oper(3)} = {Op1}<{Op2}? 1 : 0 ({Instr} {Oper(1)} {Oper(2)} {Oper(3)})");
                    Op3 = (Op1 < Op2) ? 1 : 0;
                    pc += 4;
                    break;
                case 8: // EQ A==B => C
                    DAsm($"@{Oper(3)} = {Op1}=={Op2}? 1 : 0 ({Instr} {Oper(1)} {Oper(2)} {Oper(3)})");
                    Op3 = (Op1 == Op2) ? 1 : 0;
                    pc += 4;
                    break;
                case 9:
                    DAsm($"BP += {Op1} ({Instr} {Oper(1)})");
                    bp += Op1;
                    pc += 2;
                    break;
                case 99:
                    return false;
                default:
                    throw new ApplicationException($"Illegal opcode {mem[pc - 1]} @{pc - 1}");
            }
            return true;
        }

        private void DAsm(string msg)
        {
            if (Trace) Console.WriteLine($"[{pc:0000}] {msg}");
        }

        private enum AddrMode
        {
            Memory = 0, // Memory address at
            Immediate = 1,
            BaseRelative = 2
        }

        private long Instr => mem[pc];
        private long Oper(int opnum) => mem[pc + opnum];
        private AddrMode AddressMode(int opnum) => (AddrMode)(Instr / Convert.ToInt32(Math.Pow(10, opnum + 1)) % 10);

        private long Op1 {
            get => Op(1);
            set => Op(1, value);
        }
        private long Op2
        {
            get => Op(2);
            set => Op(2, value);
        }
        private long Op3
        {
            get => Op(3);
            set => Op(3, value);
        }

        /// <summary>
        /// Get source operand value
        /// </summary>
        /// <param name="opnum"></param>
        /// <returns></returns>
        private long Op(int opnum)
        {
            var oper = Oper(opnum);
            var mode = AddressMode(opnum);
            switch (mode)
            {
                case AddrMode.Memory:
                    return mem[oper];
                case AddrMode.Immediate:
                    return oper;
                case AddrMode.BaseRelative:
                    return mem[bp + oper];
                default:
                    throw new NotImplementedException($"Address mode {mode}");
            }
        }

        /// <summary>
        /// Set destination operand value
        /// </summary>
        /// <param name="opnum"></param>
        /// <param name="value"></param>
        private void Op(int opnum, long value)
        {
            var oper = Oper(opnum);
            var mode = AddressMode(opnum);
            switch (mode)
            {
                case AddrMode.Memory:
                    mem[oper] = value;
                    break;
                case AddrMode.Immediate:
                    throw new ArgumentException($"@PC={pc}: {mode} cannot be used as a target address mode", nameof(mode));
                case AddrMode.BaseRelative:
                    mem[bp + oper] = value;
                    break;
                default:
                    throw new NotImplementedException($"Address mode {mode} is not implemented");
            }
        }
    }
}
