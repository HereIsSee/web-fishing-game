using System.Linq;
using Api.Models.Dto;

namespace Api.Models.Dto
{
    public static class GameEnvironmentDtoFactory
    {
        public static GameEnvironmentDto FromEnvironment(GameEnvironment env)
        {
            // Defensive defaults
            var dto = new GameEnvironmentDto
            {
                Width = env.Width,
                Height = env.Height,
                WaterHeight = env.WaterLevelHeight,
                WaterColor = string.IsNullOrWhiteSpace(env.WaterColor) ? "#3b67b8e8" : env.WaterColor,
                SkyColor = string.IsNullOrWhiteSpace(env.SkyColor) ? "#7ec5cae8" : env.SkyColor,
                DarkWater = new DarkWaterDto
                {
                    Enabled = env.DarkWater?.Enabled ?? false,
                    Alpha = env.DarkWater?.Alpha ?? 0.75f,
                    VisibleRadius = env.DarkWater?.VisibleRadius ?? 120f
                },
                HazardZones = (env.HazardZones ?? new()).Select(z => new HazardZoneDto
                {
                    X = z.X,
                    Y = z.Y,
                    Radius = z.Radius,
                    SpeedMultiplier = z.SpeedMultiplier
                }).ToList()
            };

            // Ensure no null lists
            dto.HazardZones ??= new();

            return dto;
        }
    }
}
