using SimpleBacktestLib.Internal.InternalModels;

namespace SimpleBacktestLib;

/// <summary>
/// Primary access point to the SimpleBacktestLib's core functions.
/// </summary>
public class BacktestBuilder
{
    /// <summary>
    /// Should be instantiated through CreateBuilder() only
    /// </summary>
    private BacktestBuilder() { }

    /// <summary>
    /// Stores the backtest setup definitions.
    /// </summary>
    private SetupDefinitions BacktestSetup { get; } = new();

    /// <summary>
    /// Helps to remove default fee when user specifies fees.
    /// </summary>
    private bool IsDefaultSpotFeeReset { get; set; } = false;

    /// <summary>
    /// Create a backtest builder with the provided candle data and default settings.
    /// </summary>
    /// <returns></returns>
    public static BacktestBuilder CreateBuilder(IEnumerable<BacktestCandle> candleData)
    {
        BacktestBuilder btb = new();
        
        // Validate dataset
        if (btb.BacktestSetup.CandleData is not null)
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
        btb.BacktestSetup.CandleData = candleData.ToImmutableList();

        // Set default evaluate time at the last month of data.
        DateTime endEvaluateTime = candleData.Last().Time;
        DateTime startEvaluateTime = endEvaluateTime.AddDays(-30);
        return btb.EvaluateBetween(startEvaluateTime, endEvaluateTime);
    }

    /// <summary>
    /// Specify a function that will run each candle and returns any trades to be executed if possible.
    /// Multiple OnTick definitions are allowed and will be called in order of definition.
    /// </summary>
    /// <param name="tickFunction"></param>
    /// <returns></returns>
    public BacktestBuilder OnTick(Action<BacktestState> onTickFunction)
    {
        BacktestSetup.OnTickFunctions.Add(onTickFunction);
        return this;
    }
    
    /// <summary>
    /// Optional functions that runs after a tick is completed, before the next tick.
    /// Can be used for cleanup or other operations.
    /// </summary>
    /// <param name="postTickFunction"></param>
    /// <returns></returns>
    public BacktestBuilder PostTick(Action<BacktestState> postTickFunction)
    {
        BacktestSetup.PostTickFunctions.Add(postTickFunction);
        return this;
    }


