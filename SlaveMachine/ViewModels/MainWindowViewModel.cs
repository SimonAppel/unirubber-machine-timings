using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace SlaveMachine.ViewModels;

public partial class MainWindowViewModel : ReactiveObject
{
    private readonly WebSocketService webSocketService;
    private StopwatchService workClock;
    private StopwatchService pieceClock;

    private string receivedMessage = string.Empty;

    public ReactiveCommand<string, Unit> SendMessageCommand { get; }
    public ReactiveCommand<Unit, Unit> DisconnectCommand { get; }
    
    public string Greeting { get; } = "My Test!";
    public string ReceivedMessage
    {
        get => receivedMessage;
        set => this.RaiseAndSetIfChanged(ref receivedMessage, value);
    }

    public MainWindowViewModel()
    {
        webSocketService = new WebSocketService("1");
        workClock = new StopwatchService();
        pieceClock = new StopwatchService();

        SendMessageCommand = ReactiveCommand.CreateFromTask<string>(webSocketService.SendMessageAsync);
        DisconnectCommand = ReactiveCommand.CreateFromTask(webSocketService.DisconnectAsync);

        _ = webSocketService.ConnectAsync("ws://127.0.0.1:8181");
    }
}
