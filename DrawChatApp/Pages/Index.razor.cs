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
        private HubConnection hubConnection;
        private List<string> messages = new List<string>();
        private string userInput;
        private string messageInput;

        protected ElementReference _canvasContainer;
        protected ElementReference _canvasReference;
        private Context2D _context;

        private bool mousedown = false;
        private double canvasx;
        private double canvasy;
        private double last_mousex;
        private double last_mousey;
        private double mousex;
        private double mousey;
        private string clr = "black";

        private class Position
        {
            public double Left { get; set; }
            public double Top { get; set; }
        }

        protected override async Task OnInitializedAsync()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl(NavigationManager.ToAbsoluteUri("/chathub"))
                .Build();

            hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
                messages.Add(encodedMsg);
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _context = await js.GetContext2DAsync(_canvasReference);

                var p = await js.InvokeAsync<Position>("eval", $"let e = document.querySelector('[_bl_{_canvasContainer.Id}=\"\"]'); e = e.getBoundingClientRect(); e = {{ 'Left': e.x, 'Top': e.y }}; e");
                (canvasx, canvasy) = (p.Left, p.Top);
            }
        }

        Task Send() =>
            hubConnection.SendAsync("SendMessage", userInput, messageInput);

        public bool IsConnected =>
            hubConnection.State == HubConnectionState.Connected;

        private void MouseDownCanvas(MouseEventArgs e)
        {
            render_required = false;
            this.last_mousex = mousex = e.ClientX - canvasx;
            this.last_mousey = mousey = e.ClientY - canvasy;
            this.mousedown = true;
        }

        private void MouseUpCanvas(MouseEventArgs e)
        {
            render_required = false;
            mousedown = false;
        }

        async Task MouseMoveCanvasAsync(MouseEventArgs e)
        {
            render_required = false;
            if (!mousedown)
            {
                return;
            }
            mousex = e.ClientX - canvasx;
            mousey = e.ClientY - canvasy;
            await DrawCanvasAsync(mousex, mousey, last_mousex, last_mousey, clr);
            last_mousex = mousex;
            last_mousey = mousey;
        }

        async Task DrawCanvasAsync(double prev_x, double prev_y, double x, double y, string clr)
        {
            await using (var ctx2 = await _context.CreateBatchAsync())
            {
                await ctx2.BeginPathAsync();
                await ctx2.MoveToAsync(prev_x, prev_y);
                await ctx2.LineToAsync(x, y);
                await ctx2.StrokeAsync();
            }

        }
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
