namespace Api.Models
{
    public class UpMove : IFishMove
    {
        public override void Move(Fish fish, int environmentWidth, int waterLevelHeight)
        {
            fish.PositionY += fish.MovementSpeed;
            ClampPosition(fish, environmentWidth, waterLevelHeight);
        }
    }
}
