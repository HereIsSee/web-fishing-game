namespace Api.Models.Proxy
{
    /// <summary>
    /// PROXY PATTERN - Session Interface
    /// Defines operations that require access control
    /// </summary>
    public interface ISession
    {
        void StartGame(string requesterId);
        void EndGame(string requesterId);
        void ResetGame(string requesterId);
        void KickPlayer(string playerId, string requesterId);
        void ChangeGameSettings(string requesterId, int timerDuration);
        
        // Read operations (no auth needed)
        string GetHostId();
        bool IsActive { get; }
        int TimerDuration { get; }
    }
}
