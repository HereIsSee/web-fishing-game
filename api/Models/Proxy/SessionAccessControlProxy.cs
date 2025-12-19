using System.Diagnostics;

namespace Api.Models.Proxy
{
    /// <summary>
    /// Ensures only the session host can perform privileged operations
    /// </summary>
    public class SessionAccessControlProxy : ISession
    {
        private readonly Session _realSession;
        private readonly string _hostId;
        private int _unauthorizedAttempts = 0;
        private readonly Dictionary<string, int> _violationsByPlayer = new();
        
        // Performance tracking
        private readonly Stopwatch _sessionTimer = Stopwatch.StartNew();
        private long _totalMemoryUsed = 0;
        private int _totalOperations = 0;
        private int _authorizedOperations = 0;
        private readonly Dictionary<string, (int calls, long totalMs)> _operationStats = new();

        public bool IsActive => _realSession.IsActive;
        public int TimerDuration => _realSession.TimerDuration;

        public SessionAccessControlProxy(Session realSession, string hostId)
        {
            _realSession = realSession ?? throw new ArgumentNullException(nameof(realSession));
            _hostId = hostId;
            Console.WriteLine($"🔐 SessionAccessControlProxy created - Host: {hostId}");
        }

        public string GetHostId() => _hostId;

        public void StartGame(string requesterId)
        {
            TrackOperation("StartGame", () =>
            {
                if (!IsAuthorized(requesterId, "StartGame"))
                {
                    throw new UnauthorizedAccessException(
                        $"❌ DENIED: Player '{requesterId}' is not authorized to start the game. Only host '{_hostId}' can start.");
                }

                _authorizedOperations++;
                Console.WriteLine($"✅ Access granted: {requesterId} starting game");
                _realSession.IsActive = true;
                _realSession.State = GameState.Playing;
                _realSession.StartTime = DateTime.UtcNow;
            });
        }

        public void EndGame(string requesterId)
        {
            if (!IsAuthorized(requesterId, "EndGame"))
            {
                throw new UnauthorizedAccessException(
                    $"❌ DENIED: Player '{requesterId}' is not authorized to end the game. Only host '{_hostId}' can end.");
            }

            Console.WriteLine($"✅ Access granted: {requesterId} ending game");
            _realSession.EndGame();
        }

        public void ResetGame(string requesterId)
        {
            TrackOperation("ResetGame", () =>
            {
                if (!IsAuthorized(requesterId, "ResetGame"))
                {
                    throw new UnauthorizedAccessException(
                        $"❌ DENIED: Player '{requesterId}' is not authorized to reset the game. Only host '{_hostId}' can reset.");
                }

                _authorizedOperations++;
                Console.WriteLine($"✅ Access granted: {requesterId} resetting game");
                // Game reset logic handled by GameHub
            });
        }

        public void KickPlayer(string playerId, string requesterId)
        {
            if (!IsAuthorized(requesterId, "KickPlayer"))
            {
                throw new UnauthorizedAccessException(
                    $"❌ DENIED: Player '{requesterId}' is not authorized to kick players. Only host '{_hostId}' can kick.");
            }

            Console.WriteLine($"✅ Access granted: {requesterId} kicking player {playerId}");
            
            if (_realSession.Players.TryRemove(playerId, out var player))
            {
                Console.WriteLine($"👢 Player {player.Name} has been kicked from the session");
            }
        }

        public void ChangeGameSettings(string requesterId, int timerDuration)
        {
            if (!IsAuthorized(requesterId, "ChangeGameSettings"))
            {
                throw new UnauthorizedAccessException(
                    $"❌ DENIED: Player '{requesterId}' is not authorized to change game settings. Only host '{_hostId}' can modify settings.");
            }

            Console.WriteLine($"✅ Access granted: {requesterId} changing timer to {timerDuration}s");
            _realSession.TimerDuration = timerDuration;
        }

        private bool IsAuthorized(string requesterId, string action)
        {
            bool authorized = requesterId == _hostId;

            if (!authorized)
            {
                _unauthorizedAttempts++;
                
                if (!_violationsByPlayer.ContainsKey(requesterId))
                    _violationsByPlayer[requesterId] = 0;
                
                _violationsByPlayer[requesterId]++;

                Console.WriteLine($"🚨 SECURITY VIOLATION: Player '{requesterId}' attempted '{action}' (Attempt #{_violationsByPlayer[requesterId]})");
                Console.WriteLine($"   Total violations across all players: {_unauthorizedAttempts}");

                // Auto-kick after 3 violations
                {
                    Console.WriteLine($"⚠️ Player '{requesterId}' exceeded violation limit - should be kicked!");
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Get security statistics
        /// </summary>
        public Dictionary<string, object> GetSecurityStats()
        {
            return new Dictionary<string, object>
            {
                ["TotalUnauthorizedAttempts"] = _unauthorizedAttempts,
                ["UniqueViolators"] = _violationsByPlayer.Count,
                ["ViolationsByPlayer"] = new Dictionary<string, int>(_violationsByPlayer),
                ["HostId"] = _hostId
            };
        }
        
        private void TrackOperation(string operationName, Action operation)
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
                long memUsed = Math.Max(0, memAfter - memBefore);
                
                _totalOperations++;
                _totalMemoryUsed += memUsed;
                
                if (!_operationStats.ContainsKey(operationName))
                    _operationStats[operationName] = (0, 0);
                
                var (calls, totalMs) = _operationStats[operationName];
                _operationStats[operationName] = (calls + 1, totalMs + sw.ElapsedMilliseconds);
            }
        }
        
        public Dictionary<string, object> GetPerformanceReport()
        {
            var report = new Dictionary<string, object>
            {
                ["ProxyType"] = "SessionAccessControlProxy (Security)",
                ["SessionDurationSeconds"] = _sessionTimer.Elapsed.TotalSeconds,
                ["TotalMemoryUsedMB"] = _totalMemoryUsed / (1024.0 * 1024.0),
                ["TotalOperations"] = _totalOperations,
                ["AuthorizedOperations"] = _authorizedOperations,
                ["UnauthorizedAttempts"] = _unauthorizedAttempts,
                ["BlockedPercentage"] = _totalOperations > 0 ? (_unauthorizedAttempts / (double)_totalOperations * 100).ToString("F1") + "%" : "0%"
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
