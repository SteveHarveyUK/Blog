using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using Metrics;

namespace Metrics
{
    /// <summary>
    /// Custom Metrics Event Listener Implementation
    ///
    /// While EventSource messages are automatically filtered by the EventCounters
    /// don't seem to be so we are having to filter based on the passed in EventSource.
    /// </summary>
    internal class CustomMetricsEventListener : EventListener
    {
        private readonly EventSource _filterSource;
        private readonly bool _enableFiltering;

        internal CustomMetricsEventListener(EventSource filter, bool enabledFiltering) : base()
        {
            _filterSource = filter;
            _enableFiltering = enabledFiltering;
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (_enableFiltering)
            {
                if (!ReferenceEquals(eventData.EventSource, _filterSource))
                {
                    return;
                }
            }

            var counterData = eventData.ToEventCounterData();

            if (counterData == null)
            {
                Console.WriteLine($"L{this.GetHashCode():x8}:" +
                                  $"E{eventData.GetHashCode():x8}:" +
                                  $"{eventData.EventName} " +
                                  $"{eventData.Payload.FirstOrDefault().ToString()} " + 
                                  $" {(ReferenceEquals(eventData.EventSource, _filterSource)?"":"(WRONG EventSource)")}"
                                  );
                return;
            }

            // Only write to console if actual data has been reported
            if (counterData?.Count == 0)
                return;

            // ReportOccurence calls pass in NaN for the metric value so we can publish different
            // patterns for ReportOccurence and ReportMetric
            if (float.IsNaN(counterData.Min))
            {
                Console.WriteLine(
                    $"L{this.GetHashCode():x8}:" +
                    $"E{counterData.EventHash:x8}:" +
                    $"'{counterData.EventSource}/{counterData.EventName}' => " +
                    $"{counterData.Name} " +
                    $"Count {counterData.Count}, " +
                    $"IntervalSec: {counterData.IntervalSec}"+ 
                    $" {(ReferenceEquals(eventData.EventSource, _filterSource)?"":"(WRONG EventSource)")}");
            }
            else
            {
                Console.WriteLine(
                    $"L{this.GetHashCode():x8}:" +
                    $"E{counterData.EventHash:x8}:" +
                    $"'{counterData.EventSource}/{counterData.EventName}' => " +
                    $"{counterData.Name} " +
                    $"Min: {counterData.Min}, " +
                    $"Max: {counterData.Max}, " +
                    $"Count {counterData.Count}, " +
                    $"Mean {counterData.Mean}, " +
                    $"StandardDeviation: {counterData.StandardDeviation}, " +
                    $"IntervalSec: {counterData.IntervalSec}"+ 
                    $" {(ReferenceEquals(eventData.EventSource, _filterSource)?"":"(WRONG EventSource)")}");
            }
        }
    }
}