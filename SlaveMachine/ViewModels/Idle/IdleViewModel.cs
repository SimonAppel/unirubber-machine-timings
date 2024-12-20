using System;
using ReactiveUI;

namespace SlaveMachine.ViewModels.Idle;

public partial class IdleViewModel : ReactiveObject
{
    private string slaveConnectionState;
    public string SlaveConnectionState
    {
        get => slaveConnectionState;
        set => this.RaiseAndSetIfChanged(ref slaveConnectionState, value);
    }

    public IdleViewModel(WebSocketHandler socketHandler)
    {
        slaveConnectionState = "RETRYING";

        socketHandler.wsService.OnClientConnected += OnSlaveConnection;
        socketHandler.wsService.OnClientDisconnected += OnSlaveRetry;
        socketHandler.OnClientEndRetry += OnSlaveDisconnection;
    }

    private void OnSlaveConnection()
    {
        Console.WriteLine("Slave just connected");
        SlaveConnectionState = "CONNECTED";
    }

    private void OnSlaveRetry(int retry)
    {
        Console.WriteLine("Slave trying to reconnect");
        SlaveConnectionState = "RETRYING";
    }

    private void OnSlaveDisconnection()
    {
        Console.WriteLine("Slave just disconnected");
        SlaveConnectionState = "DISCONNECTED";
    }
}
