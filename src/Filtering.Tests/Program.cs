using System.Reflection;
using NUnitLite;

namespace MathNet.Filtering.Tests
{
    class Program
    {
        public static int Main(string[] args)
        {
            return new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args);
        }
    }
}
