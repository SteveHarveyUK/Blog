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
        private readonly object _outputLockObj = new object();

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            lock (_outputLockObj)
            {
                var counterData = eventData.ToEventCounterData();

                var oldCol = Console.ForegroundColor;
                if (counterData == null)
                {
                    Console.ForegroundColor = MetricsFactory.GetConsoleColor(this.GetHashCode());
                    Console.Write($"EL{this.GetHashCode():x8}");
                    Console.ForegroundColor = oldCol;
                    Console.Write($" RX ED{eventData.GetHashCode():x8} => ");
                    Console.ForegroundColor = MetricsFactory.GetConsoleColor(eventData.EventSource.GetHashCode());
                    Console.Write($"ES{eventData.EventSource.GetHashCode():x8}:{eventData.EventName}");
                    Console.ForegroundColor = oldCol;
                    Console.WriteLine($"{eventData.Payload.FirstOrDefault().ToString()} ");

                    return;
                }

                // Only write to console if actual data has been reported
                if (counterData?.Count == 0)
                    return;

                // ReportOccurence calls pass in NaN for the metric value so we can publish different
                // patterns for ReportOccurence and ReportMetric
                if (float.IsNaN(counterData.Min))
                {
                    Console.ForegroundColor = MetricsFactory.GetConsoleColor(this.GetHashCode());
                    Console.Write($"EL{this.GetHashCode():x8}");
                    Console.ForegroundColor = oldCol;
                    Console.Write($" RX ED{counterData.EventHash:x8} => ");
                    Console.ForegroundColor = MetricsFactory.GetConsoleColor(eventData.EventSource.GetHashCode());
                    Console.Write(
                        $"ES{eventData.EventSource.GetHashCode():x8}:'{counterData.EventSource}/{counterData.EventName}'");
                    Console.ForegroundColor = oldCol;
                    Console.WriteLine($" => " +
                                      $"{counterData.Name} " +
                                      $"Count {counterData.Count}, " +
                                      $"IntervalSec: {counterData.IntervalSec}");
                }
                else
                {
                    Console.ForegroundColor = MetricsFactory.GetConsoleColor(this.GetHashCode());
                    Console.Write($"EL{this.GetHashCode():x8}");
                    Console.ForegroundColor = oldCol;
                    Console.Write($" RX ED{counterData.EventHash:x8} => ");
                    Console.Write(":");
                    Console.ForegroundColor = MetricsFactory.GetConsoleColor(eventData.EventSource.GetHashCode());
                    Console.Write(
                        $"ES{eventData.EventSource.GetHashCode():x8}:'{counterData.EventSource}/{counterData.EventName}'");
                    Console.ForegroundColor = oldCol;
                    Console.WriteLine($" => " +
                                      $"{counterData.Name} " +
                                      $"Min: {counterData.Min}, " +
                                      $"Max: {counterData.Max}, " +
                                      $"Count {counterData.Count}, " +
                                      $"Mean {counterData.Mean}, " +
                                      $"StandardDeviation: {counterData.StandardDeviation}, " +
                                      $"IntervalSec: {counterData.IntervalSec}");
                }
            }
        }
    }
}