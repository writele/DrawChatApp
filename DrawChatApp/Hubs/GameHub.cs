using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using DrawChatApp.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Hubs
{
    public class GameHub : Hub
    {
        MemoryCache<List<Player>> PlayersCache { get; set; } = new MemoryCache<List<Player>>();
        private readonly IPlayersService _playersService;

        public GameHub(IPlayersService playersService)
        {
            _playersService = playersService;
        }

        public string GetConnectionId() => Context.ConnectionId;

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        // Update Player in existing Player List
        public async Task UpdatePlayer(string roomId, Player updatedPlayer)
        {
            // Add playerId if needed
            var playerId = !string.IsNullOrEmpty(updatedPlayer.PlayerId) ? updatedPlayer.PlayerId : 
                $"{updatedPlayer.Name}{updatedPlayer.RoomId}";
                updatedPlayer.PlayerId = playerId;

            // Get (or create) the Players List for the Room Id
            var playersList = await PlayersCache.GetOrCreate(roomId, async () => await _playersService.GetOrCreateListAsync(roomId));

            if (playersList != null)
            {
                List<Player> updatedPlayersList = await PlayersCache.GetOrCreate(roomId, async () => await _playersService.AddOrUpdatePlayerAsync(roomId, updatedPlayer));

                if (updatedPlayersList != null)
                {
                    await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, updatedPlayersList);
                }
            }
        }

        public async Task CreateOrGetPlayersList(string roomId)
        {
            // Get (or create) Player List using Room Id
            var playersList = await PlayersCache.GetOrCreate(roomId, async () => await _playersService.GetOrCreateListAsync(roomId));

            if (playersList != null)
            {
                await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, playersList);
            }
        }

        public async Task UpdatePlayersList(string roomId, List<Player> updatedList)
        {
            foreach(var player in updatedList)
            {
                await _playersService.AddOrUpdatePlayerAsync(roomId, player);
            }

            // Get Player List using Room Id
            var playersList = await PlayersCache.GetOrCreate(roomId, async () => await _playersService.GetOrCreateListAsync(roomId));

            if (playersList != null)
            {
                await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, playersList);
            }
        }
    }
}
