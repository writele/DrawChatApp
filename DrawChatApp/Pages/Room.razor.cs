using DrawChatApp.Data;
using DrawChatApp.Infrastructure;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;

namespace DrawChatApp.Pages
{
    public partial class Room : ComponentBase
    {
        #region FIELDS
        [Parameter]
        public string RoomId { get; set; }
        [Parameter]
        public string userName { get; set; }

        public Player CurrentPlayer { get; set; }
        public RoomSettings Model { get; set; }

        public List<Player> Players { get; set; }
        public List<Word> Words { get; set; }
        public Word ActiveWord { get; set; }       
        public Player Winner { get; set; }
        public int ActiveRound { get; set; }
        public bool IsActiveGame;
        public bool IsActiveRound;
        #endregion

        #region INITIALIZATION
        protected override async Task OnInitializedAsync()
        {
            // Get Model -- pull from settings file
            Model = await JsonHelper.ReadJsonFile<RoomSettings>($"{RoomId}");

            // Get Words -- pull from json file
            Words = await JsonHelper.ReadJsonFile<List<Word>>(Constants.WordsDictionaryFileName);

            // Game Hub
            // Get current player


            // Set words
        }
        #endregion
        #region GAME
        private void AddPlayer()
        {
            // Add player via GameHub
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
