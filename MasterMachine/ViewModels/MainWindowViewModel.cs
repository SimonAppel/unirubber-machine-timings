using ReactiveUI;
using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Tmds.DBus.Protocol;
using System.Collections.Generic;

namespace MasterMachine.ViewModels;

enum SLAVE_STATE {
    IDLE = 0,
    BUSY = 1,
    ENDED = 3,
    RECOVER = 4
}

public partial class MainWindowViewModel : ReactiveObject
{
    private readonly WebSocketServerService webSocketService;
    private readonly WebSocketHandler socketHandler;
    private string receivedMessage = string.Empty;

    public ReactiveCommand<Unit, Unit> SendCommand { get; }

    public string ReceivedMessage
    {
        get => receivedMessage;
        set => this.RaiseAndSetIfChanged(ref receivedMessage, value);
    }

    public Dictionary<string, string> Messages { get; set; }

    public ReactiveCommand<string, Unit> SendMessage { get; }

    private SLAVE_STATE machineState = SLAVE_STATE.IDLE;

    public MainWindowViewModel()
    {
        Messages = [];

        webSocketService = new WebSocketServerService();
        socketHandler = new WebSocketHandler(webSocketService);

        webSocketService.OnMessageReceived += OnMessage;
        webSocketService.Start();

        SendMessage = ReactiveCommand.CreateFromTask(async (string id) =>
        {
            if(socketHandler.DoesClientExist(id) && Messages.TryGetValue(id, out var message)){
                Console.WriteLine($"Message will be sent to Slave {id} with message {message}");
                await socketHandler.InitSlaveById(id, message);
            }
        });
    }

    private void OnMessage(string id, string message)
    {
        Console.WriteLine($"Message Received from Slave {id} with content {message}");
        ReceivedMessage = message; // Update UI with the received message - <TextBlock Text="{Binding ReceivedMessage}" />
    }
}
