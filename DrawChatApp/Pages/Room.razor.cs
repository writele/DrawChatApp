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
        [Inject]
        public IPlayersService PlayersService { get; set; } = null!;
        [Inject]
        public IRoomSettingsService SettingsService { get; set; } = null!;

        public string ConnectionId { get; set; }
        public Player CurrentPlayer { get; set; }
        public RoomSettings Model { get; set; }

        public List<Player> Players { get; set; } = new List<Player>();
        //public List<Word> Words { get; set; }
        //public Word ActiveWord { get; set; }       
        //public Player Winner { get; set; }
        //public int ActiveRound { get; set; }
        public bool IsActiveGame = false;
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
            //await hubConnection.InvokeAsync<RoomSettings>("GetRoomSettings", RoomId);
            //// Load current players
            //await hubConnection.InvokeAsync("CreateOrGetPlayersList", RoomId);
            await GetData();
            //StateHasChanged();
            //await RefreshRoom();
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
                //StateHasChanged();
            });

            hubConnection.On<string, RoomSettings>("GetRoomSettings", (roomId, settings) =>
            {
                Model = settings;
                IsActiveGame = Model.IsActiveGame;
                //StateHasChanged();
            });

            hubConnection.On<string>("OnRefresh", async (roomId) =>
            {
                await GetData();
                StateHasChanged();
            });

            //hubConnection.On<string>("OnNothing", /*async*/ (roomId) =>
            //{
                
            //});

            await hubConnection.StartAsync();
        }

        protected async Task GetData()
        {
            // Load current players
            await hubConnection.InvokeAsync("CreateOrGetPlayersList", RoomId);
            // Get Model -- pull from db
            await hubConnection.InvokeAsync("GetRoomSettings", RoomId);
            //StateHasChanged();
        }
        #endregion
        #region GAME
        private async Task RefreshRoom()
        {
            await hubConnection.InvokeAsync("Refresh", RoomId);
        }
        private async Task<bool> UpdateRoomSettings(/*RoomSettings model*/)
        {
            //await hubConnection.SendAsync("UpdateRoomSettings", RoomId, Model);
            var result = await SettingsService.UpdateRoomSettingsAsync(RoomId, Model);

            if (result != null)
                return true;

            return false;
        }

        private async Task<bool> UpdateCurrentPlayer()
        {
            //await hubConnection.SendAsync("UpdateRoomSettings", RoomId, Model);
            var result = await PlayersService.AddOrUpdatePlayerAsync(RoomId, CurrentPlayer);

            if (result != null)
                return true;

            return false;
        }

        private async Task<bool> UpdatePlayersList(/*List<Player> playersList*/)
        {
            foreach (var player in Players)
            {
                await PlayersService.AddOrUpdatePlayerAsync(RoomId, player);
            }

            return true;
        }
        private async Task<bool> AddPlayer()
        {
            CurrentPlayer = new Player();
            CurrentPlayer.Name = userName;
            CurrentPlayer.RoomId = RoomId;
            CurrentPlayer.ConnectionId = hubConnection.ConnectionId;
            // Add player via GameHub
            //await hubConnection.SendAsync("UpdatePlayer", RoomId, CurrentPlayer);
            return await UpdateCurrentPlayer();
        }
        private async Task UpdatePlayer()
        {
            bool result = false;

            if (CurrentPlayer == null || string.IsNullOrEmpty(CurrentPlayer.Name))
            {
                result = await AddPlayer();
            }
            else
            {
                CurrentPlayer.Name = userName;
                CurrentPlayer.RoomId = RoomId;
                CurrentPlayer.ConnectionId = hubConnection.ConnectionId;
                // Update player via GameHub
                //await hubConnection.SendAsync("UpdatePlayer", RoomId, CurrentPlayer);
                result = await UpdateCurrentPlayer();
            }

            if (result)
                await RefreshRoom();
        }
        private async Task StartGame()
        {
            // Reset Points
            // Set active player 
            //List<Player> updatedPlayers = new List<Player>(Players);
            Players.ForEach(x => x.Points = 0);
            Players.FirstOrDefault().IsArtist = true;
            
            //RoomSettings updatedModel = (RoomSettings)Model.Clone();
            // Set Active word
            var index = new Random().Next(Model.Words.Count);
            Model.ActiveWord = Model.Words[index];

            // set game to active
            Model.IsActiveGame = true;

            var playerSuccess = await UpdatePlayersList();
            var roomSuccess = await UpdateRoomSettings();

            //await GetData();
            //StateHasChanged();
            if (playerSuccess && roomSuccess)
                await RefreshRoom();
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
            await hubConnection.InvokeAsync("RemovePlayer", RoomId, CurrentPlayer);
            await hubConnection.InvokeAsync<string>("RemoveFromGroup", RoomId);           
        }
    }
}
