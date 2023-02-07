using System;
using System.Diagnostics;
using System.Threading;

namespace DataGenerator
{
    public class Benchmark<T, T1, ReturnType>
        where T : class
        where T1 : struct
        where ReturnType : class
    {
        public static ReturnType Check(Func<T, T1, ReturnType> x, T param, T1 param1)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            ReturnType output = x(param, param1);

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine(x.Method.Name);
            Console.WriteLine("RunTime " + elapsedTime);

            return output;
        }

        public static void Check(Action<T, T1> x, T param, T1 param1)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            x(param, param1);

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

        public static void Check(Action<T, string> x, T param, string param1)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            x(param, param1);

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

