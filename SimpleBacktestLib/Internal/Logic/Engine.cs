namespace SimpleBacktestLib.Internal.Logic;

internal static class Engine
{
    /// <summary>
    /// Runs the backtest.
    /// Assumes all validation is already done.
    /// </summary>
    /// <param name="setupDefs"></param>
    /// <returns></returns>
    internal static Task<BacktestResult> RunBacktestAsync(SetupDefinitions setupDefs)
    {
        // Initialize
        BacktestState state = new(setupDefs);

        // Define working values
        decimal startingBudgetValueInQuote = ValueAssessment.GetCombinedValue(AssetType.Quote,
            state.SetupConfig.StartingBaseBalance,
            state.SetupConfig.StartingQuoteBalance,
            state.SetupConfig.CandleData[state.SetupConfig.EvaluateFirstIndex].GetPrice(state.SetupConfig.CandlePriceTime));

        // Loop over the requested candle range
        for (int i = setupDefs.EvaluateFirstIndex; i <= setupDefs.EvaluateLastIndex; i++)
        {
            // Update state for this candle
            state.CurrentCandleIndex = i;

            // Stop the backtest if we are illiquid
            if (!IsLiquid(state, startingBudgetValueInQuote))
            {
                setupDefs.EvaluateLastIndex = i;
                LogHandler.AddLogEntry(state, 
                    $"Stopping backtest because account simulated value dropped to <1% of it's starting value.", 
                    i, LogLevel.Information);
                break;
            }

            // Run all registered tick functions
            foreach (var tickFunc in setupDefs.OnTickFunctions)
                tickFunc(state);

            // Run any registered post-tick functions
            foreach (var postTickFunc in setupDefs.PostTickFunctions)
                postTickFunc(state);
        }

        // Close any open margin positions
        foreach ((int posId, _) in state.MarginTrades.Where(x => !x.Value.IsClosed))
            MarginAccess.ExecuteClosePosition(state, posId);            
        
        // Return result
        return Task.FromResult(BacktestResult.Create(state));
    }

    private static bool IsLiquid(BacktestState state, decimal startingBudgetValueInQuote)
    {
        decimal evaluateAtPrice = state.GetCurrentCandlePrice();

        // Check all margin position liquidity
        // This is not super accurate since it doesn't combine collateral for all margin positions.
        foreach ((int id, MarginPosition pos) in state.MarginTrades.Where(x => !x.Value.IsClosed))
            MarginLogic.LiquidityCheckAndClose(pos, state);

        // Check if we're trading with air and stop if we are
        decimal currentBudgetValueInQuote = ValueAssessment.GetCombinedValue(AssetType.Quote,
            state.BaseBalance, state.QuoteBalance, evaluateAtPrice);

        // Return true when we have more than 1% the starting account value
        return currentBudgetValueInQuote > startingBudgetValueInQuote * 0.01m;
    }
}
