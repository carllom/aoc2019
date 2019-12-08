using Advent.Of.Code;
using Advent.Of.Code.IntCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Day7
{
    internal class Amplification : AbstractAocTask
    {
        public override void First()
        {
            var maxAmp = int.MinValue;
            var maxPhase = "";
            var icms = new[]
            {
                new IntCodeMachine(),
                new IntCodeMachine(),
                new IntCodeMachine(),
                new IntCodeMachine(),
                new IntCodeMachine()
            };

            foreach (var perm in GetPermutations(new[] { 0, 1, 2, 3, 4 }, 5))
            {
                var next = 0; // Start input is 0

                for (int amp = 0; amp < 5; amp++)
                {
                    var icm = icms[amp];

                    // (Re)initialize streams and machine state
                    icm.Init("Day7/amplifier.ic");

                    // Fill instream with input
                    icm.Input.Write(perm.ElementAt(amp)); // Phase setting
                    icm.Input.Write(next); // Previous input

                    icm.Run();

                    next = icm.Output.Read(); // Get output
                }
                //var next = RunAmplifier(perm);
                maxAmp = Math.Max(next, maxAmp);
                if (maxAmp == next) maxPhase = perm.Aggregate("", (acc, val) => $"{acc}{val}");
            }
            Echo($"Max amplification: {maxAmp} @ {maxPhase}");
            ValidateAnswer(maxAmp, 914828);
        }

        private IEnumerable<IEnumerable<int>> GetPermutations(IEnumerable<int> phases, int numtochoose)
        {
            // We should not choose anything more
            if (numtochoose == 0) return Array.Empty<int[]>();

            // We do not have a choice
            if (phases.Count() == 1)
                return new[] { phases };

            // Iterate over phases
            List<IEnumerable<int>> permut = new List<IEnumerable<int>>();
            for (int i = 0; i < phases.Count(); i++)
            {
                var phase = phases.ElementAt(i);
                permut.AddRange(GetPermutations(phases.Where(p => p != phase), numtochoose - 1).Select(per => per.Prepend(phase)));
            }
            return permut.ToArray();
        }

        public override void Second()
        {
            var maxAmp = int.MinValue;
            var maxPhase = "";
            var icios = new[]
            {
                new IntCodeIOStream(),
                new IntCodeIOStream(),
                new IntCodeIOStream(),
                new IntCodeIOStream(),
                new IntCodeIOStream()
            };
            var icms = new[] {
                new IntCodeMachine(icios[0], icios[1]),
                new IntCodeMachine(icios[1], icios[2]),
                new IntCodeMachine(icios[2], icios[3]),
                new IntCodeMachine(icios[3], icios[4]),
                new IntCodeMachine(icios[4], icios[0])
            };

            foreach (var perm in GetPermutations(new[] { 5, 6, 7, 8, 9 }, 5))
            {
                // Initialize streams and machine state
                foreach (var icm in icms) { icm.Init("Day7/amplifier.ic"); }

                for (int i = 0; i < perm.Count(); i++)
                {
                    icms[i].Input.Write(perm.ElementAt(i));
                }
                icms[0].Input.Write(0);

                // Step as long as all machines are running
                while (true) {
                    var running = false;
                    foreach (var icm in icms)
                    {
                        running |= icm.Step();
                    }
                    if (!running) break;
                }

                var result = icms[4].Output.Read();

                //var next = RunAmplifier(perm);
                maxAmp = Math.Max(result, maxAmp);
                if (maxAmp == result) maxPhase = perm.Aggregate("", (acc, val) => $"{acc}{val}");
            }
            Echo($"Max feedback amplification: {maxAmp} @ {maxPhase}");
            ValidateAnswer(maxAmp, 17956613);
        }
    }
}