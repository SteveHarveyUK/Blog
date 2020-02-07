using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Metrics
{
    class Program
    {
        // https://msdn.microsoft.com/fr-fr/library/windows/desktop/ms686016.aspx
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);

        // https://msdn.microsoft.com/fr-fr/library/windows/desktop/ms683242.aspx
        private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }
        private static bool ExitHandler(CtrlType signal)
        {
            switch (signal)
            {
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    Console.WriteLine("Closing");
                    // TODO Cleanup resources
                    Console.ForegroundColor = oldCol;
                    Environment.Exit(0);
                    return false;

                default:
                    return false;
            }
        }

        internal static ICustomMetricsService SourceA;
        internal static ICustomMetricsService SourceB;
        internal static ICustomMetricsService SourceC;
        private static ConsoleColor oldCol;

        static void Main()
        {
            oldCol = Console.ForegroundColor;
            SetConsoleCtrlHandler(ExitHandler, true);

            try
            {
                Console.WriteLine("**** Create EventSources ****");

                SourceA = MetricsFactory.GetCustomMetricsService("SourceA", 1.0, true);
                SourceB = MetricsFactory.GetCustomMetricsService("SourceB", 3.0, true);
                SourceC = MetricsFactory.GetCustomMetricsService("SourceC", 7.0, true);

                Console.WriteLine("**** Issue Description ****");
                Console.WriteLine(
                    "Each EventSource has is own Default Listener but it appears that ALL EventListeners get ALL EventCounter events.\r\n" +
                    "So rather than having a single highlight color per event to indicate the expected filtering,\r\n" +
                    "the following event logging will show multiple highlight colors within a single line:");
                Console.WriteLine("***************************");

                var random = new Random();
                for (int i = 0; i <= 40000; i++)
                {
                    SleepingBeauty(random.Next(10, 200));
                }

                Console.ReadKey();
            }
            finally
            {
                Console.ForegroundColor = oldCol;
            }
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