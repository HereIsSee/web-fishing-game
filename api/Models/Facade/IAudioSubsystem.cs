namespace Api.Models.Facade
{
    public interface IAudioSubsystem
    {
        void PlayCatchSound();
        void PlayMissSound();
        void PlayAmbientSound();
        string GetAudioReport();
    }
}
