namespace SimpleBacktestLib.Internal.Logic;


/// <summary>
/// This class is allowed to modify state.
/// </summary>
internal static class SpotAccess
{
    /// <summary>
    /// High level internal function to execute a spot buy
    /// </summary>
    /// <param name="state"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    internal static bool ExecuteBuy(BacktestState state, TradeInput input)
    {
        // Check balance
        if (state.QuoteBalance <= 0)
        {
            LogHandler.AddLogEntry(state, $"No quote balance available. Cannot execute buy.", state.CurrentCandleIndex, LogLevel.Error);
            return false;
        }
        
        // Execute trade
        decimal currentPrice = state.GetCurrentCandlePrice();
        (decimal baseGained, decimal quoteRemoved, bool usedFullRequestAmount) 
            = TradeLogic.SimulateBuy(currentPrice, state.QuoteBalance, input, state.SetupConfig.SpotFees);

        // Validation
        if (quoteRemoved > state.QuoteBalance)
        {
            LogHandler.AddLogEntry(state, $"Insufficient quote balance to buy {input.Amount} at {currentPrice}", state.CurrentCandleIndex, LogLevel.Error);
            return false;
        }

        // Trade successful
        state.BaseBalance += baseGained;
        state.QuoteBalance -= quoteRemoved;
        state.SpotTrades.Add(new(
            TradeOperation.Buy,
            currentPrice,
            baseGained,
            quoteRemoved,
            state.CurrentCandleIndex,
            state.GetCurrentCandle().Time));

        // Log it
        LogHandler.AddLogEntry(state, $"Bought {baseGained} Base Asset for {quoteRemoved} Quote Asset at price {currentPrice}", state.CurrentCandleIndex, LogLevel.Information);
        return true;
    }


    /// <summary>
    /// High level internal function to execute a spot sell
    /// </summary>
    /// <param name="state"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    internal static bool ExecuteSell(BacktestState state, TradeInput input)
    {
        // Check balance
        if (state.BaseBalance <= 0)
        {
            LogHandler.AddLogEntry(state, $"No base balance available. Cannot execute sell.", state.CurrentCandleIndex, LogLevel.Error);
            return false;
        }

        // Execute trade
        decimal currentPrice = state.GetCurrentCandlePrice();
        (decimal baseRemoved, decimal quoteGained, bool usedFullRequestAmount)
            = TradeLogic.SimulateSell(currentPrice, state.BaseBalance, input, state.SetupConfig.SpotFees);

        // Validation
        if (baseRemoved > state.BaseBalance)
        {
            LogHandler.AddLogEntry(state, $"Insufficient base balance to sell {input.Amount} at {currentPrice}", state.CurrentCandleIndex, LogLevel.Error);
            return false;
        }

        // Trade successful
        state.BaseBalance -= baseRemoved;
        state.QuoteBalance += quoteGained;
        state.SpotTrades.Add(new(
            TradeOperation.Sell,
            currentPrice,
            baseRemoved,
            quoteGained,
            state.CurrentCandleIndex,
            state.GetCurrentCandle().Time));

        // Log it
        LogHandler.AddLogEntry(state, $"Sold {baseRemoved} Base Asset for {quoteGained} Quote Asset at price {currentPrice}", state.CurrentCandleIndex, LogLevel.Information);
        return true;
    }
}
