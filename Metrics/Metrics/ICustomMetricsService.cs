namespace Metrics
{
    /// <summary>
    /// Metrics Service Interface
    /// </summary>
    public interface ICustomMetricsService
    {
        /// <summary>
        /// Report a meaningful metric with value.
        /// The system will calculate Min/Max/Count/Mean/StandardDeviation/IntervalSec for us.
        /// </summary>
        /// <param name="name">Name of the metric to report.</param>
        /// <param name="value">Value to be reported.</param>
        void ReportMetric(string name, float value);
        /// <summary>
        /// Report an instance of an occurence to track.
        /// The system will keep track of Count/IntervalSec for us.
        /// </summary>
        /// <param name="name"></param>
        void ReportOccurence(string name);

        void ReportMethodDurationInMs(long milliseconds);
    }
}