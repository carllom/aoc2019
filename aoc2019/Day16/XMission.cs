using Advent.Of.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Day16
{
    internal class XMission : AbstractAocTask
    {
        const string input = "59717513948900379305109702352254961099291386881456676203556183151524797037683068791860532352118123252250974130706958763348105389034831381607519427872819735052750376719383812473081415096360867340158428371353702640632449827967163188043812193288449328058464005995046093112575926165337330100634707115160053682715014464686531460025493602539343245166620098362467196933484413717749680188294435582266877493265037758875197256932099061961217414581388227153472347319505899534413848174322474743198535953826086266146686256066319093589456135923631361106367290236939056758783671975582829257390514211329195992209734175732361974503874578275698611819911236908050184158";

        static readonly int[] baseptn = { 0, 1, 0, -1 };

        public override void First()
        {
            var data = input.Select(c => c - '0').ToArray();
            //var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var data2 = new int[data.Length];
            for (int phase = 1; phase <= 100; phase++)
            {
                CalcPhase2(data, data2);
                Swap(ref data, ref data2);
            }
            var first8 = Take8(data);
            Echo($"First 8 digits of input after 100 phases: {first8}");
            ValidateAnswer(first8, "63794407");
        }

        private void Swap(ref int[] a, ref int[] b)
        {
            var tmp = a; a = b; b = tmp;
        }

        /// <summary>
        /// Naive implementation - takes forever
        /// </summary>
        /// <param name="dataIn"></param>
        /// <param name="dataOut"></param>
        private void CalcPhase(int[] dataIn, int[] dataOut)
        {
            for (int mult = 1; mult <= dataIn.Length; mult++)
            {
                int digit = 0;
                for (int srcPos = 0; srcPos < dataIn.Length; srcPos++)
                {
                    digit += baseptn[((srcPos + 1) / mult) % baseptn.Length] * dataIn[srcPos];
                }
                dataOut[mult-1] = Math.Abs(digit) % 10;
            }
        }

        /// <summary>
        /// Slightly better - we use efficient data types and skip 0 values
        /// </summary>
        /// <param name="dataIn"></param>
        /// <param name="dataOut"></param>
        private void CalcPhase2(int[] dataIn, int[] dataOut)
        {
            for (int mult = 1; mult <= dataIn.Length; mult++)
            {
                int digit = 0;
                int srcPos = -1;
                var len = dataIn.Length;
                while (srcPos < len)
                {
                    srcPos += mult; // advance p0 (=0 skip)
                    for (int p1 = 0; p1 < mult; p1++)
                    {
                        if (srcPos + p1 >= len) break;
                        digit += dataIn[srcPos + p1]; // p1 = 1
                    }
                    srcPos += mult; // advance p1
                    srcPos += mult; // advance p2 (=0 skip)
                    for (int p3 = 0; p3 < mult; p3++)
                    {
                        if (srcPos + p3 >= len) break;
                        digit -= dataIn[srcPos + p3];
                    }
                    srcPos += mult; // advance p3
                }
                if (mult % 10000 == 0) Console.Write('.');
                dataOut[mult - 1] = Math.Abs(digit) % 10;
            }
        }

        private void CalcPhase3(int[] dataIn, int[] dataOut)
        {
            for (int mult = 1; mult <= dataIn.Length; mult++)
            {
                int digit = 0;
                int srcPos = -1;
                var len = MultFor(input.Length,mult*4); // Only do up to the value when it repeats
                while (srcPos < len)
                {
                    srcPos += mult; // advance p0 (=0 skip)
                    for (int p1 = 0; p1 < mult; p1++)
                    {
                        if (srcPos + p1 >= len) break;
                        digit += dataIn[srcPos + p1]; // p1 = 1
                    }
                    srcPos += mult; // advance p1
                    srcPos += mult; // advance p2 (=0 skip)
                    for (int p3 = 0; p3 < mult; p3++)
                    {
                        if (srcPos + p3 >= len) break;
                        digit -= dataIn[srcPos + p3];
                    }
                    srcPos += mult; // advance p3
                }
                dataOut[mult - 1] = Math.Abs(digit) % 10;
            }
        }

        /// <summary>
        /// Calculate a positive fan 
        /// </summary>
        /// <param name="dataIn">input array</param>
        /// <param name="dataOut">output array</param>
        /// <param name="startIdxIn">input start index ("x position")</param>
        /// <param name="startIdxOut">output start index ("y position"). Also infers width of fan</param>
        /// <param name="leftSlant">steps to the right to add when you go up to the next index</param>
        /// <param name="rightSlant">steps to the left to remove when you go down to the previous index</param>
        private void CalcPositiveFan(int[] dataIn, int[] dataOut, int startIdxIn, int startIdxOut, int leftSlant, int rightSlant)
        {
            var idxOut = startIdxOut;
            var lEdge = startIdxIn;
            var lastData = 0;
            while (idxOut > leftSlant)
            {
                dataOut[idxOut] = lastData; // Copy the old value

                // Add leftSlant entries to the new
                for (int i = 1; i <= leftSlant; i++)
                {
                    dataOut[idxOut] += dataIn[lEdge - i];
                }
                // Remove rightSlant (leftSlant+1) entries from the new value
                for (int i = 1; i <= leftSlant + 1; i++)
                {
                    var rPos = lEdge + startIdxOut - i;
                    if (rPos < dataOut.Length)
                        dataOut[idxOut] -= dataIn[rPos]; 
                }
                dataOut[idxOut] = dataOut[idxOut] % 10;
                lastData = dataOut[idxOut];
                idxOut--;
                lEdge -= leftSlant;
            }
        }


        private string Take8(int[] array, int offset=0)
        {
            return array.Skip(offset).Take(8).Aggregate("", (acc, i) => acc + i);
        }

        private int MaxMult(int arrLength)
        {
            var maxN = 0;
            for (int i = 1; i <= input.Length; i++)
            {
                var mult = MultFor(input.Length, i * 4);
                maxN = (maxN < mult) ? mult : maxN;
            }
            return maxN;
        }

        private int MultFor(int arrLength, int convSize) 
        {
            int mult = 1;
            while ((mult * arrLength) % convSize != 0) mult++;
            return mult;
        }

        public override void Second()
        {
            var data0 = input.Select(c => c - '0').ToArray();
            var mult = 10000;//MaxMult(data0.Length); // Don't do 10000 - it repeats after MaxMult() (abount 2*data0.Length) repetitions anyway
            var data = new int[data0.Length * mult]; 
            var data2 = new int[data.Length];
            for (int i = 0; i < mult; i++)
            {
                Array.Copy(data0, 0, data, i * data0.Length, data0.Length);
            }
            var sw = new System.Diagnostics.Stopwatch();
            for (int phase = 1; phase <= 100; phase++)
            {
                sw.Reset();
                sw.Start();
                CalcPositiveFan(data, data2, data.Length, data.Length - 1, 1, 2);
                CalcPhase2(data, data2);
                sw.Stop();
                Echo($"Phase took {sw.Elapsed}");
                Swap(ref data, ref data2);
            }
            var offset = int.Parse(Take8(data0));
            var result = Take8(data, offset);
            Echo($"Decoded message: {result}");
            ValidateAnswer(result, "00000000");
        }
    }
}