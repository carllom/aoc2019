using System;
using System.Collections.Generic;
using System.Text;
using Advent.Of.Code.IntCode;

namespace Advent.Of.Code.Day5
{
    class Diagnostics : AbstractAocTask
    {
        IntCodeMachine icm = new IntCodeMachine();

        public override void First()
        {
            Echo("Provide code 1");
            icm.Init("Day5/diagnostics.ic");
            icm.Input.Write(1);
            icm.Run();
        }

        public override void Second()
        {
            Echo("Provide code 5");
            icm.Init("Day5/diagnostics.ic");
            icm.Input.Write(5);
            icm.Run();
        }
    }
}
