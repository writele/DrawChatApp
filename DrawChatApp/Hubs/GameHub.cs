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
        //MemoryCache<List<Player>> PlayersCache { get; set; } = new MemoryCache<List<Player>>();
        private readonly IPlayersService _playersService;
        private readonly IRoomSettingsService _settingsService;

        public GameHub(IPlayersService playersService, IRoomSettingsService settingsService)
        {
            _playersService = playersService;
            _settingsService = settingsService;
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

        public async Task Refresh(string roomId)
        {
            await Clients.Groups(roomId).SendAsync("OnRefresh", roomId);
        }

        #region ROOM SETTINGS
        public async Task GetRoomSettings(string roomId)
        {
            // Get Room Settings using Room Id
            var settings = await _settingsService.GetRoomSettingsAsync(roomId);

            if (settings != null)
            {
                await Clients.Groups(roomId).SendAsync("GetRoomSettings", roomId, settings);
            }
        }

        public async Task CreateRoomSettings(string roomId, RoomSettings newSettings)
        {
            // Update Room Settings using Room Id
            var settings = await _settingsService.CreateRoomSettingsAsync(roomId, newSettings);

            if (settings != null)
            {
                await Clients.Groups(roomId).SendAsync("GetRoomSettings", roomId, settings);
            }
        }

        public async Task UpdateRoomSettings(string roomId, RoomSettings updatedSettings)
        {
            // Update Room Settings using Room Id
            var settings = await _settingsService.UpdateRoomSettingsAsync(roomId, updatedSettings);

            if (settings != null)
            {
                //await Clients.Groups(roomId).SendAsync("GetRoomSettings", roomId, settings);
                //await Clients.Groups(roomId).SendAsync("OnNothing", roomId);
            }
        }

        public async Task DeleteRoomSettings(string roomId)
        {
            await _settingsService.DeleteRoomSettingsAsync(roomId);
        }
        #endregion

        #region PLAYERS
        // Update Player in existing Player List
        public async Task CreateOrGetPlayersList(string roomId)
        {
            // Get (or create) Player List using Room Id
            var playersList = await _playersService.GetOrCreateListAsync(roomId);

            if (playersList != null)
            {
                await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, playersList);
            }
        }

        public async Task UpdatePlayer(string roomId, Player updatedPlayer)
        {
            // Add playerId if needed
            var playerId = !string.IsNullOrEmpty(updatedPlayer.PlayerId) ? updatedPlayer.PlayerId : 
                $"{updatedPlayer.Name}{updatedPlayer.RoomId}";
                updatedPlayer.PlayerId = playerId;

            // Get (or create) the Players List for the Room Id
            var playersList = await _playersService.GetOrCreateListAsync(roomId);

            if (playersList != null)
            {
                List<Player> updatedPlayersList = await _playersService.AddOrUpdatePlayerAsync(roomId, updatedPlayer);

                if (updatedPlayersList != null)
                {
                    //await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, updatedPlayersList);
                    //await Clients.Groups(roomId).SendAsync("OnNothing", roomId);
                }
            }
        }

        public async Task UpdatePlayersList(string roomId, List<Player> updatedList)
        {
            foreach(var player in updatedList)
            {
                await _playersService.AddOrUpdatePlayerAsync(roomId, player);
            }

            // Get Player List using Room Id
            var playersList = await _playersService.GetOrCreateListAsync(roomId);

            if (playersList != null)
            {
                //await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, playersList);
                //await Clients.Groups(roomId).SendAsync("OnNothing", roomId);
            }
        }

        public async Task RemovePlayer(string roomId, Player currentPlayer)
        {
            await _playersService.RemovePlayerAsync(roomId, currentPlayer.PlayerId);
        }
        #endregion
     
    }
}
