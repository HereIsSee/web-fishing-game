using System.Diagnostics;
using Api.Models.Facade;

namespace Api.Models.Proxy
{
    /// <summary>
    /// PROXY PATTERN - Virtual Proxy (Delayed Creation)
    /// Delays expensive initialization of AudioSubsystem until first actual use
    /// Tracks performance and memory usage for proxy pattern demonstration
    /// </summary>
    public class VirtualAudioSubsystemProxy : IAudioSubsystem
    {
        private IAudioSubsystem? _realAudioSubsystem;
        private readonly object _lock = new object();
        private bool _isInitialized = false;
        
        // Performance tracking
        private readonly Stopwatch _sessionTimer = Stopwatch.StartNew();
        private long _totalMemoryUsed = 0;
        private int _totalOperations = 0;
        private readonly Dictionary<string, (int calls, long totalMs)> _operationStats = new();

        public VirtualAudioSubsystemProxy()
        {
            Console.WriteLine("🎵 VirtualAudioSubsystemProxy created (real subsystem NOT initialized yet)");
        }

        private IAudioSubsystem GetRealSubsystem()
        {
            if (!_isInitialized)
            {
                lock (_lock)
                {
                    if (!_isInitialized)
                    {
                        Console.WriteLine("🎵 Initializing real AudioSubsystem for the first time (DELAYED CREATION)...");
                        
                        // Simulate expensive initialization
                        System.Threading.Thread.Sleep(100); // Simulate loading sound files
                        
                        _realAudioSubsystem = new AudioSubsystem();
                        _isInitialized = true;
                        
                        Console.WriteLine("✅ AudioSubsystem initialized successfully!");
                    }
                }
            }

            return _realAudioSubsystem!;
        }

        public void PlayCatchSound()
        {
            Console.WriteLine("🎵 Proxy: PlayCatchSound called - ensuring subsystem is initialized...");
            TrackOperation("PlayCatchSound", () => GetRealSubsystem().PlayCatchSound());
        }

        public void PlayMissSound()
        {
            Console.WriteLine("🎵 Proxy: PlayMissSound called - ensuring subsystem is initialized...");
            TrackOperation("PlayMissSound", () => GetRealSubsystem().PlayMissSound());
        }

        public void PlayAmbientSound()
        {
            Console.WriteLine("🎵 Proxy: PlayAmbientSound called - ensuring subsystem is initialized...");
            TrackOperation("PlayAmbientSound", () => GetRealSubsystem().PlayAmbientSound());
        }

        public string GetAudioReport()
        {
            if (!_isInitialized)
            {
                return "Audio subsystem not yet initialized (no sounds played yet)";
            }
            return GetRealSubsystem().GetAudioReport();
        }

        public bool IsInitialized() => _isInitialized;
        
        private void TrackOperation(string operationName, Action operation)
        {
            var sw = Stopwatch.StartNew();
            long memBefore = GC.GetTotalMemory(false);
            
            operation();
            
            sw.Stop();
            long memAfter = GC.GetTotalMemory(false);
            long memUsed = Math.Max(0, memAfter - memBefore);
            
            _totalOperations++;
            _totalMemoryUsed += memUsed;
            
            if (!_operationStats.ContainsKey(operationName))
                _operationStats[operationName] = (0, 0);
            
            var (calls, totalMs) = _operationStats[operationName];
            _operationStats[operationName] = (calls + 1, totalMs + sw.ElapsedMilliseconds);
        }
        
        public Dictionary<string, object> GetPerformanceReport()
        {
            var report = new Dictionary<string, object>
            {
                ["ProxyType"] = "VirtualAudioSubsystemProxy (Lazy Loading)",
                ["SessionDurationSeconds"] = _sessionTimer.Elapsed.TotalSeconds,
                ["TotalMemoryUsedMB"] = _totalMemoryUsed / (1024.0 * 1024.0),
                ["TotalOperations"] = _totalOperations,
                ["IsInitialized"] = _isInitialized,
                ["MemorySavedByLazyLoading"] = !_isInitialized ? "~2-5 MB (not initialized)" : "N/A"
            };
            
            var opStats = new Dictionary<string, object>();
            foreach (var kvp in _operationStats)
            {
                opStats[kvp.Key] = new Dictionary<string, object>
                {
                    ["TotalCalls"] = kvp.Value.calls,
                    ["TotalTimeMs"] = kvp.Value.totalMs,
                    ["AverageTimeMs"] = kvp.Value.calls > 0 ? kvp.Value.totalMs / (double)kvp.Value.calls : 0
                };
            }
            report["OperationStats"] = opStats;
            
            return report;
        }
    }
}
