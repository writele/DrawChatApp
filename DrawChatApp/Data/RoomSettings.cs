using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Data
{
    public class RoomSettings 
    { 
        public int RoomId { get; set; }
        //public List<Player> Players { get; set; }
        public string Name { get; set; } = "New Game";
        public string Password { get; set; }
        //public Word ActiveWord { get; set; }
        public List<Word> CustomWords { get; set; }
        public List<WordCategory> AllowedCategories { get; set; }
        //public int ActiveRound { get; set; }
        public int MaxRounds { get; set; }
        public int MaxTime { get; set; }
    }
}
