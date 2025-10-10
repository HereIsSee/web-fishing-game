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
        public async Task MoveBoat(string direction) // "left", "right",
        {
            // For now: just send event to all players
            await Clients.Others.SendAsync("BoatMoved", Context.ConnectionId, direction);

            Console.WriteLine($"Player {Context.ConnectionId} moved boat {direction}");
        }
        public async Task MoveBoatTo(float positionX)
        {
            Console.WriteLine($"🎯 MoveBoatTo called: Player {Context.ConnectionId} -> {positionX}");
            Console.WriteLine($"👥 Connected clients: {Context.ConnectionId} sending to others");
            await Clients.Others.SendAsync("BoatMovedTo", Context.ConnectionId, positionX);
            Console.WriteLine($"Player {Context.ConnectionId} moved to {positionX}");
        }
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _session.RemovePlayer(Context.ConnectionId);
            await Clients.Others.SendAsync("PlayerLeft", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}
