using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Services
{
    public class PlayersService : IPlayersService
    {
        public async Task<List<Player>> GetOrCreateListAsync(string listId)
        {
            return new List<Player>();
        }
        public async Task<List<Player>> AddOrUpdatePlayerAsync(string listId, Player player)
        {
            return new List<Player>();
        }
        public async Task<List<Player>> RemovePlayerAsync(string listId, string playerId)
        {
            return new List<Player>();
        }
        public async Task DeleteListAsync(string listId)
        {

        }
    }
}
