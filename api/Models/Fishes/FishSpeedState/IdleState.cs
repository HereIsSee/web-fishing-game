namespace Api.Models
{
    public class IdleState : IFishSpeedState
    {
        public string Name => "Idle";

        public void Update(Fish fish, int w, int h)
        {
            fish.ApplySpeedMultiplier(0.6);
            fish.MoveWithCurrentStrategy(w, h);
        }
    }
}
