using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;

// Error handling in web sockets...

public class WebSocketService
{
    public string slaveId;
    public string wsServerAddr;
    public ClientWebSocket webSocket;
    public event Action<string>? MessageReceived;
    public event Action? OnClientConnected;
    public event Action? OnClientDisconnected;

    public WebSocketService()
    {
        this.slaveId = "1"; //Environment.GetEnvironmentVariable("UNIRUBBER_MACHINE_ID");
        this.wsServerAddr = "ws://127.0.0.1:8181"; //Environment.GetEnvironmentVariable("UNIRUBBER_MASTER_ADDRESS"); // "ws://127.0.0.1:8181"

        Console.WriteLine($"Slave ID is {slaveId}");
        Console.WriteLine($"WebSocket Server is {wsServerAddr}");

        webSocket = new ClientWebSocket();
    }

    public async Task ReceiveMessages()
    {
        if (webSocket == null)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        Console.WriteLine("Message is received.");

        byte[] buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None
            );

            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            MessageReceived?.Invoke(message);
        }
    }

    public async Task ConnectAsync()
    {
        if (webSocket == null)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        try
        {
            Console.WriteLine($"{wsServerAddr}?slaveId={slaveId}");

            await webSocket.ConnectAsync(
                new Uri($"{wsServerAddr}?slaveId={slaveId}"),
                CancellationToken.None
            );
            OnClientConnected?.Invoke();
        }
        catch (WebSocketException se)
        {
            Console.WriteLine($"WebSocket Service had the following error: {se.Message}");
            OnClientDisconnected?.Invoke();
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine(
                $"WebSocket Service had the following non-socket-related error: {e.Message}"
            );

            OnClientDisconnected?.Invoke();
            throw;
        }
    }

    public async Task SendMessageAsync(string message)
    {
        if (webSocket == null)
        {
            throw new InvalidOperationException("WebSocket is not connected.");
        }

        Console.WriteLine($"Sending message with content: ${message}");
        var messageBytes = Encoding.UTF8.GetBytes(message);
        await webSocket.SendAsync(
            new ArraySegment<byte>(messageBytes),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
    }
}
