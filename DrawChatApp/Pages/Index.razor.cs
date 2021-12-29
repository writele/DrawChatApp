using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using DrawChatApp.Services;
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
        [Inject] private IRoomSettingsService Service { get; set; } = null!;

        private string roomNameInput;
        private bool IsNewGameEnabled => !string.IsNullOrEmpty(roomNameInput);

        public RoomSettings RoomSettings { get; set; } = new RoomSettings();

        protected override async Task OnInitializedAsync()
        {

        }

        public async Task CreateGame()
        {
            if (!IsNewGameEnabled)
            {
                return;
            }

            // Generate Room Id
            int randomId = new Random().Next(100,999);
            string roomId = $"{roomNameInput.Replace(' ', '-')}-{randomId}";

            // Generate Room Settings object
            RoomSettings.RoomId = roomId;
            RoomSettings.Name = roomNameInput;
            RoomSettings.MaxRounds = 1;
            RoomSettings.MaxTime = 30000;
            RoomSettings.IsActiveGame = false;
            RoomSettings.AllowedCategories = new List<WordCategory> { WordCategory.Thing, WordCategory.Character, WordCategory.Activity };

            // Get Words -- pull from json file
            RoomSettings.Words = await JsonHelper.ReadJsonFile<List<Word>>(Constants.WordsDictionaryFileName);

            // Create Room Settings
            await Service.CreateRoomSettingsAsync(roomId, RoomSettings);

            // Navigate to Room
            NavManager.NavigateTo($"/Room/{roomId}");
        }

        public async ValueTask DisposeAsync()
        {           
        }
    }
}
