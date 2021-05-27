using System;
using System.Diagnostics;

namespace JuanMartin.Kernel.Utilities
{
    public class UtilityHelper
    {
        /// <summary>
        /// Get duration in milliseconds of executing 'action'.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static long Measure(Action action, bool withConsolePrint=false, string label = "Duration")
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();

            if(withConsolePrint)
            {
                Console.WriteLine(label + $" {sw.ElapsedMilliseconds}ms.");
                //Console.WriteLine("Press any key to exist");
                //Console.ReadKey();

            }
            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Get duration in milliseconds of executing 'action' 'n' number of times.
        /// </summary>
        /// <param name="loopCount">number of times to execute 'action'</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static long MeasureInLoop(int loopCount, int seed, Action<int> action, bool withConsolePrint = false, string label="Duration")
        {
            var sw = new Stopwatch();
            sw.Start();
            for(var i=0;i<loopCount;i++)
                action(seed + i);
            sw.Stop();

            if (withConsolePrint)
            {
                Console.WriteLine(label + $" {sw.ElapsedMilliseconds}ms.");
                //Console.WriteLine("Press any key to exist");
                //Console.ReadKey();

            }
            return sw.ElapsedMilliseconds;
        }
    }
}
