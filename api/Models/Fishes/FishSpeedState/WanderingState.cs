namespace Api.Models
{
    public class WanderingState : IFishSpeedState
    {
        public string Name => "Wandering";
        private int _ticksLeft;

        public WanderingState(int ticksLeft = 240)
        {
            _ticksLeft = ticksLeft;
        }

        public void Update(Fish fish, int w, int h)
        {
            fish.ApplySpeedMultiplier(0.9);
            fish.MoveWithCurrentStrategy(w, h);

            _ticksLeft--;
            if (_ticksLeft <= 0)
            {
                fish.SetState(new IdleState());
            }
        }
    }
}
