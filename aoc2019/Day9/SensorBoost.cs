using Advent.Of.Code;
using Advent.Of.Code.IntCode;

namespace Day9
{
    internal class SensorBoost : AbstractAocTask
    {
        public SensorBoost()
        {
        }

        public override void First()
        {
            var icm = new IntCodeMachine();
            icm.Init("Day9/boost.ic");
            //icm.Trace = true;
            icm.Input.Write(1); // Test code
            icm.Run();
            var testOutput = icm.Output.Read();
            Echo($"debug output: {testOutput}");
            ValidateAnswer(testOutput, 2775723069);
        }

        public override void Second()
        {
            var icm = new IntCodeMachine();
            icm.Init("Day9/boost.ic");
            icm.Input.Write(2); // Boost code
            icm.Run();
            var boostcode = icm.Output.Read();
            Echo($"output: {boostcode}");
            ValidateAnswer(boostcode, 49115);
        }
    }
}