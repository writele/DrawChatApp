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
        #region CHAT fields
        private HubConnection hubConnection;
        private List<string> messages = new List<string>();
        private string userInput;
        private string messageInput;

        public bool IsConnected =>
            hubConnection.State == HubConnectionState.Connected;
        #endregion

        #region WHITEBOARD Fields
        //private HubConnection whiteboardConnection;

        //protected ElementReference _canvasContainer;
        //protected ElementReference _canvasReference;
        //private Context2D _context;

        //private bool mousedown = false;
        //private double canvasx;
        //private double canvasy;
        //private double last_mousex;
        //private double last_mousey;
        //private double mousex;
        //private double mousey;
        //private string clr = "black";

        //private class Position
        //{
        //    public double Left { get; set; }
        //    public double Top { get; set; }
        //}

        //public bool IsWhiteboardConnected =>
        //    whiteboardConnection.State == HubConnectionState.Connected;
        #endregion

        #region INITIALIZATION
        protected override async Task OnInitializedAsync()
        {
            await InitializeChatHub();
            //await InitializeWhiteboardHub();
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        _context = await js.GetContext2DAsync(_canvasReference);

        //        var p = await js.InvokeAsync<Position>("eval", $"let e = document.querySelector('[_bl_{_canvasContainer.Id}=\"\"]'); e = e.getBoundingClientRect(); e = {{ 'Left': e.x, 'Top': e.y }}; e");
        //        (canvasx, canvasy) = (p.Left, p.Top);
        //    }
        //}

        protected async Task InitializeChatHub()
        {
            // Connect to ChatHub
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
                .Build();

            // Set up ReceiveMessage function. Send() transfers info to ChatHub, which then calls this function
            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                messages.Add(encodedMsg);
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        //protected async Task InitializeWhiteboardHub()
        //{
        //    // Connect to ChatHub
        //    whiteboardConnection = new HubConnectionBuilder()
        //        .WithUrl(NavigationManager.ToAbsoluteUri("/whiteboardhub"))
        //        .Build();

        //    whiteboardConnection.On<double, double, double, double, string>("ReceiveDrawCanvasArgs", (prev_x, prev_y, x, y, clr) =>
        //    {
        //        DrawCanvasAsync(prev_x, prev_y, x, y, clr);
        //    });

        //    await whiteboardConnection.StartAsync();
        //}
        #endregion

        #region CHAT (Send to clients)
        Task Send() =>
            hubConnection.SendAsync("SendMessage", userInput, messageInput);
        #endregion


        #region WHITEBOARD Events
        // These methods are taken from Blazor.Canvas repo: TestProject_Components/Pages/Context2D/WhiteboardExample.razor
        //private void MouseDownCanvas(MouseEventArgs e)
        //{
        //    render_required = false;
        //    this.last_mousex = mousex = e.ClientX - canvasx;
        //    this.last_mousey = mousey = e.ClientY - canvasy;
        //    this.mousedown = true;
        //}

        //private void MouseUpCanvas(MouseEventArgs e)
        //{
        //    render_required = false;
        //    mousedown = false;
        //}

        //async Task MouseMoveCanvasAsync(MouseEventArgs e)
        //{
        //    render_required = false;
        //    if (!mousedown)
        //    {
        //        return;
        //    }
        //    mousex = e.ClientX - canvasx;
        //    mousey = e.ClientY - canvasy;
        //    await whiteboardConnection.SendAsync("SendDrawCanvasArgs", mousex, mousey, last_mousex, last_mousey, clr);
        //    last_mousex = mousex;
        //    last_mousey = mousey;
        //}

        //async Task DrawCanvasAsync(double prev_x, double prev_y, double x, double y, string clr)
        //{
        //    await using (var ctx2 = await _context.CreateBatchAsync())
        //    {
        //        await ctx2.BeginPathAsync();
        //        await ctx2.MoveToAsync(prev_x, prev_y);
        //        await ctx2.LineToAsync(x, y);
        //        await ctx2.StrokeAsync();
        //    }
        //}
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
