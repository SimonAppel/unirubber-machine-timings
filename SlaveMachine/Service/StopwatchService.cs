using System;
using System.ComponentModel;
using System.Timers;

public class StopwatchService : INotifyPropertyChanged
{
    private readonly Timer timer;
    private TimeSpan elapsedTime;
    private string formattedTime;

    public string FormattedTime
    {
        get => formattedTime;
        private set
        {
            formattedTime = value;
            OnPropertyChanged(nameof(FormattedTime));
        }
    }

    public TimeSpan ElapsedTime {
        get => elapsedTime;
    }

    public bool IsRunning { get; private set; }

    public StopwatchService()
    {
        elapsedTime = TimeSpan.Zero;
        formattedTime = "00:00:00";
        timer = new Timer(1000);
        timer.Elapsed += OnTimerTick;
    }

    private void OnTimerTick(object sender, ElapsedEventArgs e)
    {
        elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
        FormattedTime = elapsedTime.ToString(@"hh\:mm\:ss");
    }

    public void StartStop()
    {
        if (IsRunning)
        {
            timer.Stop();
        }
        else
        {
            timer.Start();
        }

        IsRunning = !IsRunning;
    }

    public void Reset()
    {
        timer.Stop();
        elapsedTime = TimeSpan.Zero;
        FormattedTime = "00:00:00";
        IsRunning = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
