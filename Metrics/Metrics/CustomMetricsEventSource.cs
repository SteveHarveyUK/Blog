using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Metrics
{
    /// <summary>
    /// Custom Metric Event Source
    /// Relies on ETW (Event Tracing for Windows) so uses a fundamental low level
    /// functionality within the Framework.
    /// </summary>
    internal sealed class CustomMetricsEventSource : EventSource, ICustomMetricsService
    {
        /// <summary>
        /// Dictionary to store all dynamically created counters.
        /// </summary>
        private readonly Dictionary<string,EventCounter> _dynamicCounters = new Dictionary<string, EventCounter>(11);
        private readonly EventCounter _methodDurationCounter;

        public CustomMetricsEventSource(string name) : base(name)
        {
            // There's currently a bug in the framework that means we need to create at
            // least one EventCounter within the EventSource constructor for dynamic
            // EventCounter creation to work.
            _methodDurationCounter = new EventCounter(nameof(_methodDurationCounter), this);
        }

        public EventListener DefaultListener { get; internal set; }

        public void ReportMethodDurationInMs(long milliseconds)
        {
            _methodDurationCounter.WriteMetric(milliseconds);
        }

        public void ReportMetric(string name, float value)
        {
            if (!_dynamicCounters.TryGetValue(name, out EventCounter counterInstance))
            {
                counterInstance = new EventCounter(name, this);
                _dynamicCounters.Add(name, counterInstance);
            }
            counterInstance.WriteMetric(value);
        }

        public void ReportOccurence(string name)
        {
            if (!_dynamicCounters.TryGetValue(name, out EventCounter counterInstance))
            {
                counterInstance = new EventCounter(name, this);
                _dynamicCounters.Add(name, counterInstance);
            }
            counterInstance.WriteMetric(float.NaN);
        }
    }
}