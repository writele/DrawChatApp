using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Hubs
{
    public class GameHub : Hub
    {
        MemoryCache<List<Player>> PlayersCache { get; set; } = new MemoryCache<List<Player>>();

        public string GetConnectionId() => Context.ConnectionId;


        public async Task UpdatePlayers(string connectionId, string roomId, List<Player> currentPlayers, Player updatedPlayer)
        {
            // Add playerId if needed
            var playerId = !string.IsNullOrEmpty(updatedPlayer.PlayerId) ? updatedPlayer.PlayerId : 
                $"{updatedPlayer.Name}{updatedPlayer.RoomId}";
                updatedPlayer.PlayerId = playerId;

            var playersList = await PlayersCache.GetOrCreate(roomId, currentPlayers);

            if (playersList != null)
            {
                var originalPlayer = playersList.Where(x => x.PlayerId == playerId).FirstOrDefault();
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

                if (updatedPlayersList != null)
                {
                    await Clients.Client(connectionId).SendAsync("GetPlayers", roomId, updatedPlayersList);
                }
            }
        }
    }
}
