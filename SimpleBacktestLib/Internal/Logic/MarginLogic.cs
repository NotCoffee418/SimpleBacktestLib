namespace SimpleBacktestLib.Internal.Logic;

/// <summary>
/// This class is allowed to modify state.
/// </summary>
internal static class MarginLogic
{
    /// <summary>
    /// Open a long or short position at the current candle with 
    /// user settings and an optionally custom trade input.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="tradeInput"></param>
    /// <returns>Id of the created margin position</returns>
    internal static uint ExecuteOpenPosition(TradeType marginType, BacktestState state, TradeInput tradeInput)
    {
        // Generate the short
        decimal candlePrice = state.GetCurrentCandlePrice();
        uint selectedMarginId = state.NextMarginId;
        MarginPosition pos = MarginPosition.GeneratePosition(
            marginType,
            candlePrice,
            tradeInput,
            state.BaseBalance,
            state.QuoteBalance,
            state.SetupConfig.MarginLeverageRatio,
            state.SetupConfig.MarginLiquidationRatio);

        // Log it
        string positionTypeStr = marginType == TradeType.MarginLong ? "long" : "short";
        LogHandler.AddLogEntry(
            state, $"Opening margin {positionTypeStr} (id: {selectedMarginId}) at price {candlePrice} borrowing {pos.BorrowedAmount}");

        // Add it to dictionary and prepare next position id
        state.MarginTrades.Add(selectedMarginId, pos);
        state.NextMarginId++;
        return selectedMarginId;
    }


    /// <summary>
    /// Close a margin position and take profit
    /// </summary>
    /// <param name="state"></param>
    /// <param name="positionId"></param>
    internal static void ExecuteClosePosition(BacktestState state, uint positionId)
    {
        // Get position value
        (bool isLiquid, decimal newBase, decimal newQuote) = CalculateUnrealizedBalances(
            state.MarginTrades[positionId],
            state.GetCurrentCandlePrice(),
            state.BaseBalance,
            state.QuoteBalance);

        // Log it
        decimal baseProfit = newBase - state.BaseBalance;
        decimal quoteProfit = newQuote - state.QuoteBalance;
        string liquidness = isLiquid ? "liquid" : "illiquid";
        LogHandler.AddLogEntry(state, 
            $"Closed {liquidness} margin position {positionId} with profit {baseProfit} Base Asset and {quoteProfit} Quote Asset", 
            state.CurrentCandleIndex, isLiquid ? LogLevel.Information : LogLevel.Warning);

        // Close the position and take profit/loss
        state.MarginTrades[positionId].MarkAsClosed();
        state.BaseBalance = newBase;
        state.QuoteBalance = newQuote;
    }

    /// <summary>
    /// Calculate unrealized balances factoring in this margin position.
    /// Returns the balances as they would be if the position were closed right now.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="tickPrice"></param>
    /// <param name="baseCollateral"></param>
    /// <param name="quoteCollateral"></param>
    /// <returns></returns>
    internal static (bool IsLiquid, decimal UnrealizedBase, decimal UnrealizedQuote)
        CalculateUnrealizedBalances(MarginPosition position, decimal tickPrice, decimal baseCollateral, decimal quoteCollateral)
    {
        // Determine asset used to borrow
        AssetType borrowedAsset = GetBorrowAssetType(position.MarginDirection);
        decimal repayableAmountInBorrowedAsset = position.MarginDirection == TradeType.MarginLong ?
            ValueAssessment.CalcQuote(position.InitialTradedAmount, tickPrice) : ValueAssessment.CalcBase(position.InitialTradedAmount, tickPrice);
        decimal borrowedPlOnLoan = repayableAmountInBorrowedAsset - position.BorrowedAmount;

        // Calculate updated balances
        decimal unrealizedBase = baseCollateral;
        decimal unrealizedQuote = quoteCollateral;
        if (position.MarginDirection == TradeType.MarginLong)
            unrealizedBase += ValueAssessment.CalcBase(borrowedPlOnLoan, tickPrice);
        else if (position.MarginDirection == TradeType.MarginShort)
            unrealizedQuote += ValueAssessment.CalcQuote(borrowedPlOnLoan, tickPrice);

        // Handle negative collateral
        if (unrealizedBase < 0) // Long broke base liquidity
        {
            unrealizedQuote += ValueAssessment.CalcQuote(unrealizedBase, tickPrice);
            unrealizedBase = 0;
        }
        else if (unrealizedQuote < 0) // Short broke quote liquidity
        {
            unrealizedBase += ValueAssessment.CalcBase(unrealizedQuote, tickPrice);
            unrealizedQuote = 0;
        }

        // Determine if liquid
        decimal combinedCurrentCollateralInBorrowedAsset = ValueAssessment.GetCombinedValue(
            borrowedAsset, unrealizedBase, unrealizedQuote, tickPrice);
        bool isLiquid = combinedCurrentCollateralInBorrowedAsset > position.BorrowedAmount * position.LiquidationRatio;

        // Return parsed results
        return (isLiquid, unrealizedBase, unrealizedQuote);
    }


    /// <summary>
    /// Determine the asset type we need to, or have borrowed depending on the direction of our margin trade.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Must be Long or Short</exception>
    internal static AssetType GetBorrowAssetType(TradeType direction)
    {
        if (direction != TradeType.MarginLong && direction != TradeType.MarginShort)
            throw new ArgumentException("GetPosition() expects direction to be either MarginLong or MarginShort.");
        return direction == TradeType.MarginLong ? AssetType.Quote : AssetType.Base;
    }

}
