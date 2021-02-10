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
        public static long Measure(Action action, bool with_print=false, string label = "Duration")
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();

            if(with_print)
            {
                Console.WriteLine(label + $" {sw.ElapsedMilliseconds}ms.");
                Console.WriteLine("Press any key to exist");
                Console.ReadKey();

            }
            return sw.ElapsedMilliseconds;
        }

        /// <summary>
        /// Get duration in milliseconds of executing 'action' 'n' number of times.
        /// </summary>
        /// <param name="n">number of times to execute 'action'</param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static long MeasureInLoop(int n, Action action, bool with_print = false, string label="Duration")
        {
            var sw = new Stopwatch();
            sw.Start();
            for(var i=0;i<n;i++)
                action();
            sw.Stop();

            if (with_print)
            {
                Console.WriteLine(label + $" {sw.ElapsedMilliseconds}ms.");
                Console.WriteLine("Press any key to exist");
                Console.ReadKey();

            }
            return sw.ElapsedMilliseconds;
        }
    }
}
