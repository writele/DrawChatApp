using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Data
{
    public enum WordCategory
    {
        // Generic objects
        Thing = 1,
        // Specific locations (countries, states, tourist spots, ect.)
        Place = 2,
        // Fictional characters
        Character = 3,
        // Actions and hobbies
        Activity = 4,
        // Abstract concepts
        Concept = 5
    }
}
