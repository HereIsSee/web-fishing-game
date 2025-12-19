namespace Api.Models
{
    public interface IFishSpeedState
    {
        string Name { get; }
        void Update(Fish fish, int environmentWidth, int waterLevelHeight);
    }
}
