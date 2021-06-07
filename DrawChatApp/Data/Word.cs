using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Data
{
    public class Word
    {
        public string Name { get; set; }
        public WordCategory Category { get; set; }
        // Other accepted spellings of the word, may include close synonyms and/or common misspellings
        public List<string> Spellings { get; set; }
    }
}
