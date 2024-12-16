using System;
using System.Timers;

public class StopwatchService
{
    private readonly Timer timer;
    private TimeSpan elapsedTime;
    private string formattedTime;
    public bool IsRunning { get; private set; }

    public event Action<TimeSpan>? OnClockTick;

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
        OnClockTick?.Invoke(elapsedTime);
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
        IsRunning = false;
    }
}
