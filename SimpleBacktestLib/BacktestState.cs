using SimpleBacktestLib.Internal.Helpers;

namespace SimpleBacktestLib;

/// <summary>
/// State at a given point in the backtest. Modified by the <see cref="Engine"/> class.
/// </summary>
public class BacktestState
{
    public BacktestState()
    {
        Trade = new(this);
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
    /// Internal reference of instance of the trade manager, to be passed by engine to TickData as "Trade".
    /// Instantiated by constructor.
    /// </summary>
    public CommonTradeManager Trade { get; }

    /// <summary>
    /// Get all margin trades opened during the backtest so far.
    /// </summary>
    /// <returns></returns>
    public Dictionary<uint, MarginPosition> GetAllMarginTrades()
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
    /// </summary>
    internal List<BacktestTrade> SpotTrades { get; } = new();

    /// <summary>
    /// Used to generate unique IDs for margin trades.
    /// </summary>
    internal uint NextMarginId { get; set; } = 1;
    
    /// <summary>
    /// History of margin trades so far and open positions
    /// </summary>
    internal Dictionary<uint, MarginPosition> MarginTrades { get; } = new();

    /// <summary>
    /// In-progress log. Should be modified through LogHandler and it's shortcuts only.
    /// </summary>
    internal List<LogEntry> LogEntries { get; } = new();
}
