using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace DrawChatApp.Services
{
    public class PlayersService : IPlayersService
    {
        private readonly IMongoCollection<Player> _players;

        public PlayersService(IGameDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _players = database.GetCollection<Player>(settings.PlayersCollectionName);
        }

        public async Task<List<Player>> GetOrCreateListAsync(string listId)
        {
            return await _players.Find<Player>(x => x.RoomId == listId).ToListAsync() ?? new List<Player>();           
        }

        public async Task<List<Player>> AddOrUpdatePlayerAsync(string listId, Player player)
        {
            var newPlayer = _players
                .Find<Player>(x => x.PlayerId == player.PlayerId && x.ConnectionId == player.ConnectionId)
                .FirstOrDefaultAsync();
            if (newPlayer.Result == null)
            {
                await _players.InsertOneAsync(player);
            }
            else
            {
                await _players.ReplaceOneAsync(x => x.PlayerId == player.PlayerId && x.ConnectionId == player.ConnectionId, player);
            }

            return await GetOrCreateListAsync(listId);
        }

        public async Task<List<Player>> RemovePlayerAsync(string listId, string playerId)
        {
            await _players.DeleteOneAsync(x => x.PlayerId == playerId);
            return await GetOrCreateListAsync(listId);
        }

        public async Task DeleteListAsync(string listId)
        {
            await _players.DeleteManyAsync(x => x.RoomId == listId);
        }
    }
}