    /// <summary>
    /// Optional function that fires whenever a log entry is made by the user application or the library.
    /// Useful for reporting in real-time or handling exceptions.
    /// </summary>
    /// <param name="onLogEntryFunction"></param>
    /// <returns></returns>
    public BacktestBuilder OnLogEntry(Action<LogEntry, BacktestState> onLogEntryFunction)
    {
        BacktestSetup.OnLogEntryFunctions.Add(onLogEntryFunction);
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

        // Handle end time being outside of the candle data range
        // FindIndex would set it to -1, from which we subtract another 1, making it -2
        if (BacktestSetup.EvaluateLastIndex < 0)
            BacktestSetup.EvaluateLastIndex = BacktestSetup.CandleData.Count - 1;
        
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
    /// Specify which price to use for evaluating trades and executing them.
    /// Defaults to candle open price.
    /// </summary>
    /// <param name="atTime"></param>
    /// <returns></returns>
    public BacktestBuilder WithCandlePriceTime(PriceTime atTime)
    {
        BacktestSetup.CandlePriceTime = atTime;
        return this;
    }

    /// <summary>
    /// Specify the default order size when trading the quote asset.
    /// By default, it will use the full available balance (Max).
    /// This can be overridden in the trading strategy.
    /// </summary>
    /// <param name="amountType">Define the meaning of the amount</param>
    /// <param name="amount">Amount or value</param>
    /// <param name="allowPartial">When false, an exception is thrown instead</param>
    /// <returns></returns>
    public BacktestBuilder WithDefaultSpotBuyOrderSize(AmountType amountType, decimal amount, bool allowPartial = true)
    {
        BacktestSetup.DefaultSpotBuyOrderSize = new TradeInput(amountType, amount, allowPartial);
        return this;
    }

    /// <summary>
    /// Specify the default order size when trading the base asset.
    /// By default, it will use the full available balance (Max).
    /// This can be overridden in the trading strategy.
    /// </summary>
    /// <param name="amountType">Define the meaning of the amount</param>
    /// <param name="amount">Amount or value</param>
    /// <param name="allowPartial">When false, an exception is thrown instead</param>
    /// <returns></returns>
    public BacktestBuilder WithDefaultSpotSellOrderSize(AmountType amountType, decimal amount, bool allowPartial = true)
    {
        BacktestSetup.DefaultSpotSellOrderSize = new TradeInput(amountType, amount, allowPartial);
        return this;
    }


    /// <summary>
    /// Add a custom spot fee that will be applied to each spot trade.
    /// By default, a 0.1% fee is applied to all trades.
    /// 
    /// NOTE! When adding adding any custom fees, the default 0.1% fee will be removed.
    /// If you wish to retain it, it should be manually added again.
    /// 
    /// Multiple fees can be applied to each trade. They will be executed in the order they were added.
    /// </summary>
    /// <param name="amountType">Define the meaning of the amount</param>
    /// <param name="amount">Amount or value</param>
    /// <param name="feeSource">From which asset(s) is the fee paid?</param>
    /// <returns></returns>
    public BacktestBuilder AddSpotFee(AmountType amountType, decimal amount, FeeSource feeSource)
    {
        // Remove the default spot fee when user adds their own fee
        if (!IsDefaultSpotFeeReset)
        {
            _ = RemoveSpotFees();
            IsDefaultSpotFeeReset = true;
        }

        BacktestSetup.SpotFees.Add(new(amountType, amount, feeSource));
        return this;
    }

    /// <summary>
    /// Remove all spot trading fees.
    /// </summary>
    /// <returns></returns>
    public BacktestBuilder RemoveSpotFees()
    {
        BacktestSetup.SpotFees.Clear();
        return this;
    }

    /// <summary>
    /// Specify the default quote input size for margin trading.
    /// Percentage values are relative to the combined collateral of the base and quote balance.
    /// Absolute values specify the amount to borrow and exchange into the other asset.
    /// Max will utilize the full base and quote balances as collateral.
    /// </summary>
    /// <param name="type">Define the meaning of the amount</param>
    /// <param name="amount">Amount or value</param>
    /// <param name="allowPartial">When false, an exception is thrown instead</param>
    /// <returns></returns>
    public BacktestBuilder WithDefaultMarginLongOrderSize(AmountType type, decimal amount, bool allowPartial = true)
    {
        BacktestSetup.DefaultMarginLongOrderSize = new TradeInput(type, amount, allowPartial);
        return this;
    }

    /// <summary>
    /// Specify the default quote input size for margin trading.
    /// Percentage values are relative to the combined collateral of the base and quote balance.
    /// Absolute values specify the amount to borrow and exchange into the other asset.
    /// Max will utilize the full base and quote balances as collateral.
    /// </summary>
    /// <param name="type">Define the meaning of the amount</param>
    /// <param name="amount">Amount or value</param>
    /// <param name="allowPartial">When false, an exception is thrown instead</param>
    /// <returns></returns>
    public BacktestBuilder WithDefaultMarginShortOrderSize(AmountType type, decimal amount, bool allowPartial = true)
    {
        BacktestSetup.DefaultMarginShortOrderSize = new TradeInput(type, amount, allowPartial);
        return this;
    }
    
    /// <summary>
    /// Sets the margin leverage ratio for the backtest. Default is 5.
    /// </summary>
    /// <param name="ratio"></param>
    /// <returns></returns>
    public BacktestBuilder WithMarginLeverageRatio(decimal ratio)
    {
        BacktestSetup.MarginLeverageRatio = ratio;
        return this;
    }

    /// <summary>
    /// Ratio below which the margin position will liquidate. Default is 0.1.
    /// </summary>
    /// <returns></returns>
    public BacktestBuilder WithMarginLiquidationRatio(decimal liquidationRatio)
    {
        BacktestSetup.MarginLiquidationRatio = liquidationRatio;
        return this;
    }


    /// <summary>
    /// Define the quote trading balance.
    /// Default quote budget is 10000.
    /// </summary>
    /// <param name="quoteBudget">Amount of quote asset to start the backtest with</param>
    /// <returns></returns>
    public BacktestBuilder WithQuoteBudget(decimal quoteBudget)
    {
        if (quoteBudget <= 0)
            throw new ArgumentException("Quote budget must be greater than 0.");
        BacktestSetup.StartingQuoteBalance = quoteBudget;
        return this;
    }

    /// <summary>
    /// Define the base trading balance.
    /// By default, the base budget is 0 since we start with a quote budget instead.
    /// </summary>
    /// <param name="baseBudget">Amount of base asset to start the backtest with</param>
    /// <returns></returns>
    public BacktestBuilder WithBaseBudget(decimal baseBudget)
    {
        if (baseBudget < 0)
            throw new ArgumentException("Base budget cannot be negative.");
        BacktestSetup.StartingBaseBalance = baseBudget;
        return this;
    }

    /// <summary>
    /// Run the backtest asynchronously.
    /// </summary>
    /// <returns></returns>
    public async Task<BacktestResult> RunAsync()
    {
        // Validate setup
        if (BacktestSetup.OnTickFunctions.Count < 1)
            throw new ArgumentException("At least one OnTick function must be added to the backtest setup.");
        

        // Run the backtest
        return await Engine.RunBacktestAsync(BacktestSetup);
    }

    /// <summary>
    /// Run the backtest synchronously.
    /// </summary>
    /// <returns></returns>
    public BacktestResult Run()
        => RunAsync().ConfigureAwait(false).GetAwaiter().GetResult();

}
