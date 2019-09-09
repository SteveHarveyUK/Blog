using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Metrics
{
    /// <summary>
    /// Class to extract the data from the EventCounter
    /// </summary>
    public class EventCounterData
    {
        public EventCounterData(EventWrittenEventArgs eventData)
        {
            var payload = (IDictionary<string, object>) eventData.Payload[0];

            EventHash = eventData.GetHashCode();
            EventSource = eventData.EventSource.Name;
            EventName = eventData.EventName;
            Name = payload["Name"].ToString();
            Mean = (float)payload["Mean"];
            StandardDeviation = (float)payload["StandardDeviation"];
            Count = (int)payload["Count"];
            IntervalSec = (float)payload["IntervalSec"];
            Min = (float)payload["Min"];
            Max = (float)payload["Max"];
        }

        public int EventHash { get; }

        public string EventName { get; }
        public string EventSource { get; }
        public string Name { get; }
        public float Mean { get; }
        public float StandardDeviation { get; }
        public int Count { get; }
        public float IntervalSec { get; }
        public float Min { get; }
        public float Max { get; }
    }
}