using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DrawChatApp.Data
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WordCategory
    {
        // Generic objects
        Thing,
        // Specific locations (countries, states, tourist spots, ect.)
        Place,
        // Fictional characters
        Character,
        // Actions and hobbies
        Activity,
        // Abstract concepts
        Concept
    }
}
