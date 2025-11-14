namespace Api.Models.Facade
{
    using Api.Models;
    public class AudioSubsystem : IAudioSubsystem
    {
        private List<string> audioLog = new();

        public void PlayCatchSound()
        {
            audioLog.Add("🔊 Playing catch sound (success_chime.wav)");
        }

        public void PlayMissSound()
        {
            audioLog.Add("🔊 Playing miss sound (fail_beep.wav)");
        }

        public void PlayAmbientSound()
        {
            audioLog.Add("🔊 Playing ambient sound (water_loop.wav)");
        }

        public string GetAudioReport()
        {
            return string.Join(" | ", audioLog);
        }
    }
}
