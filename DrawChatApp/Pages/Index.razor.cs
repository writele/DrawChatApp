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

        public RoomSettings RoomSettings { get; set; } = new RoomSettings();

        //private HubConnection hubConnection;
        //public bool IsConnected =>
        //    hubConnection.State == HubConnectionState.Connected;

        protected override async Task OnInitializedAsync()
        {
            //hubConnection = new HubConnectionBuilder()
            //    .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
            //    .Build();


            //await hubConnection.StartAsync();
        }

        public async Task CreateGame()
        {
            // Generate Room Id
            int randomId = new Random().Next(100,999);
            string roomId = $"{roomNameInput.Replace(' ', '-')}-{randomId}";

            // Generate Room Settings object
            RoomSettings.RoomId = roomId;
            RoomSettings.Name = roomNameInput;
            RoomSettings.MaxRounds = 1;
            RoomSettings.MaxTime = 30000;
            RoomSettings.IsActiveGame = false;
            //RoomSettings.PlayerHostName = userNameInput;
            RoomSettings.AllowedCategories = new List<WordCategory> { WordCategory.Thing, WordCategory.Character, WordCategory.Activity };

            // Get Words -- pull from json file
            RoomSettings.Words = await JsonHelper.ReadJsonFile<List<Word>>(Constants.WordsDictionaryFileName);

            // Create Room Settings
            await Service.CreateRoomSettingsAsync(roomId, RoomSettings);

            // Navigate to Room
            NavManager.NavigateTo($"/Room/{roomId}");
        }

        public bool IsNewGameEnabled()
        {
            return !string.IsNullOrEmpty(roomNameInput);
        }


        public async ValueTask DisposeAsync()
        {           
        }
    }
}
