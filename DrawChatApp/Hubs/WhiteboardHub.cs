using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawChatApp.Hubs
{
    public class WhiteboardHub : Hub
    {
        public async Task SendDrawCanvasArgs(double prev_x, double prev_y, double x, double y, string clr)
        {
            await Clients.All.SendAsync("ReceiveDrawCanvasArgs", prev_x, prev_y, x, y, clr);
        }
    }
}
