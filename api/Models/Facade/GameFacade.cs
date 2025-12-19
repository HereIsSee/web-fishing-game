namespace Api.Models.Facade
{
    using Api.Models;
    using Api.Models.Proxy;
    
    public class GameFacade : IGameFacade
    {
        private readonly IGameRenderingSubsystem _renderingSubsystem;
        private readonly IGameLogicSubsystem _gameLogicSubsystem;
        private readonly IAudioSubsystem _audioSubsystem;

        public GameFacade()
        {
            _renderingSubsystem = new GameRenderingSubsystem();
            _gameLogicSubsystem = new GameLogicSubsystem();
            _audioSubsystem = new AudioSubsystem();
        }

        public void InitializeGame()
        {
            _renderingSubsystem.ClearScreen();
            _audioSubsystem.PlayAmbientSound();
        }

        public bool AttemptFishCatch(Player player, Fish fish)
        {
            bool caught = _gameLogicSubsystem.TryFishCatch(player, fish);
            
            if (caught)
            {
                _renderingSubsystem.RenderFish(fish);
                _gameLogicSubsystem.UpdatePlayerScore(player, fish.Points);
                _audioSubsystem.PlayCatchSound();
            }
            else
            {
                _audioSubsystem.PlayMissSound();
            }

            return caught;
        }

        public void ApplyEffect(Player player, Decorator.IFishDecorator decorator)
        {
            _gameLogicSubsystem.ApplyDecoratorEffect(player, decorator);
            _renderingSubsystem.RenderPlayer(player);
            _audioSubsystem.PlayCatchSound();
        }

        public void RenderFrame(Player player)
        {
            _renderingSubsystem.RenderPlayer(player);
            _audioSubsystem.PlayAmbientSound();
        }

        public void RenderAllPlayers(List<Player> players)
        {
            foreach (var player in players)
            {
                _renderingSubsystem.RenderPlayer(player);
            }
            _audioSubsystem.PlayAmbientSound();
        }

        public void UpdateAllPlayerScores(List<Player> players)
        {
            foreach (var player in players)
            {
                _gameLogicSubsystem.UpdatePlayerScore(player, 0); 
            }
        }

        public void PlayGameOverSound()
        {
            _audioSubsystem.PlayMissSound();
        }

        public void PlaySuccessSound()
        {
            _audioSubsystem.PlayCatchSound();
        }

        public void UpdateGameState()
        {
            // Placeholder for game state updates
            Console.WriteLine("🎮 Updating game state...");
        }

        public object GetSystemReport()
        {
            return new
            {
                RenderingReport = _renderingSubsystem.GetRenderReport(),
                GameLogicReport = _gameLogicSubsystem.GetGameLogicReport(),
                AudioReport = _audioSubsystem.GetAudioReport()
            };
        }
    }
}
