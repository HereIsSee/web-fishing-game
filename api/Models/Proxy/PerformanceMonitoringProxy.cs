using System.Diagnostics;

namespace Api.Models.Proxy
{
    /// <summary>
    /// PROXY PATTERN - Added Functionality Proxy
    /// Adds real-time performance monitoring and profiling to GameHub operations
    /// Tracks execution time, memory usage, and operation frequency
    /// </summary>
    public class PerformanceMonitoringProxy
    {
        private readonly Dictionary<string, List<double>> _executionTimes;
        private readonly Dictionary<string, long> _memoryAllocations;
        private readonly Dictionary<string, int> _callCounts;
        private readonly Stopwatch _sessionStopwatch;
        private long _totalMemoryStart;

        public PerformanceMonitoringProxy()
        {
            _executionTimes = new Dictionary<string, List<double>>();
            _memoryAllocations = new Dictionary<string, long>();
            _callCounts = new Dictionary<string, int>();
            _sessionStopwatch = Stopwatch.StartNew();
            _totalMemoryStart = GC.GetTotalMemory(false);
        }

        /// <summary>
        /// Execute an operation with performance monitoring
        /// </summary>
        public void ExecuteWithMonitoring(string operationName, Action operation)
        {
            var sw = Stopwatch.StartNew();
            long memBefore = GC.GetTotalMemory(false);

            try
            {
                operation();
            }
            finally
            {
                sw.Stop();
                long memAfter = GC.GetTotalMemory(false);
                long memAllocated = Math.Max(0, memAfter - memBefore);

                RecordMetrics(operationName, sw.Elapsed.TotalMilliseconds, memAllocated);
            }
        }

        /// <summary>
        /// Execute an operation with return value and performance monitoring
        /// </summary>
        public T ExecuteWithMonitoring<T>(string operationName, Func<T> operation)
        {
            var sw = Stopwatch.StartNew();
            long memBefore = GC.GetTotalMemory(false);

            try
            {
                return operation();
            }
            finally
            {
                sw.Stop();
                long memAfter = GC.GetTotalMemory(false);
                long memAllocated = Math.Max(0, memAfter - memBefore);

                RecordMetrics(operationName, sw.Elapsed.TotalMilliseconds, memAllocated);
            }
        }

        private void RecordMetrics(string operationName, double elapsedMs, long memoryAllocated)
        {
            // Record execution time
            if (!_executionTimes.ContainsKey(operationName))
                _executionTimes[operationName] = new List<double>();
            _executionTimes[operationName].Add(elapsedMs);

            // Record memory allocation
            if (!_memoryAllocations.ContainsKey(operationName))
                _memoryAllocations[operationName] = 0;
            _memoryAllocations[operationName] += memoryAllocated;

            // Record call count
            if (!_callCounts.ContainsKey(operationName))
                _callCounts[operationName] = 0;
            _callCounts[operationName]++;
        }

        /// <summary>
        /// Get comprehensive performance report
        /// </summary>
        public Dictionary<string, object> GetPerformanceReport()
        {
            var report = new Dictionary<string, object>();
            var operationStats = new Dictionary<string, Dictionary<string, object>>();

            foreach (var operation in _executionTimes.Keys)
            {
                var times = _executionTimes[operation];
                var stats = new Dictionary<string, object>
                {
                    ["TotalCalls"] = _callCounts[operation],
                    ["AverageTimeMs"] = times.Average(),
                    ["MinTimeMs"] = times.Min(),
                    ["MaxTimeMs"] = times.Max(),
                    ["TotalTimeMs"] = times.Sum(),
                    ["TotalMemoryBytes"] = _memoryAllocations[operation],
                    ["AvgMemoryPerCallBytes"] = _memoryAllocations[operation] / _callCounts[operation]
                };

                operationStats[operation] = stats;
            }

            var totalSessionTime = _sessionStopwatch.Elapsed.TotalSeconds;
            var totalMemoryUsed = GC.GetTotalMemory(false) - _totalMemoryStart;

            report["OperationStats"] = operationStats;
            report["SessionDurationSeconds"] = totalSessionTime;
            report["TotalMemoryUsedBytes"] = totalMemoryUsed;
            report["TotalMemoryUsedMB"] = totalMemoryUsed / (1024.0 * 1024.0);
            report["TotalOperations"] = _callCounts.Values.Sum();

            return report;
        }

        /// <summary>
        /// Get formatted performance summary for display
        /// </summary>
        public string GetFormattedSummary()
        {
            var report = GetPerformanceReport();
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("\n╔══════════════════════════════════════════════════════════════════════╗");
            sb.AppendLine("║           PROXY PATTERN - PERFORMANCE MONITORING RESULTS             ║");
            sb.AppendLine("╚══════════════════════════════════════════════════════════════════════╝");

            sb.AppendLine($"\n📊 Session Statistics:");
            sb.AppendLine($"   Duration: {report["SessionDurationSeconds"]:F2}s");
            sb.AppendLine($"   Total Memory: {report["TotalMemoryUsedMB"]:F2} MB ({report["TotalMemoryUsedBytes"]:N0} bytes)");
            sb.AppendLine($"   Total Operations: {report["TotalOperations"]}");

            var operationStats = (Dictionary<string, Dictionary<string, object>>)report["OperationStats"];
            
            if (operationStats.Count > 0)
            {
                sb.AppendLine("\n📈 Operation Performance:");
                sb.AppendLine("   " + new string('─', 65));

                foreach (var kvp in operationStats.OrderByDescending(x => (int)x.Value["TotalCalls"]))
                {
                    var operation = kvp.Key;
                    var stats = kvp.Value;

                    sb.AppendLine($"\n   {operation}:");
                    sb.AppendLine($"      Calls: {stats["TotalCalls"]}");
                    sb.AppendLine($"      Time:  Avg={stats["AverageTimeMs"]:F3}ms, Min={stats["MinTimeMs"]:F3}ms, Max={stats["MaxTimeMs"]:F3}ms");
                    sb.AppendLine($"      Total: {stats["TotalTimeMs"]:F2}ms");
                    sb.AppendLine($"      Memory: {stats["TotalMemoryBytes"]:N0} bytes (avg {stats["AvgMemoryPerCallBytes"]:N0} per call)");
                }
            }

            sb.AppendLine("\n" + new string('═', 72));

            return sb.ToString();
        }

        /// <summary>
        /// Reset all metrics (useful for new game sessions)
        /// </summary>
        public void Reset()
        {
            _executionTimes.Clear();
            _memoryAllocations.Clear();
            _callCounts.Clear();
            _sessionStopwatch.Restart();
            _totalMemoryStart = GC.GetTotalMemory(false);
        }
    }
}
