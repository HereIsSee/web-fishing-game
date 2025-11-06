namespace Api.Models
{
    public abstract class IFishMove
    {
        public abstract void Move(Fish fish, int environmentWidth, int waterLevelHeight);

        protected void ClampPosition(Fish fish, int environmentWidth, int waterLevelHeight)
        {
            if (fish.PositionX < 0) fish.PositionX = 0;
            if (fish.PositionX > environmentWidth) fish.PositionX = environmentWidth;

            if (fish.PositionY < 0) fish.PositionY = 0;
            if (fish.PositionY > waterLevelHeight) fish.PositionY = waterLevelHeight;
        }
    }
}
