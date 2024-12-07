using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Error handling in web sockets...

public class WebSocketService
{
    public string slaveId;
    private ClientWebSocket? webSocket;
    public event Action<string>? MessageReceived;
    
    public WebSocketService(string slaveId)
    {
        Console.WriteLine($"Slave ID is {slaveId}");
        this.slaveId = slaveId;
    }

    public async Task DisconnectAsync()
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        webSocket.Dispose();
        webSocket = null;
    }

    private async Task ReceiveMessages()
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        Console.WriteLine("Message is received.");
        var buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await DisconnectAsync();
            }
            else
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                MessageReceived?.Invoke(message);
            }
        }
    }

    public async Task ConnectAsync(string uri)
    {
        Console.WriteLine($"{uri}?slaveId={slaveId}");
        webSocket = new ClientWebSocket();

        await webSocket.ConnectAsync(new Uri($"{uri}?slaveId={slaveId}"), CancellationToken.None);
        await Task.Run(ReceiveMessages);
    }

    public async Task SendMessageAsync(string message)
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        Console.WriteLine($"Sending message with content: ${message}");
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}
