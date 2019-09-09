using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading;

namespace Metrics
{
    class Program
    {
        internal static ICustomMetricsService SourceA = MetricsFactory.GetCustomMetricsService("SourceA", 1.0, false);
        internal static ICustomMetricsService SourceB = MetricsFactory.GetCustomMetricsService("SourceB", 1.0, false);

        static void Main(string[] args)
        {
            var random = new Random();
            for (int i = 0; i <= 40; i++)
            {
                SleepingBeauty(random.Next(10, 200));
            }

            Console.ReadKey();
        }

        static void SleepingBeauty(int sleepTimeInMs)
        {
            Debug.WriteLine($"SleepingBeauty({sleepTimeInMs})...");
            var stopwatch = Stopwatch.StartNew();

            Thread.Sleep(sleepTimeInMs);

            stopwatch.Stop();

            SourceA.ReportMethodDurationInMs(stopwatch.ElapsedMilliseconds);
            SourceA.ReportMetric("someCounter", DateTime.Now.Millisecond);

            // SourceB.ReportMethodDurationInMs(stopwatch.ElapsedMilliseconds);
            SourceB.ReportMetric("someOtherCounter", DateTime.Now.Millisecond);

            Debug.WriteLine($"SleepingBeauty({sleepTimeInMs}) complete.");
        }
    }
}