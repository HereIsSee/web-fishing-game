namespace Api.Models.Visitor
{
    using Api.Models.Fishes;

    /// <summary>
    /// VISITOR PATTERN - Concrete Visitor 1
    /// Calculates score values for different fish types
    /// </summary>
    public class ScoreVisitor : IFishVisitor
    {
        public int CalculatedScore { get; private set; }
        public string ScoreBreakdown { get; private set; } = string.Empty;

        public void Visit(BlackFish fish)
        {
            // Black fish: base points + bonus for speed
            int basePoints = fish.Points;
            int speedBonus = (int)(fish.MovementSpeed * 2);
            CalculatedScore = basePoints + speedBonus;
            
            ScoreBreakdown = $"BlackFish: Base({basePoints}) + SpeedBonus({speedBonus}) = {CalculatedScore}";
            Console.WriteLine($"🎯 ScoreVisitor: {ScoreBreakdown}");
        }

        public void Visit(BlueFish fish)
        {
            // Blue fish: standard points
            CalculatedScore = fish.Points;
            
            ScoreBreakdown = $"BlueFish: StandardPoints = {CalculatedScore}";
            Console.WriteLine($"🎯 ScoreVisitor: {ScoreBreakdown}");
        }

        public void Visit(YellowFish fish)
        {
            // Yellow fish: base points + radius bonus (larger = more points)
            int basePoints = fish.Points;
            int sizeBonus = (int)(fish.Radius * 0.5);
            CalculatedScore = basePoints + sizeBonus;
            
            ScoreBreakdown = $"YellowFish: Base({basePoints}) + SizeBonus({sizeBonus}) = {CalculatedScore}";
            Console.WriteLine($"🎯 ScoreVisitor: {ScoreBreakdown}");
        }

        public void Visit(BombFish fish)
        {
            // Bomb fish: negative points (penalty)
            CalculatedScore = -50;
            
            ScoreBreakdown = $"BombFish: PENALTY = {CalculatedScore} (explodes!)";
            Console.WriteLine($"🎯 ScoreVisitor: {ScoreBreakdown}");
        }

        public void Visit(FatFish fish)
        {
            // Fat fish: high base points + weight bonus
            int basePoints = fish.Points;
            int weightBonus = 25;
            CalculatedScore = basePoints + weightBonus;
            
            ScoreBreakdown = $"FatFish: Base({basePoints}) + WeightBonus({weightBonus}) = {CalculatedScore}";
            Console.WriteLine($"🎯 ScoreVisitor: {ScoreBreakdown}");
        }
    }
}
