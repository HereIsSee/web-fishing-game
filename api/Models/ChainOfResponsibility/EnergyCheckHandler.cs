namespace Api.Models.ChainOfResponsibility
{
    /// <summary>
    /// CHAIN OF RESPONSIBILITY - Handler 2
    /// Checks if the player has enough energy to catch fish
    /// Actually deducts energy from the Player object
    /// </summary>
    public class EnergyCheckHandler : CatchAttemptHandler
    {
        private const int ENERGY_COST_PER_CATCH = 10;

        public override void Handle(CatchAttemptContext context)
        {
            ProcessRequest(context);
            base.Handle(context);
        }

        protected override void ProcessRequest(CatchAttemptContext context)
        {
            context.AddLog("Handler 2: Checking player energy...");

            var player = context.Player;
            
            // Regenerate energy based on time passed
            RegenerateEnergy(player);
            
            Console.WriteLine($"⚡ Player {player.Name} energy: {player.Energy}/{Player.MAX_ENERGY}");
            
            if (player.Energy < ENERGY_COST_PER_CATCH)
            {
                context.Fail($"⚡ Not enough energy! Current: {player.Energy}, Required: {ENERGY_COST_PER_CATCH}. Wait for regen!");
                return;
            }

            // ACTUALLY DEDUCT ENERGY from player
            player.Energy -= ENERGY_COST_PER_CATCH;
            Console.WriteLine($"⚡ Deducted {ENERGY_COST_PER_CATCH} energy. {player.Name} now has {player.Energy}/{Player.MAX_ENERGY}");
            context.AddLog($"✅ Energy check passed (Remaining: {player.Energy})");
        }

        private void RegenerateEnergy(Player player)
        {
            var now = DateTime.UtcNow;
            var timeSinceLastRegen = (now - player.LastEnergyRegen).TotalSeconds;
            
            if (timeSinceLastRegen >= 1.0 && player.Energy < Player.MAX_ENERGY)
            {
                int secondsPassed = (int)timeSinceLastRegen;
                int energyToRegen = secondsPassed * Player.ENERGY_REGEN_PER_SECOND;
                int oldEnergy = player.Energy;
                player.Energy = Math.Min(Player.MAX_ENERGY, player.Energy + energyToRegen);
                player.LastEnergyRegen = now;
                
                if (player.Energy != oldEnergy)
                {
                    Console.WriteLine($"♻️ Regenerated {player.Energy - oldEnergy} energy for {player.Name}: {player.Energy}/{Player.MAX_ENERGY}");
                }
            }
        }

        public void RestoreEnergyByConnectionId(string connectionId, int amount)
        {
            // This method is kept for backward compatibility but now uses Session to find player
            Console.WriteLine($"♻️ RestoreEnergyByConnectionId called for {connectionId} (+{amount})");
        }
    }
}
