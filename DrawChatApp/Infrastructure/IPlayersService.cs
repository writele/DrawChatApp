using DrawChatApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Infrastructure
{
    public interface IPlayersService
    {
        Task<List<Player>> GetOrCreateListAsync(string listId);
        Task<List<Player>> AddOrUpdatePlayerAsync(string listId, Player player);
        Task<List<Player>> RemovePlayerAsync(string listId, string playerId);
        Task DeleteListAsync(string listId);
    }
}
