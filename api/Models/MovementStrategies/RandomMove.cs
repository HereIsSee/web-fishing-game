namespace Api.Models
{
    public class RandomMove : IFishMove
    {
        private static readonly Random random = new Random();
        private double directionX;
        private double directionY;
        private int framesUntilDirectionChange;

        public override void Move(Fish fish, int environmentWidth, int waterLevelHeight)
        {
            if (framesUntilDirectionChange <= 0)
            {
                double angle = random.NextDouble() * Math.PI * 2;
                directionX = Math.Cos(angle);
                directionY = Math.Sin(angle);
                framesUntilDirectionChange = random.Next(20, 50);
            }

            fish.PositionX += directionX * fish.MovementSpeed;
            fish.PositionY += directionY * fish.MovementSpeed;

            framesUntilDirectionChange--;

            ClampPosition(fish, environmentWidth, waterLevelHeight);
        }
    }
}
