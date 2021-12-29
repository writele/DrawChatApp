using DrawChatApp.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace DrawChatApp.Components
{
    public partial class GameChat : ComponentBase
    {
        [Parameter]
        public string RoomId { get; set; } = "";
        [Parameter]
        public string UserName { get; set; } = "";
        [Parameter]
        public Word CorrectWord { get; set; }
        [Parameter]
        public EventCallback<string> OnCorrectGuess {  get; set; }

        #region CHAT fields
        private HubConnection hubConnection;
        private List<string> messages = new List<string>();
        private string messageInput;

        public bool IsConnected =>
            hubConnection.State == HubConnectionState.Connected;
        #endregion

        #region INITIALIZATION
        protected override async Task OnInitializedAsync()
        {
            await InitializeChatHub();
        }

        protected async Task InitializeChatHub()
        {
            // Connect to ChatHub
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
                .Build();

            // Set up ReceiveMessage function. Send() transfers info to ChatHub, which then calls this function
            hubConnection.On<string, string>("ReceiveMessage", async (userName, message) =>
            {
                var encodedMsg = $"{userName}: {message}";

                //if word is correct, invoke OnCorrectGuess
                if (CorrectWord.Name.ToLower() == message.ToLower() || CorrectWord.Spellings.Contains(message.ToLower()))
                {
                    encodedMsg = $"{userName} guessed the word!";
                    messages.Add(encodedMsg);
                    //await hubConnection.InvokeAsync("SendCorrectGuess", RoomId, userName);
                    //Send event callback through component
                    await OnCorrectGuess.InvokeAsync(userName);
                }
                else
                {
                    messages.Add(encodedMsg);
                    StateHasChanged();
                }                            
            });

            await hubConnection.StartAsync();
        }
        #endregion

        #region CHAT (Send to clients)
        Task Send() =>
            hubConnection.SendAsync("SendMessage", UserName, messageInput);
        #endregion

        private bool render_required = true;
        protected override bool ShouldRender()
        {
            if (!render_required)
            {
                render_required = true;
                return false;
            }
            return base.ShouldRender();
        }

        public async ValueTask DisposeAsync()
        {
            await hubConnection.DisposeAsync();
        }
    }
}
