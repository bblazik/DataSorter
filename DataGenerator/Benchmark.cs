using System;
using System.Diagnostics;

namespace DataGenerator
{
    public class Benchmark
    {
        public static TimeSpan Check<T>(Action<T> action, T param)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            action(param);

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            var ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine(action.Method.Name);
            Console.WriteLine("RunTime " + elapsedTime);

            return ts;
        }
    }
}