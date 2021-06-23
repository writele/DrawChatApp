using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Pages
{
    public partial class Room : ComponentBase
    {
        #region FIELDS
        [Parameter]
        public string RoomId { get; set; }
        [Parameter]
        public string userName { get; set; } = "";

        public string ConnectionId { get; set; }
        public Player CurrentPlayer { get; set; }
        public RoomSettings Model { get; set; }

        public List<Player> Players { get; set; } = new List<Player>();
        public List<Word> Words { get; set; }
        public Word ActiveWord { get; set; }       
        public Player Winner { get; set; }
        public int ActiveRound { get; set; }
        public bool IsActiveGame;
        public bool IsActiveRound;

        private HubConnection hubConnection;
        public bool IsConnected =>
            hubConnection.State == HubConnectionState.Connected;
        #endregion

        #region INITIALIZATION
        protected override async Task OnInitializedAsync()
        {
            IsActiveGame = false;
            // Get Model -- pull from settings file
            Model = await JsonHelper.ReadJsonFile<RoomSettings>($"{RoomId}");

            // Get Words -- pull from json file
            Words = await JsonHelper.ReadJsonFile<List<Word>>(Constants.WordsDictionaryFileName);

            // Game Hub
            await InitializeGameHub();

            // Get current player


            // Set words
        }

        protected async Task InitializeGameHub()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
                .Build();

            hubConnection.On<string, List<Player>>("GetPlayers", (roomId, playersList) =>
            {
                Players = playersList;
                CurrentPlayer = playersList.Where(x => x.Name == userName).First();
                StateHasChanged();
            });

            await hubConnection.StartAsync();

            ConnectionId = await hubConnection.InvokeAsync<string>("GetConnectionId");
        }
        #endregion
        #region GAME
        private async Task AddPlayer()
        {
            var newPlayer = new Player();
            newPlayer.Name = userName;
            newPlayer.RoomId = RoomId;
            // Add player via GameHub
            List<Player> originalPlayers = new List<Player>(Players);
            await hubConnection.SendAsync("UpdatePlayers", ConnectionId, RoomId, originalPlayers, newPlayer);
        }
        private void StartGame()
        {
            // Reset Points
            // Set active player 
            // Set Active word
        }
        private void StartRound()
        {
            // Active Round ++
            // IsActiveRound = true

        }
        private void EndRound()
        {
            // if ActiveRound = MaxRound 
                // EndGame()
            // Else
                // Set Active word
                // Set Artist
                // IsActiveRound = false
        }

        private void OnChat(string input)
        {
            // if input = Active Word && !IsDone -> OnCorrectGuess()
            
        }

        private void OnCorrectGuess()
        {
            // Player.IsDone = true
            // Award points

            // Check if all players are Done
                // If so, EndRound
        }

        private void EndGame()
        {
            // IsActiveGame = false
        }
        #endregion
        #region CHAT
        #endregion
        #region WHITEBOARD
        #endregion

        public async ValueTask DisposeAsync()
        {
        }
    }
}
