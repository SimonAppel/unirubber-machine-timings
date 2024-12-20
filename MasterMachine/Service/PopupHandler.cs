using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public class PopupHandler : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private CancellationTokenSource cts;
    private CancellationToken token;
    private bool isPopupVisible;
    private string titleText;
    private string contentText;

    private string staticTitle = "Controlador";
    private string staticContent = "La prensa";
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

    public PopupHandler(WebSocketServerService wsService)
    {
        isPopupVisible = false;
        titleText = "";
        contentText = "";

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
        await Task.Delay(3000, token);
        IsPopupVisible = false;
    }

    private async void OnSlaveConnect(string id)
    {
        await ChangePopupState(
            $"{staticTitle} Encontrado",
            $"{staticContent} {id} se ha conectado"
        );
    }

    private async void OnSlaveDisconnect(string id)
    {
        await ChangePopupState(
            $"{staticTitle} Extraviado",
            $"{staticContent} {id} se ha desconectado"
        );
    }
}
