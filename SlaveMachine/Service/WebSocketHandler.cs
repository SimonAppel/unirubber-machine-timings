using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SlaveMachine.ViewModels.Idle;
using Tmds.DBus.Protocol;

public class WebSocketHandler
{
    public WebSocketService wsService;
    private CancellationTokenSource cts;
    private CancellationToken token;

    public int maxRetry;

    public event Action? OnClientEndRetry;

    public WebSocketHandler()
    {
        this.wsService = new WebSocketService();
        this.maxRetry = 3;
    }

    // Error handling should be here. The WS class should just manage what errors are what to throw, there is the logic
    private void GenerateToken()
    {
        cts = new CancellationTokenSource();
        token = cts.Token;
    }

    public async Task ReceiveMessages()
    {
        await wsService.ReceiveMessages();
    }

    public async Task SendMessageAsync(string message)
    {
        try
        {
            await wsService.SendMessageAsync(message);
        }
        catch (Exception)
        {
            if (message.Equals("HB"))
            {
                Console.WriteLine("Heartbeat stop beating");
                cts.Cancel();
                await ConnectAsync();
            }
            else
            {
                throw new ConnectException("Socket had an issue sending the message");
            }
        }
    }

    public async Task ConnectAsync()
    {
        if (wsService.slaveId == null)
        {
            Console.WriteLine("UNIRUBBER_MACHINE_ID cannot be null");
            throw new Exception("UNIRUBBER_MACHINE_ID cannot be null");
        }

        int retry = 0;
        while (retry < maxRetry && wsService.webSocket.State != WebSocketState.Open)
        {
            if (
                wsService.webSocket.State == WebSocketState.Aborted
                || wsService.webSocket.State == WebSocketState.Closed
            )
            {
                wsService.webSocket.Dispose();
                wsService.webSocket = new ClientWebSocket();
            }

            try
            {
                Console.WriteLine($"Connecting attempt Nº{retry}");
                await wsService.ConnectAsync(retry);
            }
            catch (WebSocketException)
            {
                retry++;

                Console.WriteLine($"Trying to reconnect at {DateTime.Now}");
                await Task.Delay(10000);
            }
            catch (System.Exception e)
            {
                // General Error
                Console.WriteLine($"WS State {wsService.webSocket.State}");
                Console.WriteLine($"Other Error: {e.Message}");
                throw;
            }
        }

        Console.WriteLine(wsService.webSocket.State);

        if (wsService.webSocket.State != WebSocketState.Open)
        {
            Console.WriteLine("Client cannot find the server");
            OnClientEndRetry?.Invoke();

            throw new Exception("Client cannot find the server");
        }

        GenerateToken();
        await Task.WhenAll(Task.Run(DoHeartbeat, token), Task.Run(ReceiveMessages, token));
    }

    public async Task DoHeartbeat()
    {
        try
        {
            string heartbeatString = "HB";

            while (!token.IsCancellationRequested)
            {
                await SendMessageAsync(heartbeatString);
                await Task.Delay(10000, token); // 10 secs o cada minuto?
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error {e.Message}");
            Console.WriteLine($"Error Type {e.GetType()}");
            throw;
        }
    }
}
