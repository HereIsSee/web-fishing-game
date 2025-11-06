namespace Api.Models
{
    public class ResetScoreCommand : ICommand
    {
        public void Execute(Player player)
        {
            player.Score = 0;
        }
    }
}

