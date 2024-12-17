using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fleck;

public class WebSocketServerService
{
    private readonly WebSocketServer server;
    public readonly Dictionary<string, IWebSocketConnection> clients;

    public WebSocketServerService()
    {
        Console.WriteLine("Opening WebSocket server.");
        server = new WebSocketServer("ws://127.0.0.1:8181");
        clients = new Dictionary<string, IWebSocketConnection>();
    }

    private string GetSlaveIdFromUri(string rawUri)
    {
        var query = rawUri.Split('?');
        if (query.Length < 1)
            throw new InvalidOperationException("Client does not provide arguments");

        var parameters = System.Web.HttpUtility.ParseQueryString(query[1]);
        if (parameters["slaveId"] == null)
            throw new InvalidOperationException("Client does not provide ID");

        return parameters["slaveId"];
    }

    private void OnSocketOpen(IWebSocketConnection socket, string id)
    {
        Console.WriteLine("A new client has connected.");
        string slaveId = GetSlaveIdFromUri(socket.ConnectionInfo.Path);
        Console.WriteLine($"The slave Nº {slaveId} has connected.");

        clients[slaveId] = socket;
        OnClientConnected?.Invoke(slaveId);
    }

    private void OnSocketClose(IWebSocketConnection socket, string id)
    {
        if (!clients.ContainsKey(id))
        {
            return;
        }

        clients.Remove(id);
        Console.WriteLine($"Client disconnected: {id}");
        OnClientDisconnected?.Invoke(id);
    }

    private void OnSocketMessage(IWebSocketConnection socket, string message, string id)
    {
        Console.WriteLine($"Message from {id}: {message}");
        OnMessageReceived?.Invoke(id, message);
    }

    public void Start()
    {
        server.Start(socket =>
        {
            string globalId = GetSlaveIdFromUri(socket.ConnectionInfo.Path);
            Console.WriteLine(globalId);

            socket.OnOpen = () => OnSocketOpen(socket, globalId);
            socket.OnClose = () => OnSocketClose(socket, globalId);
            socket.OnMessage = message => OnSocketMessage(socket, message, globalId);
        });

        Console.WriteLine("WebSocket server started.");
    }

    public void Stop()
    {
        foreach (var client in clients.Values)
        {
            client.Close();
        }
        clients.Clear();
        Console.WriteLine("WebSocket server stopped.");
    }

    public async Task SendMessageAsync(string slaveId, string message)
    {
        try
        {
            if (clients.TryGetValue(slaveId, out var socket))
            {
                await socket.Send(message);
                Console.WriteLine($"Message sent to {slaveId}: {message}");
            }
            else
            {
                Console.WriteLine($"Client {slaveId} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    public event Action<string>? OnClientConnected;

    public event Action<string>? OnClientDisconnected;

    public event Action<string, string>? OnMessageReceived;
}
