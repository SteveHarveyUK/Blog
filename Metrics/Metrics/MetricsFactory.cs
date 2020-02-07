using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Globalization;

namespace Metrics
{
    /// <summary>
    /// Factory Pattern to access ICustomMetricsService
    /// </summary>
    public static class MetricsFactory
    {
        private static readonly ConcurrentDictionary<int, ConsoleColor> HighlightColors =  new ConcurrentDictionary<int, ConsoleColor>();
        private static readonly List<ConsoleColor> Colors = new List<ConsoleColor> { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Blue, ConsoleColor.Magenta };

        public static ConsoleColor GetConsoleColor(int hashCode)
        {
            if (HighlightColors.TryGetValue(hashCode, out var consoleColor))
            {
                return consoleColor;
            }

            return Console.ForegroundColor;
        }

        /// <summary>
        /// Static collection of EventSource by name.
        /// </summary>
        private static readonly Dictionary<string, CustomMetricsEventSource> MetricsEventSources = new Dictionary<string, CustomMetricsEventSource>(11);

        private static readonly object OutputLockObj = new object();

        /// <summary>
        /// Create or Return the metrics service base on passed name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="updateRateSeconds"></param>
        /// <param name="collectMetrics"></param>
        /// <returns></returns>
        public static ICustomMetricsService GetCustomMetricsService(string name, double updateRateSeconds, bool collectMetrics)
        {
            if (!MetricsEventSources.TryGetValue(name, out var eventSource))
            {
                MetricsEventSources.Add(name, eventSource = new CustomMetricsEventSource(name));
                eventSource.DefaultListener = RegisterCustomMetricsEventListener(eventSource, updateRateSeconds, collectMetrics);

                var index = (HighlightColors.Count==0) ? 0 : (HighlightColors.Count / 2)%Colors.Count;
                var consoleColor = Colors[index];
                HighlightColors.AddOrUpdate(eventSource.GetHashCode(), consoleColor, (i,c) => HighlightColors[i]=c);
                HighlightColors.AddOrUpdate(eventSource.DefaultListener.GetHashCode(), consoleColor, (i, c) => HighlightColors[i] = c);

                lock (OutputLockObj)
                {
                    Console.Write("Created EventSource ");
                    var oldCol = Console.ForegroundColor;
                    Console.ForegroundColor = consoleColor;
                    Console.Write($"'{name}' @ ES{eventSource.GetHashCode():x8}");
                    Console.ForegroundColor = oldCol;
                    Console.Write(" with default EventListener @ ");
                    Console.ForegroundColor = consoleColor;
                    Console.WriteLine($"EL{eventSource.DefaultListener.GetHashCode():x8}");
                    Console.ForegroundColor = oldCol;
                }
            }

            return eventSource;
        }

        /// <summary>
        /// Hook up an EventListener for the passed ICustomMetricsService EventSource
        /// </summary>
        /// <param name="metricsService"></param>
        /// <param name="updateRateSeconds"></param>
        /// <param name="collectMetrics"></param>
        /// <returns></returns>
        private static EventListener RegisterCustomMetricsEventListener(ICustomMetricsService metricsService, double updateRateSeconds, bool collectMetrics)
        {
            var eventSource = metricsService as EventSource;
            var reader = new CustomMetricsEventListener();
            var arguments = new Dictionary<string, string>
            {
                {"EventCounterIntervalSec", updateRateSeconds.ToString(CultureInfo.InvariantCulture)}
            };
            reader.EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.None, collectMetrics ? arguments : null);

            return reader;
        }
    }
}
