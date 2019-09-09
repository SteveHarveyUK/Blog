using System;
using System.Diagnostics.Tracing;

namespace Metrics
{
    public static class EventWrittenEventArgsExtensions
    {
        public static bool IsEventCounter(this EventWrittenEventArgs eventData)
        {
            return string.Equals(eventData.EventName, "EventCounters", StringComparison.Ordinal);
        }

        public static EventCounterData ToEventCounterData(this EventWrittenEventArgs eventData)
        {
            if (!eventData.IsEventCounter())
                return null;

            return new EventCounterData(eventData);
        }
    }
}