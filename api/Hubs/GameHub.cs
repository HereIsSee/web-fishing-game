using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class GameHub : Hub
    {
        // Called when a player joins
        public async Task JoinGame(string playerName)
        {
            await Clients.All.SendAsync("PlayerJoined", playerName);
        }

        // Called when a player performs an action
        public async Task PlayerAction(string playerName, string action)
        {
            await Clients.All.SendAsync("UpdateAction", playerName, action);
        }
    }
}
