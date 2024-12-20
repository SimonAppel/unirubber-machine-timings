using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Tmds.DBus.Protocol;

namespace MasterMachine.ViewModels;

enum SLAVE_STATE
{
    IDLE = 0,
    BUSY = 1,
    ENDED = 3,
    RECOVER = 4,
}

public partial class MainWindowViewModel : ReactiveObject
{
    private readonly WebSocketServerService webSocketService;
    private readonly WebSocketHandler socketHandler;
    private string receivedMessage = string.Empty;

    public string ReceivedMessage
    {
        get => receivedMessage;
        set => this.RaiseAndSetIfChanged(ref receivedMessage, value);
    }

    public Dictionary<string, string> Messages { get; set; }

    public ReactiveCommand<string, Unit> SendMessage { get; }
    public PopupHandler ModalHandler { get; }

    private SLAVE_STATE machineState = SLAVE_STATE.IDLE;

    public MainWindowViewModel()
    {
        Messages = [];

        webSocketService = new WebSocketServerService();
        socketHandler = new WebSocketHandler(webSocketService);
        ModalHandler = new PopupHandler(webSocketService);

        webSocketService.Start();
        webSocketService.OnMessageReceived += OnMessage;

        SendMessage = ReactiveCommand.CreateFromTask(
            async (string id) =>
            {
                if (socketHandler.DoesClientExist(id) && Messages.TryGetValue(id, out var message))
                {
                    Console.WriteLine($"Message will be sent to Slave {id} with message {message}");
                    await socketHandler.InitSlaveById(id, message);
                }
            }
        );
    }

    private void OnMessage(string id, string message)
    {
        Console.WriteLine($"Message Received from Slave {id} with content {message}");
        ReceivedMessage = message; // Update UI with the received message - <TextBlock Text="{Binding ReceivedMessage}" />
    }
}
