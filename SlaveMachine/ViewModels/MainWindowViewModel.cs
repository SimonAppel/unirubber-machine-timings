using System;
using System.Collections.Generic;
using System.Net.WebSockets;
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
    RECOVER = 4,
}

public partial class MainWindowViewModel : ReactiveObject
{
    private readonly WebSocketHandler webSocket;
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
    public PopupHandler ModalHandler { get; }

    public UserControl CurrentView
    {
        get => currentView;
        set => this.RaiseAndSetIfChanged(ref currentView, value);
    }

    public MainWindowViewModel()
    {
        webSocket = new WebSocketHandler();
        ModalHandler = new PopupHandler(webSocket.wsService);

        // Passing down these objects it's an anti-pattern. Should just do a Dependency Injection with it.
        // It's basically the same as this, but as a shared object for holding many data in case of anything.
        // Read DI or Event Aggregator or Mediator Service or Shared View Model
        idleWindow = new(webSocket);
        busyWindow = new();

        currentView = idleWindow;
        _ = webSocket.ConnectAsync();

        webSocket.wsService.MessageReceived += OnMessageReceived;
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

                busyWindow.SetPiecesAmount(PieceCount);
                CurrentView = busyWindow;
                busyWindow.StartTimers();

                break;

            default:
                return;
        }
    }
}
