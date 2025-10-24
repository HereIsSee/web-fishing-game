using Api.Models;
using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs
{
    public class GameHub : Hub
    {
        private static Session _session = new Session(); // Visada yra
        private readonly IHubContext<GameHub> _hubContext;
        private readonly ILogger<GameHub> _logger;

        public GameHub(IHubContext<GameHub> hubContext, ILogger<GameHub> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task JoinSession(string playerName)
        {
            Console.WriteLine($"🎣 Player {playerName} joining session...");

            // When a player joins the game create a new boat for him
            _session.AddPlayer(Context.ConnectionId, playerName);

            var player = _session.GetPlayer(Context.ConnectionId);

            var allPlayers = _session.GetAllPlayers();
            await Clients.Caller.SendAsync("ReceiveAllPlayers", allPlayers);

            await Clients.All.SendAsync("PlayerJoined", player);
            await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);

            Console.WriteLine($"✅ Player {playerName} joined!");
            Console.WriteLine($"Player {Context.ConnectionId} connection id!");
            Console.WriteLine($"📊 Sent {allPlayers.Count} existing players to new player");

            Console.WriteLine(_session.IsActive);
            if (!_session.IsActive)
            {
                Console.WriteLine("irst player joined — starting game automatically!");
                await StartGame();
            }
        }
        public async Task LeaveSession()
        {
            _session.RemovePlayer(Context.ConnectionId);
            await Clients.All.SendAsync("PlayerLeft", Context.ConnectionId);
        }

        public async Task StartGame()
        {
            _session.StartGame();
            await Clients.All.SendAsync("GameStarted", _session.TimerDuration);
            _ = Task.Run(async () =>
            {
                while (_session.IsActive)
                {
                    _session.Environment.Update();

                    await _hubContext.Clients.All.SendAsync("UpdateFishes", _session.Environment.Fishes);
                    await Task.Delay(10);
                }

            });
        }

        public async Task MoveBoatTo(float positionX)
        {
            var player = _session.GetPlayer(Context.ConnectionId);

            player.UpdateBoatPosition(positionX);

            await Clients.All.SendAsync("BoatMovedTo", player);

            Console.WriteLine($"Player {Context.ConnectionId} moved to {positionX}");
        }
        public async Task ToggleFishingRodCast()
        {
            var player = _session.GetPlayer(Context.ConnectionId);

            // Update hook position
            player.ToggleFishingRodCast();

            await Clients.All.SendAsync("FishingRodCastChanged", player);

            Console.WriteLine($"Player {Context.ConnectionId} has toggled his cast");
        } 
        public async Task MoveHook(float positionX, float positionY)
        {
            var player = _session.GetPlayer(Context.ConnectionId);

            if (player == null) return;

            // Update hook position
            player.FishingRod.PositionX = positionX;
            player.FishingRod.PositionY = positionY;

            await Clients.All.SendAsync("HookMovedTo", player);

            Console.WriteLine($"Player {Context.ConnectionId} hook moved to {positionX} {positionY}");
        } 
        
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _session.RemovePlayer(Context.ConnectionId);
            await Clients.Others.SendAsync("PlayerLeft", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
        
    }
}
