namespace Api.Models
{
    public class LeftMove : IFishMove
    {
        public override void Move(Fish fish, int environmentWidth, int waterLevelHeight)
        {
            fish.PositionX -= fish.MovementSpeed;
            ClampPosition(fish, environmentWidth, waterLevelHeight);
        }
    }
}
