namespace Api.Models
{
    public interface IFishGroup
    {
        void Update(int environmentWidth, int waterLevelHeight);
        IEnumerable<Fish> Flatten();
        void TriggerScare();
        bool RemoveFish(int fishId);
    }
}
