namespace Api.Models
{
    public interface ICommand
    {
        void Execute(Player player);
    }
}
