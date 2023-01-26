# Getting Started

### Creating a project
SimpleBacktestLib works on all project types supporting .NET 7 or higher, including Polyglot projects.  
The simplest way to get started is to create a console project using Visual Studio.

### Installation
SimpleBacktestLib is available as a [NuGet package](https://www.nuget.org/packages/SimpleBacktestLib). You can install it using the NuGet package manager in Visual Studio or by running the following command in the package manager console:

```powershell
Install-Package SimpleBacktestLib
```

### Setting up the Backtest
First you will need to [load your candle data](loading-candle-data.md).  
SimpleBacktestLib does not provide any built-in functionality to load candle data, but you can use any method you like to load your data, as long as it is shaped as a list of [BacktestCandle](type-backtestcandle.md).

Then we can configure the backtest. Each backtest needs to at least have a strategy defined through `Tick()` like so.

```csharp
BacktestBuilder builder = BacktestBuilder.Create(candleData)
    .Tick(state =>
    {
        // your strategy code goes here
    })
    .OnLogEntry((logEntry, state) =>
    {
        // Optionally do something when a log entry is created
    });
```

Additionally, you can append a number of options to configure the backtest.  
See [Configuring the backtest](backtest-configuration.md) for more information.

### Implementing your strategy
The `Tick()` method is called for each candle in the backtest and is where you implement your strategy.  
The method takes a [BacktestState](type-backteststate.md) object as a parameter, which contains all the information you need to implement your strategy.

You can execute trades by calling the following methods on the `BacktestState` object inside the `Tick()` method:
- `x.Trade.Spot.Buy()`
- `x.Trade.Spot.Sell()`
- `x.Trade.Margin.long()`
- `x.Trade.Margin.Short()`
- `x.Trade.Margin.ClosePosition(positionId)`

These optionally also take an override amount parameter by which you can override the default amount of the trade that was configured in BacktestBuilder.

### Running the backtest
Once you have configured your backtest, you can run it using the `Run()` or `RunAsync()` methods.

```csharp
BacktestResult result = await builder.RunAsync();
```

### Accessing the results
The [BacktestResult](type-backtestresult.md) object contains all the information about the backtest.
You can use the object to evaluate the performance of your strategy, access the logs and review individual trades.