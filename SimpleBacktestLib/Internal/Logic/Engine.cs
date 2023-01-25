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
        
        // Loop over the requested candle range
        for (int i = setupDefs.EvaluateFirstIndex; i <= setupDefs.EvaluateFirstIndex; i++)
        {
            // Update state for this candle
            state.CurrentCandleIndex = i;

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
}
