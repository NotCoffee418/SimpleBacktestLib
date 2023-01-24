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
        => PendingResult.MarginTrades.ToList(); // Cloned version

    /// <summary>
    /// Get all spot trades executed during the backtest so far.
    /// </summary>
    /// <returns></returns>
    public List<BacktestTrade> GetAllSpotTrades()
        => PendingResult.SpotTrades.ToList(); // Cloned version

    

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
    /// Incomplete backtest result. Should not be revealed to the user until completion aince information is missing.
    /// </summary>
    internal BacktestResult PendingResult { get; } = new();

}
