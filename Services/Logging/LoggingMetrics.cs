namespace StudentManagementAPI.Services.Infrastructure.Logging
{
    public class LoggingMetrics
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public long MemoryUsedMB { get; set; }
        public int ActiveRequests { get; set; }
        public double CpuUsagePercent { get; set; }
        public long TotalRequests { get; set; }
        public long ErrorCount { get; set; }
    }

    public interface IMetricsService
    {
        LoggingMetrics GetCurrentMetrics();
        void IncrementRequestCount();
        void IncrementErrorCount();
    }

    public class MetricsService : IMetricsService
    {
        private long _totalRequests = 0;
        private long _errorCount = 0;

        public LoggingMetrics GetCurrentMetrics()
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();

            return new LoggingMetrics
            {
                MemoryUsedMB = process.WorkingSet64 / 1024 / 1024,
                TotalRequests = Interlocked.Read(ref _totalRequests),
                ErrorCount = Interlocked.Read(ref _errorCount)
            };
        }

        public void IncrementRequestCount()
        {
            Interlocked.Increment(ref _totalRequests);
        }

        public void IncrementErrorCount()
        {
            Interlocked.Increment(ref _errorCount);
        }
    }
}