namespace Api.Models.Proxy
{
    /// <summary>
    /// PROXY PATTERN - Security Proxy
    /// Adds authorization checks before delegating to real Scoreboard
    /// </summary>
    public class SecureScoreboardProxy : IScoreboard
    {
        private readonly Scoreboard _realScoreboard;
        private readonly HashSet<string> _adminUsers;
        private readonly string _sessionHostId;

        public Dictionary<string, int> PlayerScores => _realScoreboard.PlayerScores;
        public string CurrentGameState => _realScoreboard.CurrentGameState;
        public int RemainingTime => _realScoreboard.RemainingTime;

        public SecureScoreboardProxy(Scoreboard realScoreboard, string sessionHostId)
        {
            _realScoreboard = realScoreboard ?? throw new ArgumentNullException(nameof(realScoreboard));
            _sessionHostId = sessionHostId;
            _adminUsers = new HashSet<string> { sessionHostId }; // Host is always admin
        }

        public void AddAdmin(string userId)
        {
            _adminUsers.Add(userId);
        }

        public void UpdateScores(Session session)
        {
            // Read operation - allowed for everyone
            _realScoreboard.Update(session);
        }

        public void ResetScore(string playerName, string requesterId)
        {
            if (!IsAuthorized(requesterId))
            {
                throw new UnauthorizedAccessException(
                    $"User {requesterId} is not authorized to reset scores. Only admins or session host can perform this action.");
            }

            Console.WriteLine($"✅ Security check passed for {requesterId} - resetting score for {playerName}");
            
            if (_realScoreboard.PlayerScores.ContainsKey(playerName))
            {
                _realScoreboard.PlayerScores[playerName] = 0;
            }
        }

        public void AddPoints(string playerName, int points, string requesterId)
        {
            if (!IsAuthorized(requesterId))
            {
                throw new UnauthorizedAccessException(
                    $"User {requesterId} is not authorized to modify scores.");
            }

            Console.WriteLine($"✅ Security check passed for {requesterId} - adding {points} points to {playerName}");
            
            if (_realScoreboard.PlayerScores.ContainsKey(playerName))
            {
                _realScoreboard.PlayerScores[playerName] += points;
            }
            else
            {
                _realScoreboard.PlayerScores[playerName] = points;
            }
        }

        public void ResetAllScores(string requesterId)
        {
            if (!IsAuthorized(requesterId))
            {
                throw new UnauthorizedAccessException(
                    $"User {requesterId} is not authorized to reset all scores.");
            }

            Console.WriteLine($"✅ Security check passed for {requesterId} - resetting all scores");
            _realScoreboard.PlayerScores.Clear();
        }

        private bool IsAuthorized(string userId)
        {
            return _adminUsers.Contains(userId) || userId == _sessionHostId;
        }
    }
}
