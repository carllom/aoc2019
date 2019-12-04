using System;
using System.Collections.Generic;
using System.Text;

namespace Advent.Of.Code.Day4
{
    class SecureContainer : AbstractAocTask
    {
        const int from = 124075;
        const int to = 580769;

        public override void First()
        {
            int numValid = 0;
            for (int i = from; i <= to; i++)
            {
                if (IsValid(i)) numValid++;
            }
            Echo($"Number of valid codes: {numValid}");
            ValidateAnswer(numValid, 2150);
        }

        private bool IsValid(int value)
        {
            bool pair = false;
            int seqSize = 1;
            while (value > 0)
            {
                var dig0 = value % 10;
                value /= 10;
                var dig1 = value % 10;
                if (dig0 < dig1) return false;
                if (dig0 == dig1) pair = true;
            }
            return pair;
        }

        public override void Second()
        {
            int numValid = 0;
            for (int i = from; i <= to; i++)
            {
                if (IsValid2(i)) numValid++;
            }
            Echo($"Number of valid codes: {numValid}");
            ValidateAnswer(numValid, 1462);
        }

        private bool IsValid2(int value)
        {
            bool pair = false;
            int seqSize = 1;
            while (value > 0)
            {
                var dig0 = value % 10;
                value /= 10;
                var dig1 = value % 10;
                if (dig0 < dig1) return false;
                if (dig0 == dig1)
                    seqSize++;
                else
                {
                    if (seqSize == 2) pair = true;
                    seqSize = 1;
                }
            }
            return pair;
        }
    }
}
