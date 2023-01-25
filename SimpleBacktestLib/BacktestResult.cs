namespace SimpleBacktestLib;

public record BacktestResult
{
    internal static BacktestResult Create(BacktestState state)
    {
        // Get candles and price info
        BacktestCandle firstCandle = state.SetupConfig.CandleData[state.SetupConfig.EvaluateFirstIndex];
        BacktestCandle finalCandle = state.SetupConfig.CandleData[state.SetupConfig.EvaluateLastIndex];
        decimal startPrice = firstCandle.GetPrice(state.SetupConfig.CandlePriceTime);
        decimal finalPrice = finalCandle.GetPrice(state.SetupConfig.CandlePriceTime);

        // Calculate profits
        decimal combinedStartBaseBalance = ValueAssessment.GetCombinedValue(
            AssetType.Base,
            state.SetupConfig.StartingBaseBalance,
            state.SetupConfig.StartingQuoteBalance,
            startPrice);
        decimal combinedFinalBaseBalance = ValueAssessment.GetCombinedValue(
            AssetType.Base,
            state.BaseBalance,
            state.QuoteBalance,
            finalPrice);
        decimal baseProfit = combinedFinalBaseBalance - combinedStartBaseBalance;
        decimal quoteProfit = ValueAssessment.CalcQuote(baseProfit, finalPrice);
        decimal profitRatio = ValueAssessment.GetProfitRatio(combinedStartBaseBalance, combinedFinalBaseBalance);
        decimal buyAndHoldProfitRatio = ValueAssessment.GetProfitRatio(startPrice, finalPrice);


        // Create the object
        return new BacktestResult
        {
            TotalProfitInBase = baseProfit,
            TotalProfitInQuote = quoteProfit,
            ProfitRatio = profitRatio,
            BuyAndHoldProfitRatio = buyAndHoldProfitRatio,
            StartingBaseBudget = state.SetupConfig.StartingBaseBalance,
            StartingQuoteBudget = state.SetupConfig.StartingQuoteBalance,
            FinalBaseBudget = state.BaseBalance,
            FinalQuoteBudget = state.QuoteBalance,
            SpotTrades = state.SpotTrades.ToImmutableList(),
            MarginTrades = state.MarginTrades.Values.ToImmutableList(),
            FirstCandleTime = firstCandle.Time,
            LastCandleTime = finalCandle.Time,
        };
    }


    /// <summary>
    /// Total profit made during the backtest expressed in base asset.
    /// This factors in the value of your entire balance (quote and base).
    /// </summary>
    public decimal TotalProfitInBase { get; private set; }

    /// <summary>
    /// Total profit made during the backtest expressed in quote asset.
    /// This factors in the value of your entire balance (quote and base).
    /// </summary>
    public decimal TotalProfitInQuote { get; private set; }

    /// <summary>
    /// Ratio of total gain or loss throughout the backtest.
    /// </summary>
    public decimal ProfitRatio { get; private set; }

    /// <summary>
    /// Ratio of total gains or loss that would have occurred when buying and holding the asset instead.
    /// </summary>
    public decimal BuyAndHoldProfitRatio { get; private set; }


    /// <summary>
    /// Base budget at the start of the backtest.
    /// </summary>
    public decimal StartingBaseBudget { get; private set; }

    /// <summary>
    /// Quote budget at the start of the backtest
    /// </summary>
    public decimal StartingQuoteBudget { get; private set; }

    /// <summary>
    /// Base budget at the end of the backtest
    /// </summary>
    public decimal FinalBaseBudget { get; private set; }

    /// <summary>
    /// Quote budget at the end of the backtest
    /// </summary>
    public decimal FinalQuoteBudget { get; private set; }


    /// <summary>
    /// All spot trades executed during the backtest.
    /// </summary>
    public ImmutableList<BacktestTrade> SpotTrades { get; private set; }

    /// <summary>
    /// All margin trades executed during the backtest.
    /// </summary>
    public ImmutableList<MarginPosition> MarginTrades { get; private set; }
    
    /// <summary>
    /// Time of the first candle
    /// </summary>
    public DateTime FirstCandleTime { get; private set; }

    /// <summary>
    /// Time of the last candle
    /// </summary>
    public DateTime LastCandleTime { get; private set; }

    /// <summary>
    /// Time between the first and last candle that was evaluated.
    /// </summary>
    /// <returns></returns>
    public TimeSpan EvaluatedCandleTimespan()
        => LastCandleTime.Subtract(FirstCandleTime);
}
