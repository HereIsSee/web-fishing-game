using Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class GameHub : Hub
    {
        private static Session _session = new Session(); // Visada yra

        public async Task JoinSession(string playerName)
        {
            Console.WriteLine($"🎣 Player {playerName} joining session...");
            
            // When a player joins the game create a new boat for him
            _session.AddPlayer(Context.ConnectionId, playerName);
            await Clients.All.SendAsync("PlayerJoined", playerName);
            
            Console.WriteLine($"✅ Player {playerName} joined!");
        }
        public async Task LeaveSession()
        {
            _session.RemovePlayer(Context.ConnectionId);
            await Clients.All.SendAsync("PlayerLeft", Context.ConnectionId);
        }
        public async Task StartGame()
        {
            // Paleisti žaidimą iškart, net jei mažai žaidėjų
            _session.StartGame();
            await Clients.All.SendAsync("GameStarted", _session.TimerDuration);
        }
        public async Task PlayerMove(double PositionX)
        {
            _session.UpdatePlayerPosition(Context.ConnectionId, PositionX);
            var player = _session.GetPlayer(Context.ConnectionId);

            await Clients.All.SendAsync("PlayerMove", player);
        } 

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _session.RemovePlayer(Context.ConnectionId);
            await Clients.Others.SendAsync("PlayerLeft", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}
