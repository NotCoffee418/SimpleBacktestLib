# Type: BacktestResult

The `BacktestResult` class is a record that contains the results of a backtest. It contains the following properties:

- `TotalProfitInQuote`: The total profit made during the backtest, expressed in the quote asset. This factors in the value of the entire balance (quote and base).
- `ProfitRatio`: The ratio of total gain or loss throughout the backtest.
- `BuyAndHoldProfitRatio`: The ratio of total gains or loss that would have occurred when buying and holding the asset instead of trading.
- `StartingBaseBudget`: The base budget at the start of the backtest.
- `StartingQuoteBudget`: The quote budget at the start of the backtest.
- `FinalBaseBudget`: The base budget at the end of the backtest.
- `FinalQuoteBudget`: The quote budget at the end of the backtest.
- `SpotTrades`: A list of all spot trades executed during the backtest, represented as `BacktestTrade` objects.
- `MarginTrades`: A list of all margin trades executed during the backtest, represented as `MarginPosition` objects.
- `FirstCandleTime`: The timestamp of the first candle that was evaluated during the backtest.
- `LastCandleTime`: The timestamp of the last candle that was evaluated during the backtest.
- `EvaluatedCandleTimespan()`: A method that returns the time between the first and last candle that was evaluated, represented as a `TimeSpan` object.

All properties have a private setter, meaning that they can be set only by the class that creates the BacktestResult object.

This class can be used to access results of a backtest and evaluate the performance of your strategy.
