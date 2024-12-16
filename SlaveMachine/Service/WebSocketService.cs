using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

// Error handling in web sockets...

public class WebSocketService
{
    private CancellationTokenSource cts;
    private CancellationToken token;

    public string slaveId;
    private ClientWebSocket? webSocket;
    public event Action<string>? MessageReceived;
    public event Action? OnClientConnected;
    public event Action? OnClientDisconnected;

    public WebSocketService(string slaveId, bool isEventOnly = false)
    {
        if (!isEventOnly)
        {
            Console.WriteLine($"Slave ID is {slaveId}");
            this.slaveId = slaveId;
            webSocket = new ClientWebSocket();
        }
    }

    private void GenerateToken(){
        cts = new CancellationTokenSource();
        token = cts.Token;
    }

    public async Task DisconnectAsync()
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        OnClientDisconnected?.Invoke();
        webSocket.Dispose();
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

    // This is why we need a handler. The service provides the basis of working and the handler, the logic.
    private async Task DoHeartbeat()
    {
        string heartbeatString = "HB";

        while (!token.IsCancellationRequested)
        {
            _ = SendMessageAsync(heartbeatString);
            await Task.Delay(10000, token);     // 10 secs o cada minuto?
        }
    }

    // See how to make things retry the connection in case the server is down.
    public async Task ConnectAsync(string uri)
    {
        Console.WriteLine($"{uri}?slaveId={slaveId}");

        await webSocket.ConnectAsync(new Uri($"{uri}?slaveId={slaveId}"), token);
        GenerateToken();

        OnClientConnected?.Invoke();

        await Task.Run(DoHeartbeat, token);
        await Task.Run(ReceiveMessages, token);
    }

    public async Task SendMessageAsync(string message)
    {
        if (webSocket == null || webSocket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        try
        {
            Console.WriteLine($"Sending message with content: ${message}");
            var messageBytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception)
        {
            if (message.Equals("HB"))
            {
                Console.WriteLine("Heartbeat stop beating");
                cts.Cancel();
                await DisconnectAsync();
            }
            else
            {
                throw new ConnectException("Socket server is not online");
            }
        }
    }
}
