namespace Api.Models.ChainOfResponsibility
{
    /// <summary>
    /// Constructs and manages the catch validation chain
    /// </summary>
    public class CatchValidationChain
    {
        private readonly CatchAttemptHandler _chainHead;
        private readonly CooldownCheckHandler _cooldownHandler;
        private readonly EnergyCheckHandler _energyHandler;

        public CatchValidationChain()
        {
            // Build the chain: Cooldown → Energy → Range → Obstacle → Resolution
            _cooldownHandler = new CooldownCheckHandler();
            _energyHandler = new EnergyCheckHandler();
            var rangeHandler = new RangeCheckHandler();
            var obstacleHandler = new ObstacleCheckHandler();
            var resolutionHandler = new CatchResolutionHandler();

            // Link the chain
            _cooldownHandler
                .SetNext(_energyHandler)
                .SetNext(rangeHandler)
                .SetNext(obstacleHandler)
                .SetNext(resolutionHandler);

            _chainHead = _cooldownHandler;

            Console.WriteLine("🔗 Chain of Responsibility created with 5 handlers:");
            Console.WriteLine("   1. CooldownCheckHandler");
            Console.WriteLine("   2. EnergyCheckHandler");
            Console.WriteLine("   3. RangeCheckHandler");
            Console.WriteLine("   4. ObstacleCheckHandler");
            Console.WriteLine("   5. CatchResolutionHandler (final)\n");
        }

        public CatchAttemptContext ProcessCatchAttempt(Player player, Fish fish, Session session)
        {
            Console.WriteLine($"\n🎣 Processing catch attempt for player '{player.Name}'...");
            Console.WriteLine("══════════════════════════════════════════════════════════");

            var context = new CatchAttemptContext(player, fish, session);
            _chainHead.Handle(context);

            Console.WriteLine("══════════════════════════════════════════════════════════");
            Console.WriteLine($"Result: {(context.IsValid ? "✅ SUCCESS" : $"❌ FAILED - {context.FailureReason}")}\n");

            return context;
        }

        public void RestorePlayerEnergy(string connectionId, int amount)
        {
            _energyHandler.RestoreEnergyByConnectionId(connectionId, amount);
        }

        public string GetChainDescription()
        {
            return @"
CHAIN OF RESPONSIBILITY PATTERN - Catch Validation Pipeline
═══════════════════════════════════════════════════════════════════

This chain validates every catch attempt through 5 sequential handlers:

1️⃣  CooldownCheckHandler
    ↓ Checks if fishing rod cooldown expired (2 seconds)
    ↓ Prevents spam casting
    
2️⃣  EnergyCheckHandler  
    ↓ Verifies player has enough energy (10 per cast)
    ↓ Deducts energy cost if valid
    
3️⃣  RangeCheckHandler
    ↓ Calculates distance between rod and fish
    ↓ Ensures fish is within 150 units range
    
4️⃣  ObstacleCheckHandler
    ↓ Detects obstacles between rod and fish
    ↓ Blocks catch if path is obstructed
    
5️⃣  CatchResolutionHandler (Final)
    ✅ Marks fish as hooked
    ✅ Updates player score
    ✅ Logs successful catch

If ANY handler fails, the chain stops and the catch is denied.
This demonstrates the Chain of Responsibility pattern where each
handler decides whether to process and/or pass to the next handler.
═══════════════════════════════════════════════════════════════════";
        }
    }
}
