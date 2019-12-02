using System;

namespace Advent.Of.Code
{
    abstract class AbstractAocTask
    {
        public virtual void First() => Console.WriteLine("(Not implemented)");
        public virtual void Second() => Console.WriteLine("(Not implemented)");

        protected void ValidateAnswer<T>(T result, T correct)
        {
            if (!result.Equals(correct))
                throw new ApplicationException($"Expected {correct}, got {result}");
        }

        protected void Echo(string msg) => Console.WriteLine(msg);
    }
}
