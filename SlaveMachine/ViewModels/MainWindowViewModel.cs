using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace SlaveMachine.ViewModels;

enum SLAVE_STATE {
    IDLE = 0,
    BUSY = 1,
    ENDED = 3,
    RECOVER = 4
}

public partial class MainWindowViewModel : ReactiveObject
{
    private readonly WebSocketService webSocketService;
    private int pieceCount = -1;
    private SLAVE_STATE machineState = SLAVE_STATE.IDLE;

    public int PieceCount
    {
        get => pieceCount;
        set => this.RaiseAndSetIfChanged(ref pieceCount, value);
    }

    public ReactiveCommand<Unit, Unit> ButtonClickCommand { get; }

    public MainWindowViewModel()
    {
        webSocketService = new WebSocketService("1");
        webSocketService.MessageReceived += OnMessageReceived;
        _ = webSocketService.ConnectAsync("ws://127.0.0.1:8181");

        ButtonClickCommand = ReactiveCommand.Create(() =>
        {
            Console.WriteLine("Button clicked!");
        });
    }

    // Read docs about this v.
    private void OnMessageReceived(string message)
    {
        Console.WriteLine($"Recieved: {message}");
        string[] splitMessage = message.Split("-");

        switch (splitMessage[0])
        {
            case "INIT":
                if(PieceCount == -1){
                    PieceCount = Int32.Parse(splitMessage[1]);
                }
                Console.WriteLine($"Slave should initiate with {splitMessage[1]} pieces");
            break;

            default:
                return;
        }

    }


    public void TestCommand(){
        Console.WriteLine("TEST COMMAND.");
    }
}
