using System.Collections.Generic;

namespace Api.Models.Dto
{
    public class GameEnvironmentDto
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int WaterHeight { get; set; }
        public string WaterColor { get; set; } = "#3b67b8e8";
        public string SkyColor { get; set; } = "#7ec5cae8";

        public List<HazardZoneDto> HazardZones { get; set; } = new();
        public DarkWaterDto DarkWater { get; set; } = new DarkWaterDto { Enabled = false };
    }

    public class HazardZoneDto
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public float SpeedMultiplier { get; set; }
    }

    public class DarkWaterDto
    {
        public bool Enabled { get; set; }
        public float Alpha { get; set; }
        public float VisibleRadius { get; set; }
    }
}
