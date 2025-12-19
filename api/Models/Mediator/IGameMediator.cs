using Api.Models.Facade;

namespace Api.Models.Mediator
{
    /// <summary>
    /// MEDIATOR PATTERN - Mediator Interface
    /// Defines communication protocol between game components
    /// </summary>
    public interface IGameMediator
    {
        void Notify(object sender, GameEvent gameEvent);
        void RegisterPlayer(Player player);
        void RegisterScoreboard(Scoreboard scoreboard);
        void RegisterAudioSubsystem(IAudioSubsystem audioSubsystem);
    }

    /// <summary>
    /// Game events that trigger mediation
    /// </summary>
    public class GameEvent
    {
        public string EventType { get; set; } = string.Empty;
        public Dictionary<string, object> Data { get; set; } = new();

        public GameEvent(string eventType)
        {
            EventType = eventType;
        }

        public void AddData(string key, object value)
        {
            Data[key] = value;
        }

        public T? GetData<T>(string key)
        {
            return Data.TryGetValue(key, out var value) && value is T typedValue ? typedValue : default;
        }
    }
}
