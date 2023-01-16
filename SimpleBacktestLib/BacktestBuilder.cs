using System.Globalization;

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

        // Validate dataset is in chronological order
        var previousCandle = candleData.First();
        if (candleData.Count() > 1)
            foreach (var candle in candleData.Skip(1))
            {
                if (candle.Time < previousCandle.Time)
                    throw new ArgumentException("Candle data is not in chronological order.");
                previousCandle = candle;
            }

        // Set data
        BacktestSetup.CandleData = candleData.ToImmutableList();

        // Set default evaluate time at the last month of data.
        DateTime endEvaluateTime = candleData.Last().Time;
        DateTime startEvaluateTime = endEvaluateTime.AddDays(-30);
        return this.EvaluateBetween(startEvaluateTime, endEvaluateTime);
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
    /// Specify a time range on which to run the backtest.
    /// Data outside this range will be factoredin if OnTick or an indicator requires it, but they will not be evaluated for trading.
    /// </summary>
    /// <param name="start">Candles on or after this date will be evaluated</param>
    /// <param name="end">Candles on or before this data will be evaluated</param>
    /// <returns></returns>
    public BacktestBuilder EvaluateBetween(DateTime start, DateTime end)
    {
        // Validate
        if (BacktestSetup.CandleData is null)
            throw new ArgumentException(nameof(EvaluateBetween) + " should be called after candle data is defined.");
        if (start > end)
            throw new ArgumentException("Start date is after end date.");

        // Find indexes
        BacktestSetup.EvaluateFirstIndex = BacktestSetup.CandleData.FindIndex(c => c.Time >= start);
        BacktestSetup.EvaluateLastIndex = BacktestSetup.CandleData.FindIndex(c => c.Time > end) - 1;
        return this;
    }

    /// <summary>
    /// Shortcut function for EvaluateBetween with string inputs.
    /// </summary>
    /// <param name="startTimeStr">Parsable string</param>
    /// <param name="endTimeStr"></param>
    /// <exception cref="ArgumentException"></exception>
    public BacktestBuilder EvaluateBetween(string startTimeStr, string endTimeStr)
    {
        if (!DateTime.TryParse(startTimeStr, out DateTime start))
            throw new ArgumentException("startTimeStr could not be parsed.");
        if (!DateTime.TryParse(endTimeStr, out DateTime end))
            throw new ArgumentException("endTimeStr could not be parsed.");
        return this.EvaluateBetween(start, end);
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
