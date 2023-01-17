namespace SimpleBacktestLib;

/// <summary>
/// A request to execute a trade.
/// </summary>
public abstract class TradeRequest
{
    /// <summary>
    /// Description of the actions taken, if any.
    /// Null should be expected only if the trade request has not been attempted yet
    /// </summary>
    public string LogEntry { get; protected set; }

    /// <summary>
    /// Defines if the trade was able to execute.
    /// Conditions such as insufficient funds could leave this value at 'false' after the trade is attempted.
    /// </summary>
    public TradeStatus Status { get; protected set; } = TradeStatus.Pending;

    /// <summary>
    /// Executing the trade request happens internally and should not be called by the user.
    /// This method contains the logic to simulate the trade for the backtest.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    internal abstract void Execute(int candleInde, BacktestState state);
}
