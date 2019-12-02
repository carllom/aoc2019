using Advent.Of.Code.IntCode;

namespace Advent.Of.Code.Day2
{
    class GravAssist : AbstractAocTask
    {
        private IntCodeMachine icm = new IntCodeMachine();

        public override void First()
        {
            icm.Init("Day2/gravassist.ic");
            icm.mem[1] = 12; icm.mem[2] = 2; // Patch code
            icm.Run();
            Echo($"Result {icm.mem[0]}");
            ValidateAnswer(icm.mem[0], 3790645);
        }

        public override void Second()
        {
            int noun, verb;
            for (int i = 0; i < 10000; i++)
            {
                icm.Init("Day2/gravassist.ic");
                icm.mem[1] = noun = i / 100;
                icm.mem[2] = verb = i % 100;
                icm.Run();
                if (icm.mem[0] == 19690720)
                {
                    Echo($"Solution for result 19690720 (noun = {noun}, verb = {verb}) is {i}");
                    ValidateAnswer(i, 6577);
                    break;
                }
            }
        }
    }
}
