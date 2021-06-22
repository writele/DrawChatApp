using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Hubs
{
    public class GameHub : Hub
    {
        MemoryCache<List<Player>> PlayersCache { get; set; } = new MemoryCache<List<Player>>();
        public async Task UpdatePlayers(string roomId, List<Player> currentPlayers, Player updatedPlayer)
        {
            // Add playerId if needed
            var playerId = !string.IsNullOrEmpty(updatedPlayer.PlayerId) ? updatedPlayer.PlayerId : 
                $"{updatedPlayer.Name}{updatedPlayer.RoomId}";
                updatedPlayer.PlayerId = playerId;

            List<Player> playersList = await PlayersCache.GetOrCreate(roomId, currentPlayers);
            var originalPlayer = playersList.Where(x => x.PlayerId == playerId).First();
            var index = playersList.IndexOf(originalPlayer);

            if (index != -1)
            {
                playersList[index] = updatedPlayer;
            }
            else
            {
                playersList.Add(updatedPlayer);
            }
                

            List<Player> updatedPlayersList = await PlayersCache.GetOrCreate(roomId, playersList);

            await Clients.All.SendAsync("GetPlayers", roomId, updatedPlayersList);
        }
    }
}
