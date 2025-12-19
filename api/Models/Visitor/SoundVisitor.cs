namespace Api.Models.Visitor
{
    using Api.Models.Fishes;

    /// <summary>
    /// VISITOR PATTERN - Concrete Visitor 2
    /// Determines which sound effect to play for each fish type
    /// </summary>
    public class SoundVisitor : IFishVisitor
    {
        public string SoundEffect { get; private set; } = string.Empty;
        public int Volume { get; private set; }
        public string SoundCategory { get; private set; } = string.Empty;

        public void Visit(BlackFish fish)
        {
            SoundEffect = "rare_catch_chime.wav";
            Volume = 80;
            SoundCategory = "Rare";
            
            Console.WriteLine($"🔊 SoundVisitor: BlackFish → Play '{SoundEffect}' at volume {Volume}% (Category: {SoundCategory})");
        }

        public void Visit(BlueFish fish)
        {
            SoundEffect = "splash_small.wav";
            Volume = 50;
            SoundCategory = "Common";
            
            Console.WriteLine($"🔊 SoundVisitor: BlueFish → Play '{SoundEffect}' at volume {Volume}% (Category: {SoundCategory})");
        }

        public void Visit(YellowFish fish)
        {
            SoundEffect = "splash_medium.wav";
            Volume = 60;
            SoundCategory = "Uncommon";
            
            Console.WriteLine($"🔊 SoundVisitor: YellowFish → Play '{SoundEffect}' at volume {Volume}% (Category: {SoundCategory})");
        }

        public void Visit(BombFish fish)
        {
            SoundEffect = "explosion_boom.wav";
            Volume = 100;
            SoundCategory = "Danger";
            
            Console.WriteLine($"🔊 SoundVisitor: BombFish → Play '{SoundEffect}' at volume {Volume}% (Category: {SoundCategory}) ⚠️");
        }

        public void Visit(FatFish fish)
        {
            SoundEffect = "splash_large_heavy.wav";
            Volume = 85;
            SoundCategory = "Epic";
            
            Console.WriteLine($"🔊 SoundVisitor: FatFish → Play '{SoundEffect}' at volume {Volume}% (Category: {SoundCategory})");
        }

        public string GetFullSoundDescription()
        {
            return $"Sound: {SoundEffect} | Volume: {Volume}% | Category: {SoundCategory}";
        }
    }
}
