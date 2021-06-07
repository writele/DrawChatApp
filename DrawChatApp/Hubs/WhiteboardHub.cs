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
        public void DrawShape(int shape, string fillColor, string color,
            int size, int x, int y, int x1, int y1)
        {
            //Clients.All.Draw(shape, fillColor, color, size, x, y, x1, y1);
        }

        public async Task SendDrawCanvasArgs(double prev_x, double prev_y, double x, double y, string clr)
        {
            await Clients.All.SendAsync("ReceiveDrawCanvasArgs", prev_x, prev_y, x, y, clr);
        }

        //public async Task SendMouseDownArgs(MouseEventArgs e)
        //{
        //    await Clients.All.SendAsync("ReceiveMouseDownArgs", e);
        //}

        //public async Task SendMouseUpArgs(MouseEventArgs e)
        //{
        //    await Clients.All.SendAsync("ReceiveMouseUpArgs", e);
        //}
    }
}
