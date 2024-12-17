using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace SlaveMachine.ViewModels.Busy;

public partial class BusyViewModel : ReactiveObject
{
    // El colocar {get; set;} permite su uso en bindings en UI
    public StopwatchHandler SwHandler { get; }
    public ReactiveCommand<Unit, Unit> NewTimer { get; }

    private int remainingPieces;
    public string testing;

    public int RemainingPieces
    {
        get => remainingPieces;
        set => this.RaiseAndSetIfChanged(ref remainingPieces, value);
    }

    public BusyViewModel()
    {
        remainingPieces = 0;
        SwHandler = new StopwatchHandler();

        NewTimer = ReactiveCommand.Create(NewPieceTimer);
    }

    private void NewPieceTimer()
    {
        SwHandler.NewPieceTimer();
        if (RemainingPieces > 0)
        {
            RemainingPieces--;
        }
    }

    public void SetPiecesAmount(int pieces)
    {
        RemainingPieces = pieces;
    }
}
