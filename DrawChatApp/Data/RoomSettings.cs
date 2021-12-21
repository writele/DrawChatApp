using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Data
{
    public class RoomSettings 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string RoomId { get; set; }
        //public List<Player> Players { get; set; }
        public string ConnectionId { get; set; }
        public string Name { get; set; } = "New Game";
        public string PlayerHostName { get; set; }
        public string Password { get; set; }
        public List<Word> CustomWords { get; set; } 
        public List<WordCategory> AllowedCategories { get; set; }
        public int MaxRounds { get; set; }
        public int MaxTime { get; set; }
        public List<Word> Words { get; set; }
        public Word ActiveWord { get; set; }
        public Player Winner { get; set; }
        public int ActiveRound { get; set; }
        public bool IsActiveGame;
        public bool IsActiveRound;
    }
}
