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
    public class RoomSettingsService : IRoomSettingsService
    {
        private readonly IMongoCollection<RoomSettings> _settings;

        public RoomSettingsService(IGameDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _settings = database.GetCollection<RoomSettings>(settings.RoomSettingsCollectionName);
        }

        public async Task<RoomSettings> GetRoomSettingsAsync(string roomId)
        {
            return await _settings.Find<RoomSettings>(x => x.RoomId == roomId).FirstOrDefaultAsync();
        }

        public async Task<RoomSettings> CreateRoomSettingsAsync(string roomId, RoomSettings settings)
        {
            await _settings.InsertOneAsync(settings);
            return await GetRoomSettingsAsync(roomId);
        }

        public async Task<RoomSettings> UpdateRoomSettingsAsync(string roomId, RoomSettings settings)
        {
            await _settings.ReplaceOneAsync(x => x.RoomId == settings.RoomId, settings);
            return await GetRoomSettingsAsync(roomId);
        }

        public async Task DeleteRoomSettingsAsync(string roomId)
        {
            await _settings.DeleteManyAsync(x => x.RoomId == roomId);
        }
    }
}
