using System;
using System.Timers;

public class StopwatchService
{
    private readonly Timer timer;
    public TimeSpan elapsedTime;
    public bool IsRunning { get; private set; }

    public event Action<TimeSpan>? OnClockTick;

    public StopwatchService()
    {
        elapsedTime = TimeSpan.Zero;
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
