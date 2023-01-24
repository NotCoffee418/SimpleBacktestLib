namespace SimpleBacktestLib;

/// <summary>
/// State at a given point in the backtest. Modified by the <see cref="Engine"/> class.
/// </summary>
public class BacktestState
{
    public BacktestState()
    {
        TradeManagerInstance = new(this);
    }
    
    /// <summary>
    /// Current balance of base asset, excluding margin positions.
    /// </summary>
    public decimal BaseBalance { get; internal set; }

    /// <summary>
    /// Current balance of quote asset, excluding margin positions.
    /// </summary>
    public decimal QuoteBalance { get; internal set; }


    /// <summary>
    /// Get all margin trades opened during the backtest so far.
    /// </summary>
    /// <returns></returns>
    public List<MarginPosition> GetAllMarginTrades()
        => MarginTrades.ToList(); // Cloned version

    /// <summary>
    /// Get all spot trades executed during the backtest so far.
    /// </summary>
    /// <returns></returns>
    public List<BacktestTrade> GetAllSpotTrades()
        => SpotTrades.ToList(); // Cloned version

    /// <summary>
    /// Get the full log at the current point in the backtest.
    /// The preferred way to get logs in progress is through BacktestBuilder.OnLogEntry().
    /// </summary>
    /// <returns></returns>
    public List<LogEntry> GetFullLog()
        => LogEntries.ToList(); // Cloned Version

    /// <summary>
    /// Internal-only properties of the backtest state.
    /// </summary>
    internal SetupDefinitions SetupConfig { get; } = new();

    /// <summary>
    /// Internal reference of instance of the trade manager, to be passed by engine to TickData as "Trade".
    /// Instantiated by constructor.
    /// </summary>
    internal CommonTradeManager TradeManagerInstance { get; }

    /// <summary>
    /// History of spot trades so far
    /// </summary>
    internal List<BacktestTrade> SpotTrades { get; } = new();

    /// <summary>
    /// History of margin trades so far and open positions
    /// </summary>
    internal List<MarginPosition> MarginTrades { get; } = new();

    /// <summary>
    /// In-progress log. Should be modified through LogHandler and it's shortcuts only.
    /// </summary>
    internal List<LogEntry> LogEntries { get; } = new();
}
