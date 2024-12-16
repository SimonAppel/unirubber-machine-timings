using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace SlaveMachine.ViewModels.Busy;

public partial class BusyViewModel : ReactiveObject
{
    private StopwatchService initWorkWatch;
    private StopwatchService pieceWatch;

    private string diffInitWorkTime;
    private string diffPieceTime;

    public string DiffInitWorkTime
    {
        get => diffInitWorkTime;
        set => this.RaiseAndSetIfChanged(ref diffInitWorkTime, value);
    }
    public string DiffPieceTime
    {
        get => diffPieceTime;
        set => this.RaiseAndSetIfChanged(ref diffPieceTime, value);
    }

    public BusyViewModel()
    {
        diffInitWorkTime = "00:00:00";
        diffPieceTime = "00:00:00";

        initWorkWatch = new StopwatchService();
        pieceWatch = new StopwatchService();

        initWorkWatch.OnClockTick += OnInitWorkChange;
        pieceWatch.OnClockTick += OnPieceWorkChange;

        initWorkWatch.StartStop();
        pieceWatch.StartStop();
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
}
