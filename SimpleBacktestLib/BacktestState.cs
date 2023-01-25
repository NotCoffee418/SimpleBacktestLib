namespace SimpleBacktestLib;

/// <summary>
/// State at a given point in the backtest. Modified by the <see cref="Engine"/> class.
/// </summary>
public class BacktestState
{
    internal BacktestState(SetupDefinitions setupDefs)
    {
        SetupConfig = setupDefs;
        Trade = new(this);
        BaseBalance = setupDefs.StartingBaseBalance;
        QuoteBalance = setupDefs.StartingQuoteBalance;
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
    /// Index of the currently evaluated candle in CandleData
    /// Updated by the Engine class.
    /// </summary>
    public int CurrentCandleIndex { get; internal set; } = -1;

    /// <summary>
    /// Allows execution of trades duing the backtest in OnTick()
    /// </summary>
    public CommonTradeManager Trade { get; }

    /// <summary>
    /// Get all margin trades opened during the backtest so far.
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, MarginPosition> GetAllMarginTrades()
        => MarginTrades.ToDictionary(x => x.Key, x => x.Value); // Cloned version

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
    /// Get the price of the current candle as defined by the settings.
    /// </summary>
    /// <returns></returns>
    public decimal GetCurrentCandlePrice()
        => GetCurrentCandle().GetPrice(SetupConfig.CandlePriceTime);

    /// <summary>
    /// Add a log entry and trigger OnLogEntry.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="level"></param>
    public void AddLogEntry(string message, LogLevel level = LogLevel.Information)
        => LogHandler.AddLogEntry(this, message, CurrentCandleIndex, level);

    /// <summary>
    /// Get the candle currently being evaluated.
    /// </summary>
    /// <returns></returns>
    public BacktestCandle GetCurrentCandle()
        => SetupConfig.CandleData[CurrentCandleIndex];
    
    /// <summary>
    /// Get all backtest candles input to the backtest.
    /// </summary>
    /// <returns></returns>
    public ImmutableList<BacktestCandle> GetAllBacktestCandles()
        => SetupConfig.CandleData;

    /// <summary>
    /// Internal-only properties of the backtest state.
    /// </summary>
    internal SetupDefinitions SetupConfig { get; } = new();


    /// <summary>
    /// History of spot trades so far
    /// Use access function instead for a cloned version during backtest.
    /// </summary>
    internal List<BacktestTrade> SpotTrades { get; } = new();

    /// <summary>
    /// Used to generate unique IDs for margin trades.
    /// </summary>
    internal int NextMarginId { get; set; } = 1;

    /// <summary>
    /// History of margin trades so far and open positions
    /// Use access function instead for a cloned version during backtest.
    /// </summary>
    internal Dictionary<int, MarginPosition> MarginTrades { get; } = new();

    /// <summary>
    /// In-progress log. Should be modified through LogHandler and it's shortcuts only.
    /// Use access function instead for a cloned version during backtest.
    /// </summary>
    internal List<LogEntry> LogEntries { get; } = new();
}
