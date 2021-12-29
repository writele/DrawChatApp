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
        public Player CurrentPlayer { get; set; } = new Player();
        public RoomSettings Model { get; set; }

        public List<Player> Players { get; set; } = new List<Player>();

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
            await GetData();
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
                    .FirstOrDefault() ?? new Player();
                StateHasChanged();
            });

            hubConnection.On<string, RoomSettings>("GetRoomSettings", (roomId, settings) =>
            {
                Model = settings;
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        protected async Task GetData()
        {
            // Load current players
            await hubConnection.InvokeAsync("CreateOrGetPlayersList", RoomId);
            // Get Model -- pull from db
            await hubConnection.InvokeAsync("GetRoomSettings", RoomId);
        }
        #endregion
        #region GAME
        private async Task RefreshRoom()
        {
            await hubConnection.InvokeAsync("Refresh", RoomId);
        }
        private async Task<bool> UpdateRoomSettings()
        {
            var result = await SettingsService.UpdateRoomSettingsAsync(RoomId, Model);

            if (result != null)
                return true;

            return false;
        }

        private async Task<bool> UpdateCurrentPlayer()
        {
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
            CurrentPlayer.Name = userName;
            CurrentPlayer.RoomId = RoomId;
            CurrentPlayer.ConnectionId = hubConnection.ConnectionId;
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
                result = await UpdateCurrentPlayer();
            }

            if (result)
                await RefreshRoom();
        }
        private async Task StartGame()
        {
            // Reset Points
            // Set active player 
            Players.ForEach(x => x.Points = 0);
            Players.FirstOrDefault().IsArtist = true;
            
            // Set Active word
            var index = new Random().Next(Model.Words.Count);
            Model.ActiveWord = Model.Words[index];

            // set game to active
            Model.IsActiveGame = true;

            var playerSuccess = await UpdatePlayersList();
            var roomSuccess = await UpdateRoomSettings();

            if (playerSuccess && roomSuccess)
                await RefreshRoom();
        }
        private async void OnCorrectGuess(string winningUsername)
        {

            if (CurrentPlayer.Name == winningUsername)
            {
                CurrentPlayer.IsDone = true;
                // Award points
                CurrentPlayer.Points = CurrentPlayer.Points + 10;
                await UpdateCurrentPlayer();
            }
            else
            {
                await RefreshRoom();
            }
            // Check if all players are Done
            // If so, EndRound
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
