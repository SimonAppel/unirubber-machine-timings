using System;
using System.ComponentModel;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

// Error handling in web sockets...

public class PopupHandler : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private CancellationTokenSource cts;
    private CancellationToken token;
    private bool isPopupVisible;
    private string titleText;
    private string contentText;
    private string staticTitle = "Servidor";
    public bool IsPopupVisible
    {
        get => isPopupVisible;
        set
        {
            isPopupVisible = value;
            OnPropertyChanged();
        }
    }

    public string TitleText
    {
        get => titleText;
        set
        {
            titleText = value;
            OnPropertyChanged();
        }
    }

    public string ContentText
    {
        get => contentText;
        set
        {
            contentText = value;
            OnPropertyChanged();
        }
    }

    public PopupHandler(WebSocketService wsService)
    {
        isPopupVisible = false;

        cts = new CancellationTokenSource();
        token = cts.Token;

        wsService.OnClientConnected += OnSlaveConnect;
        wsService.OnClientDisconnected += OnSlaveDisconnect;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RegenerateToken()
    {
        cts.Cancel();
        cts = new CancellationTokenSource();
        token = cts.Token;
    }

    private async Task ChangePopupState(string title, string content)
    {
        if (IsPopupVisible == true)
        {
            IsPopupVisible = false;
            RegenerateToken();
        }

        TitleText = title;
        ContentText = content;

        IsPopupVisible = true;
        await Task.Delay(2000, token);
        IsPopupVisible = false;
    }

    private async void OnSlaveConnect()
    {
        await ChangePopupState($"{staticTitle} Encontrado", "Preparado para recibir OC");
    }

    private async void OnSlaveDisconnect(int retry)
    {
        await ChangePopupState(
            $"{staticTitle} Extraviado",
            $"Reconectando... {3 - retry} intentos restantes"
        );
    }
}
