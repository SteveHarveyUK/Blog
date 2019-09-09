using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Text;

namespace Metrics
{
    /// <summary>
    /// Factory Pattern to access ICustomMetricsService
    /// </summary>
    public static class MetricsFactory
    {
        /// <summary>
        /// Static collection of EventSource by name.
        /// </summary>
        private static readonly Dictionary<string, CustomMetricsEventSource> MetricsEventSources = new Dictionary<string, CustomMetricsEventSource>(11);

        /// <summary>
        /// Create or Return the metrics service base on passed name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="updateRateSeconds"></param>
        /// <returns></returns>
        public static ICustomMetricsService GetCustomMetricsService(string name, double updateRateSeconds, bool enableFiltering)
        {
            if (!MetricsEventSources.TryGetValue(name, out var eventSource))
            {
                MetricsEventSources.Add(name, eventSource = new CustomMetricsEventSource(name));
                eventSource.DefaultListener = RegisterCustomMetricsEventListener(eventSource, updateRateSeconds, enableFiltering);

                Console.WriteLine($"Created EventSource '{name}' @ E{eventSource.GetHashCode():x8} with default EventListener @ L{eventSource.DefaultListener.GetHashCode():x8}");
            }

            return eventSource;
        }

        /// <summary>
        /// Hook up an EventListener for the passed ICustomMetricsService EventSource
        /// </summary>
        /// <param name="metricsService"></param>
        /// <param name="updateRateSeconds"></param>
        /// <returns></returns>
        private static EventListener RegisterCustomMetricsEventListener(ICustomMetricsService metricsService, double updateRateSeconds, bool enableFiltering)
        {
            var eventSource = metricsService as EventSource;
            var reader = new CustomMetricsEventListener(eventSource, enableFiltering);
            var arguments = new Dictionary<string, string>
            {
                {"EventCounterIntervalSec", updateRateSeconds.ToString(CultureInfo.InvariantCulture)}
            };
            reader.EnableEvents(eventSource, EventLevel.LogAlways, EventKeywords.None, arguments);

            return reader;
        }
    }
}
