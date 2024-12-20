using Avalonia.Controls;
using SlaveMachine.ViewModels.Idle;

namespace SlaveMachine.Views.IdleView;

public partial class IdleWindow : UserControl
{
    private IdleViewModel viewModel;

    public IdleWindow(WebSocketHandler socketHandler)
    {
        InitializeComponent();

        viewModel = new IdleViewModel(socketHandler);
        DataContext = viewModel;
    }
}
