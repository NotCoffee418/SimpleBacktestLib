# Backtest Configuration
There are a number of configuration options available to customize the backtest. These can be appended to the `BacktestBuilder` object using the following methods:

#### CreateBuilder(IEnumerable<BacktestCandle> candleData)
Creates a new `BacktestBuilder` object with input candle data.  
This is the only way to create a new instance of BacktestBuilder.

#### Run() / RunAsync()
Starts the backtest and returns [BacktestResult](type-backtestresult.md).  
This method can take a long time to execute depending on the amount of data and the complexity of the strategy.

#### WithQuoteBudget(decimal quoteBudget)
Sets the starting quote balance of the backtest.  
This is the balance consumed when spot buying or margin trading long.

#### WithBaseBudget(decimal baseBudget)
Sets the starting base balance of the backtest.  
This is the balance consumed when spot selling or margin trading short.

#### EvaluateBetween(DateTime start, DateTime end)
Sets the start and end date of the backtest.  
This is useful if you want to use data from a longer period of time, but only evaluate a specific period.  

By default, the backtest will evaluate only the last month of data.  
This method is optional and can be omitted.  
This method has an alternative version that accepts strings as input.  

#### OnTick(Action<BacktestState> onTickFunction)
Register a function to be called on each tick of the backtest.  
This function should contain your stategy logic and is able to execute simulated trades through the `BacktestState` object.  

Multiple OnTick functions can be registered, though this is not recommended.  
You must have at least one OnTick function registered to start the backtest.  

#### PostTick(Action<BacktestState> postTickFunction)
Register a function to be called after each tick of the backtest.  
This is an optional function that can be used to perform additional logic after each tick.  

Multiple PostTick functions can be registered.  
This function is optional and can be omitted.  

#### OnLogEntry(Action<BacktestLogEntry> onLogEntryFunction)
Register a function to be called when a log entry is created.  
This is an optional function that can be used to perform additional logic when a log entry is created.  

Multiple OnLogEntry functions can be registered.  
This function is optional and can be omitted.  

#### WithCandlePriceTime(PriceTime atTime)
Sets the time of the candle to use for the backtest.  
Options are: `AtOpen`, `AtClose`, `AtHigh`, `AtLow`, `AtRandom`.  
AtRandom will use a random time between the open and close of the candle.  

#### WithDefaultSpotBuyOrderSize(AmountType amountType, decimal amount, bool allowPartial = true)
Sets the default quote amount to consume when spot buying.  
For more information on the `AmountType` enum, see [here](type-tradeinput-fee.md).

#### WithDefaultSpotSellOrderSize(AmountType amountType, decimal amount, bool allowPartial = true)
Sets the default base amount to consume when spot selling.  
For more information on the `AmountType` enum, see [here](type-tradeinput-fee.md).

#### WithDefaultMarginLongOrderSize(AmountType amountType, decimal amount, bool allowPartial = true)
Specify the default quote input size for margin trading.
Percentage values are relative to the combined collateral of the base and quote balance.
Absolute values specify the amount to borrow and exchange into the other asset.
Max will utilize the full base and quote balances as collateral.
For more information on the `AmountType` enum, see [here](type-tradeinput-fee.md).

#### WithDefaultMarginShortOrderSize(AmountType amountType, decimal amount, bool allowPartial = true)
Specify the default base input size for margin trading.
Percentage values are relative to the combined collateral of the base and quote balance.
Absolute values specify the amount to borrow and exchange into the other asset.
Max will utilize the full base and quote balances as collateral.
For more information on the `AmountType` enum, see [here](type-tradeinput-fee.md).

#### WithMarginLeverage(decimal leverage)
Specify the leverage to use for margin trading.
Setting this value to 5 will allow you to borrow up to 5x the amount of your combined balance value.
Default value is 5.

#### WithMarginLiquidationRatio(decimal ratio)
Ratio below which the margin position will liquidate. Default is 0.1.

#### AddSpotFee(AmountType amountType, decimal amount, FeeSource feeSource)
Adds a spot fee to the backtest. Multiple fees with different structures can be added.
For more information on the `AmountType` enum, see [here](type-tradeinput-fee.md).
**NOTE:** This method will remove the default the default spot fees when first called.

#### RemoveSpotFees()
Removes the default spot fees from the backtest.

### WithInitialCustomData(object data)
Sets the initial custom data object to be used in the backtest.