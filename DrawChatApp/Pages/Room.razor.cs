using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using DrawChatApp.Services;
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
        //public List<Word> Words { get; set; }
        //public Word ActiveWord { get; set; }       
        //public Player Winner { get; set; }
        //public int ActiveRound { get; set; }
        //public bool IsActiveGame;
        //public bool IsActiveRound;

        private HubConnection hubConnection;
        public bool IsConnected =>
            hubConnection.State == HubConnectionState.Connected;
        #endregion

        #region INITIALIZATION
        protected override async Task OnInitializedAsync()
        {
            // Game Hub
            await InitializeGameHub();

            // Add user to client group
            await hubConnection.InvokeAsync<string>("AddToGroup", RoomId);
            // Add player
            //await AddPlayer();
            // Get Model -- pull from db
            await hubConnection.InvokeAsync<RoomSettings>("GetRoomSettings", RoomId);
            // Load current players
            await hubConnection.InvokeAsync("CreateOrGetPlayersList", RoomId);
        }

        protected async Task InitializeGameHub()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/gamehub"))
                .Build();

            hubConnection.On<string, List<Player>>("GetPlayers", (roomId, playersList) =>
            {
                Players = playersList;
                if (Players.Count > 0)
                    CurrentPlayer = playersList.Where(x => x.Name == userName && x.ConnectionId == hubConnection.ConnectionId)
                    .FirstOrDefault();
                StateHasChanged();
            });

            hubConnection.On<string, RoomSettings>("GetRoomSettings", (roomId, settings) =>
            {
                Model = settings;
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }
        #endregion
        #region GAME
        private async Task UpdateRoomSettings()
        {
            await hubConnection.InvokeAsync<RoomSettings>("UpdateRoomSettings", RoomId, Model);
        }
        private async Task UpdatePlayersList()
        {
            await hubConnection.SendAsync("UpdatePlayersList", RoomId, Players);
        }
        private async Task AddPlayer()
        {
            var newPlayer = new Player();
            newPlayer.Name = userName;
            newPlayer.RoomId = RoomId;
            newPlayer.ConnectionId = hubConnection.ConnectionId;
            // Add player via GameHub
            await hubConnection.SendAsync("UpdatePlayer", RoomId, newPlayer);
        }
        private async Task UpdatePlayer()
        {
            if (CurrentPlayer == null || string.IsNullOrEmpty(CurrentPlayer.Name))
            {
                await AddPlayer();
            }
            else
            {
                CurrentPlayer.Name = userName;
                CurrentPlayer.RoomId = RoomId;
                CurrentPlayer.ConnectionId = hubConnection.ConnectionId;
                // Update player via GameHub
                await hubConnection.SendAsync("UpdatePlayer", RoomId, CurrentPlayer);
            }
        }
        private async Task StartGame()
        {
            // Reset Points
            // Set active player 
            //List<Player> updatedPlayers = new List<Player>(Players);
            Players.ForEach(x => x.Points = 0);
            Players.FirstOrDefault().IsArtist = true;
            await UpdatePlayersList();

            // Set Active word
            var index = new Random().Next(Model.Words.Count);
            Model.ActiveWord = Model.Words[index];

            // set game to active
            Model.IsActiveGame = true;

            await UpdateRoomSettings();
            StateHasChanged();
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
            await hubConnection.InvokeAsync<string>("RemoveFromGroup", RoomId);
            await hubConnection.InvokeAsync("RemovePlayer", RoomId, CurrentPlayer);
        }
    }
}
