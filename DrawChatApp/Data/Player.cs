using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Data
{
    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public int Points { get; set; } = 0;
        public bool IsHost { get; set; } = false;
        public bool IsArtist { get; set; } = false;
    }
}
