using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fleck;

public class WebSocketHandler
{
    private WebSocketServerService wsService;

    public WebSocketHandler(WebSocketServerService websocket)
    {
        wsService = websocket;
    }

    public async Task InitSlaveById(string id, string amount)
    {
        string message = $"INIT-{amount}";
        await wsService.SendMessageAsync(id, message);
    }

    public async Task AbortSlaves() { }

    public async Task AbortSlaveById() { }

    public bool DoesClientExist(string id)
    {
        return wsService.clients.ContainsKey(id);
    }
}
