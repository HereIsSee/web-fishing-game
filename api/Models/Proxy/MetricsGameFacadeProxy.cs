using System.Diagnostics;
using Api.Models.Facade;
using Api.Models.Decorator;

namespace Api.Models.Proxy
{
    /// <summary>
    /// Wraps GameFacade to add performance tracking
    /// </summary>
    public class MetricsGameFacadeProxy : IGameFacade
    {
        private readonly GameFacade _realFacade;
        private readonly PerformanceMonitoringProxy _performanceMonitor;

        public MetricsGameFacadeProxy(GameFacade realFacade)
        {
            _realFacade = realFacade ?? throw new ArgumentNullException(nameof(realFacade));
            _performanceMonitor = new PerformanceMonitoringProxy();
        }

        public void InitializeGame()
        {
            _performanceMonitor.ExecuteWithMonitoring("GameFacade.InitializeGame", 
                () => _realFacade.InitializeGame());
        }

        public bool AttemptFishCatch(Player player, Fish fish)
        {
            return _performanceMonitor.ExecuteWithMonitoring("GameFacade.AttemptFishCatch", 
                () => _realFacade.AttemptFishCatch(player, fish));
        }

        public void ApplyEffect(Player player, IFishDecorator decorator)
        {
            _performanceMonitor.ExecuteWithMonitoring("GameFacade.ApplyEffect", 
                () => _realFacade.ApplyEffect(player, decorator));
        }

        public void RenderFrame(Player player)
        {
            _performanceMonitor.ExecuteWithMonitoring("GameFacade.RenderFrame", 
                () => _realFacade.RenderFrame(player));
        }

        public void PlaySuccessSound()
        {
            _performanceMonitor.ExecuteWithMonitoring("GameFacade.PlaySuccessSound", 
                () => _realFacade.PlaySuccessSound());
        }

        public void UpdateGameState()
        {
            _performanceMonitor.ExecuteWithMonitoring("GameFacade.UpdateGameState", 
                () => _realFacade.UpdateGameState());
        }

        public void RenderAllPlayers(List<Player> players)
        {
            _performanceMonitor.ExecuteWithMonitoring("GameFacade.RenderAllPlayers", 
                () => _realFacade.RenderAllPlayers(players));
        }

        public void UpdateAllPlayerScores(List<Player> players)
        {
            _performanceMonitor.ExecuteWithMonitoring("GameFacade.UpdateAllPlayerScores", 
                () => _realFacade.UpdateAllPlayerScores(players));
        }

        public void PlayGameOverSound()
        {
            _performanceMonitor.ExecuteWithMonitoring("GameFacade.PlayGameOverSound", 
                () => _realFacade.PlayGameOverSound());
        }

        /// <summary>
        /// Get comprehensive performance report
        /// </summary>
        public Dictionary<string, object> GetPerformanceReport()
        {
            return _performanceMonitor.GetPerformanceReport();
        }

        /// <summary>
        /// Get formatted performance summary
        /// </summary>
        public string GetFormattedSummary()
        {
            return _performanceMonitor.GetFormattedSummary();
        }

        /// <summary>
        /// Reset all metrics
        /// </summary>
        public void ResetMetrics()
        {
            _performanceMonitor.Reset();
        }
    }
}
