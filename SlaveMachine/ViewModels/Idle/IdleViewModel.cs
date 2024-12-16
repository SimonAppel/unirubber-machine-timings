using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace SlaveMachine.ViewModels.Idle;

public partial class IdleViewModel : ReactiveObject
{

    private bool isSlaveConnected;
    public bool IsSlaveConnected
    {
        get => isSlaveConnected;
        set => this.RaiseAndSetIfChanged(ref isSlaveConnected, value);
    }

    public IdleViewModel(WebSocketService socket)
    {
        Console.WriteLine("Idle View just started");

        socket.OnClientConnected += OnSlaveConnection;
        socket.OnClientDisconnected += OnSlaveDisconnection;
    }

    private void OnSlaveConnection()
    {
        Console.WriteLine("Slave just connected");
        IsSlaveConnected = true;
    }
    private void OnSlaveDisconnection()
    {
        Console.WriteLine("Slave just disconnected");
        IsSlaveConnected = false;
    }
}
