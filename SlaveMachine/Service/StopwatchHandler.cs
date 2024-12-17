using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class StopwatchHandler : INotifyPropertyChanged
{
    private StopwatchService initWorkWatch;
    private StopwatchService pieceWatch;
    private string diffInitWorkTime;
    private string diffPieceTime;

    public List<double> timePerPiece;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string DiffInitWorkTime
    {
        get => diffInitWorkTime;
        set
        {
            if (diffInitWorkTime != value)
            {
                diffInitWorkTime = value;
                OnPropertyChanged();
            }
        }
    }

    public string DiffPieceTime
    {
        get => diffPieceTime;
        set
        {
            if (diffPieceTime != value)
            {
                diffPieceTime = value;
                OnPropertyChanged();
            }
        }
    }

    public StopwatchHandler()
    {
        timePerPiece = [];
        diffInitWorkTime = "00:00:00";
        diffPieceTime = "00:00:00";

        initWorkWatch = new StopwatchService();
        pieceWatch = new StopwatchService();

        initWorkWatch.OnClockTick += OnInitWorkChange;
        pieceWatch.OnClockTick += OnPieceWorkChange;
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnInitWorkChange(TimeSpan ts)
    {
        DiffInitWorkTime = ts.ToString(@"hh\:mm\:ss");
    }

    private void OnPieceWorkChange(TimeSpan ts)
    {
        DiffPieceTime = ts.ToString(@"hh\:mm\:ss");
    }

    public void StartWork()
    {
        initWorkWatch.StartStop();
        pieceWatch.StartStop();
    }

    public void NewPieceTimer()
    {
        timePerPiece.Add(pieceWatch.elapsedTime.TotalSeconds);
        DiffPieceTime = "00:00:00";
        pieceWatch.Reset();
        pieceWatch.StartStop();
    }
}
