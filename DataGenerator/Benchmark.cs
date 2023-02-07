using System;
using System.Diagnostics;
using System.Threading;

namespace DataGenerator
{
    public class Benchmark<T> where T : struct
    {
        public static void Check(Action<T> x, T param)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            x(param);

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine(x.Method.Name);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }
}