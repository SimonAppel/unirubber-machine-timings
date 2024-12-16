using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using SlaveMachine.Views.BusyView;
using SlaveMachine.Views.IdleView;

namespace SlaveMachine.ViewModels;

enum SLAVE_STATE
{
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

    public UserControl currentView;
    public BusyWindow busyWindow;
    public IdleWindow idleWindow;

    public UserControl CurrentView
    {
        get => currentView;
        set => this.RaiseAndSetIfChanged(ref currentView, value);
    }

    public MainWindowViewModel()
    {
        webSocketService = new WebSocketService("1");

        // Passing down these objects it's an anti-pattern. Should just do a Dependency Injection with it.
        // It's basically the same as this, but as a shared object for holding many data in case of anything.
        // Read DI or Event Aggregator or Mediator Service or Shared View Model
        idleWindow = new(webSocketService);
        busyWindow = new();

        currentView = busyWindow;
        //currentView = idleWindow;

        _ = webSocketService.ConnectAsync("ws://127.0.0.1:8181");
        webSocketService.MessageReceived += OnMessageReceived;
    }

    // Read docs about this v.
    private void OnMessageReceived(string message)
    {
        Console.WriteLine($"Recieved: {message}");
        string[] splitMessage = message.Split("-");

        switch (splitMessage[0])
        {
            case "INIT":
                if (PieceCount == -1)
                {
                    PieceCount = Int32.Parse(splitMessage[1]);
                }
                Console.WriteLine($"Slave should initiate with {splitMessage[1]} pieces");
                CurrentView = busyWindow;
                break;

            default:
                return;
        }

    }

    public void TestCommand()
    {
        Console.WriteLine("TEST COMMAND.");
    }
}
