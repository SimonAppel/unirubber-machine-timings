using System;
using System.Diagnostics;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Threading;
using SlaveMachine.ViewModels.Busy;

namespace SlaveMachine.Views.BusyView;

public partial class BusyWindow : UserControl
{
    private readonly Timer timer;
    private BusyViewModel viewModel;

    public BusyWindow()
    {
        InitializeComponent();

        viewModel = new BusyViewModel();
        DataContext = viewModel;

        timer = new Timer(1000);
        timer.Elapsed += UpdateClock;
        timer.Start();
    }

    private void UpdateClock(object? sender, ElapsedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ClockTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
        });
    }
}