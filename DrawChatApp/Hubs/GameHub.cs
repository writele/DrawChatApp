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

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task CreatePlayer(string roomId, List<Player> currentPlayers, Player updatedPlayer)
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

                List<Player> updatedPlayersList;
                if (currentPlayers.Count == 0)
                {
                    updatedPlayersList = await PlayersCache.GetOrCreate(roomId, playersList);
                }
                else
                {
                    updatedPlayersList = await PlayersCache.Update(roomId, playersList);
                }

                if (updatedPlayersList != null)
                {
                    await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, updatedPlayersList);
                }
            }
        }

        public async Task UpdatePlayer(string roomId, List<Player> currentPlayers, Player updatedPlayer)
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


                List<Player> updatedPlayersList = await PlayersCache.Update(roomId, playersList);

                if (updatedPlayersList != null)
                {
                    await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, updatedPlayersList);
                }
            }
        }

        public async Task UpdatePlayerList(string roomId, List<Player> newPlayersList)
        {
                List<Player> updatedPlayersList = await PlayersCache.Update(roomId, newPlayersList);

                if (updatedPlayersList != null)
                {
                    await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, updatedPlayersList);
                }
        }

        public async Task GetPlayerList(string roomId, List<Player> newPlayersList)
        {
            List<Player> updatedPlayersList = await PlayersCache.GetOrCreate(roomId, newPlayersList);

            if (updatedPlayersList != null)
            {
                await Clients.Groups(roomId).SendAsync("GetPlayers", roomId, updatedPlayersList);
            }
        }

    }
}
