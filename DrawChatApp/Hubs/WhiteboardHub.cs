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
    }
}
