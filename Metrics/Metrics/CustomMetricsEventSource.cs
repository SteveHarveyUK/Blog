﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;

namespace Metrics
{
    [EventSource(Name = "My-CustomMetricsEventSource-Minimal")]
    public sealed class CustomMetricsEventSource : EventSource
    {
        private EventCounter methodCalledCounter;
        private EventCounter methodDurationCounter;

        private Dictionary<string, EventCounter> dynamicCounters =
            new Dictionary<string, EventCounter>();

        public static CustomMetricsEventSource Log = new CustomMetricsEventSource();

        public CustomMetricsEventSource()
        {
            methodCalledCounter = new EventCounter(nameof(methodCalledCounter), this);
            methodDurationCounter = new EventCounter(nameof(methodDurationCounter), this);
        }

        public void ReportMethodCalled()
        {
            methodCalledCounter.WriteMetric(1);
        }

        public void ReportMethodDurationInMs(long milliseconds)
        {
            methodDurationCounter.WriteMetric(milliseconds);
        }

        public void ReportMetric(string name, float value)
        {
            if (!dynamicCounters.TryGetValue(name, out EventCounter counterInstance))
            {
                counterInstance = new EventCounter(name, this);
                dynamicCounters.Add(name, counterInstance);
            }
            counterInstance.WriteMetric(value);
        }
    }
}