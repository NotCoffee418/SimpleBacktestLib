namespace SimpleBacktestLib.TradingManagers;

public class MarginManager
{
    /// <summary>
    /// Should not be instantiated externally.
    /// These functions can be called through Tick()'s x.Trade.Margin
    /// </summary>
    internal MarginManager(BacktestState state)
    {
        State = state;
    }

    /// <summary>
    /// Reference to the instance state for reading and modifying.
    /// </summary>
    public BacktestState State { get; }
}
