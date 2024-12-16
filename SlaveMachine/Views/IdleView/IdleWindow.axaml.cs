using System;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Threading;
using SlaveMachine.ViewModels.Idle;

namespace SlaveMachine.Views.IdleView;

public partial class IdleWindow : UserControl
{
    private IdleViewModel viewModel;

    public IdleWindow(WebSocketService socket)
    {
        InitializeComponent();

        viewModel = new IdleViewModel(socket);
        DataContext = viewModel;
    }
}