using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using Excubo.Blazor.Canvas;
using Excubo.Blazor.Canvas.Contexts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] private NavigationManager NavManager { get; set; } = null!;

        private string userNameInput;
        private string roomNameInput;

        public RoomSettings RoomSettings { get; set; } = new RoomSettings();

        protected override async Task OnInitializedAsync()
        {

        }

        public async Task CreateGame()
        {
            // Generate Room Id
            int randomId = new Random().Next(100,999);
            string roomId = $"{roomNameInput}-{randomId}";

            // Generate Room Settings object
            RoomSettings.RoomId = roomId;
            RoomSettings.Name = roomNameInput;
            RoomSettings.MaxRounds = 1;
            RoomSettings.MaxTime = 30000;
            RoomSettings.PlayerHostName = userNameInput;
            RoomSettings.AllowedCategories = new List<WordCategory> { WordCategory.Thing, WordCategory.Character, WordCategory.Activity };

            // Create Room Settings file
            await JsonHelper.CreateJsonFile($"{roomId}", RoomSettings);

            // Navigate to Room
            NavManager.NavigateTo($"/Room/{roomId}?userName={userNameInput}");
        }

        public bool IsNewGameEnabled()
        {
            return !string.IsNullOrEmpty(userNameInput) && !string.IsNullOrEmpty(roomNameInput);
        }




        public async ValueTask DisposeAsync()
        {           
        }
    }
}
