using System;
using System.Diagnostics;

namespace DataGenerator
{
    public class Benchmark
    {
        public static X3 Check<X1, X3>(Func<X1, X3> func, X1 param)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var output = func(param);
            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Display(func.Method.Name, ts);

            return output;
        }

        public static void Check<X1>(Action<X1> func, X1 param)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            func(param);
            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            Display(func.Method.Name, ts);
        }

        private static void Display(string methodName, TimeSpan ts)
        {
            // Format and display the TimeSpan value. 
            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine(methodName);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }
}

