namespace Api.Models
{
    public abstract class GameEnvironment
    {
        public int Id { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public int WaterLevelHeight { get; set; }
        public List<Fish> Fishes { get; set; } = new();
        public List<Obstacle> Obstacles { get; set; } = new();
        public string WaterColor { get; set; } = null!;
        public string SkyColor { get; set; } = null!;
        public List<HazardZone> HazardZones { get; set; } = new();
        public DarkWaterSettings DarkWater { get; set; } = new DarkWaterSettings { Enabled = false };
                public void Update()
        {
            PreUpdate();

            EnsureEnvironmentFeatures();     // hazard zones / dark water settings
            UpdateFish();                    // move fish
            ApplyEnvironmentEffects();       // optional additional effects
            MaintainPopulation();            // refill fish if needed

            PostUpdate();
        }

        // Optional hooks
        protected virtual void PreUpdate() { }
        protected virtual void PostUpdate() { }

        // Primitive operations (steps)
        protected virtual void UpdateFish()
        {
            foreach (var fish in Fishes)
                fish.UpdatePosition(Width, WaterLevelHeight);
        }

        protected virtual void ApplyEnvironmentEffects() { }

        protected abstract void MaintainPopulation();

        // Feature hooks (enabled/disabled by concrete environments)
        protected virtual bool EnableHazardZones() => false;
        protected virtual bool EnableDarkWater() => false;

        // Feature generators (overridden if enabled)
        protected virtual List<HazardZone> GenerateHazardZones() => new();
        protected virtual DarkWaterSettings GenerateDarkWaterSettings() => new DarkWaterSettings { Enabled = false };

        // Ensures properties are set (no nulls needed)
        private void EnsureEnvironmentFeatures()
        {
            // Hazards
            if (EnableHazardZones())
            {
                if (HazardZones == null || HazardZones.Count == 0)
                    HazardZones = GenerateHazardZones();
            }
            else
            {
                HazardZones = new List<HazardZone>();
            }

            // Dark water
            if (EnableDarkWater())
            {
                if (DarkWater == null || !DarkWater.Enabled)
                    DarkWater = GenerateDarkWaterSettings();
            }
            else
            {
                DarkWater = new DarkWaterSettings { Enabled = false };
            }
        }

        public abstract void DeleteFish(int fishId);
    }
}
