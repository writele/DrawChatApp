using System.Collections.Generic;

namespace DrawChatApp.Data
{
    public class Word
    {
        public string Name { get; set; }
        public WordCategory WordCategory { get; set; }
        // Other accepted spellings of the word, may include close synonyms and/or common misspellings
        public string Spellings { get; set; }
    }
}
