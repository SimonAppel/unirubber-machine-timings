using ReactiveUI;
using System;

namespace MasterMachine.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    public string Greeting { get; } = "My Test!";
    private readonly WebSocketServerService webSocketService;
    private string _receivedMessage = string.Empty;
    
    public string ReceivedMessage
    {
        get => _receivedMessage;
        set => this.RaiseAndSetIfChanged(ref _receivedMessage, value);
    }

    public MainWindowViewModel()
    {
        webSocketService.OnMessageReceived += OnMessageReceived;
        webSocketService = new WebSocketServerService();
        webSocketService.Start();
    }

    private void OnMessageReceived(string id, string message)
    {
        Console.WriteLine($"Message Received from Slave {id} with content {message}");
        ReceivedMessage = message; // Update UI with the received message - <TextBlock Text="{Binding ReceivedMessage}" />
    }
}
