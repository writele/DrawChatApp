using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Data
{
    public class Word
    {
        public List<string> Spellings { get; set; }
        public WordCategory Category { get; set; }
    }
}
