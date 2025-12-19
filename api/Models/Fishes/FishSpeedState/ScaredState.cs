namespace Api.Models
{
    public class ScaredState : IFishSpeedState
    {
        public string Name => "Scared";
        private int _ticksLeft;

        public ScaredState(int ticksLeft = 120)
        {
            _ticksLeft = ticksLeft;
        }

        public void Update(Fish fish, int w, int h)
        {
            fish.ApplySpeedMultiplier(2);
            fish.MoveWithCurrentStrategy(w, h);

            _ticksLeft--;
            if (_ticksLeft <= 0)
            {
                fish.SetState(new WanderingState(240));
            }
        }
    }
}
