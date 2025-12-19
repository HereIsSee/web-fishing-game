namespace Api.Models
{
    public class DarkWaterSettings
    {
        public bool Enabled { get; set; }
        public float Alpha { get; set; } = 0.75f;
        public float VisibleRadius { get; set; } = 120f;
    }
}
