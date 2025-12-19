namespace Api.Models.Proxy
{
    /// <summary>
    /// Interface for Scoreboard - enables Proxy pattern
    /// </summary>
    public interface IScoreboard
    {
        Dictionary<string, int> PlayerScores { get; }
        string CurrentGameState { get; }
        int RemainingTime { get; }
        
        void UpdateScores(Session session);
        void ResetScore(string playerName, string requesterId);
        void AddPoints(string playerName, int points, string requesterId);
        void ResetAllScores(string requesterId);
    }
}
