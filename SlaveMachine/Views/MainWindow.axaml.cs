using System;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Threading;

namespace SlaveMachine.Views;

public partial class MainWindow : Window
{
    private readonly Timer timer;

    public MainWindow()
    {
        InitializeComponent();

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