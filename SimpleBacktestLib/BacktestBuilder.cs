namespace SimpleBacktestLib;

/// <summary>
/// Primary access point to the SimpleBacktestLib's core functions.
/// </summary>
public class BacktestBuilder
{
    private SetupDefinitions BacktestSetup { get; } = new();

    /// <summary>
    /// Create a backtest builder with default settings
    /// </summary>
    /// <returns></returns>
    public static BacktestBuilder CreateBuilder() => new();

    public BacktestBuilder WithCandleData(IEnumerable<BacktestCandle> candleData)
    {
        // Validate dataset
        if (BacktestSetup.CandleData is not null)
            throw new ArgumentException("Candle data has already been set.");
        if (candleData is null || candleData.Count() == 0)
            throw new ArgumentException("Input candle data is null or empty.");
        
        // Set data
        BacktestSetup.CandleData = candleData;
        return this;
    }

    /// <summary>
    /// Specify a function that will run each candle and returns any trades to be executed if possible.
    /// Multiple OnTick definitions are allowed and will be called in order of definition.
    /// </summary>
    /// <param name="tickFunction"></param>
    /// <returns></returns>
    public BacktestBuilder OnTick(Func<TickData, IEnumerable<TradeRequest>?> onTickFunction)
    {
        BacktestSetup.OnTickFunctions.Add(onTickFunction);
        return this;
    }

    /// <summary>
    /// Read-only access to the tick data and any trade requests that were executed this tick.
    /// This is primarily intended for progress reporting, logging and evaluation.
    /// </summary>
    /// <param name="postTickFunction"></param>
    /// <returns></returns>
    public BacktestBuilder PostTick(Action<(TickData TickData, IEnumerable<TradeRequest> ExecutedTrades)> postTickFunction)
    {
        BacktestSetup.PostTickFunctions.Add(postTickFunction);
        return this;
    }

    /// <summary>
    /// Run the backtest asynchronously.
    /// </summary>
    /// <returns></returns>
    public async Task<BacktestResult> RunAsync()
    {
        // Validate setup
        // todo... ensure all required properties are set
        throw new NotImplementedException();

        // Run the backtest
        return await Engine.RunBacktestAsync(BacktestSetup);
    }

    /// <summary>
    /// Run the backtest synchronously.
    /// </summary>
    /// <returns></returns>
    public BacktestResult Run()
        => RunAsync().GetAwaiter().GetResult();

}
