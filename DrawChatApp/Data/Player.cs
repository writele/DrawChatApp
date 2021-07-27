using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Data
{
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PlayerId { get; set; }
        public string ConnectionId { get; set; }
        public string RoomId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Points { get; set; } = 0;
        public bool IsHost { get; set; } = false;
        public bool IsArtist { get; set; } = false;
        public bool IsDone { get; set; } = false;
    }
}
