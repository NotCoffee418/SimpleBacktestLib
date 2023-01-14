namespace SimpleBacktestLib.Internal.Margin;

/// <summary>
/// Describes an active margin position. Should be defined by the <see cref="Engine"/> class.
/// </summary>
public record MarginPosition
{
    /// <summary>
    /// The price at which the margin position was opened.
    /// </summary>
    public decimal OpenPrice { get; private init; }

    /// <summary>
    /// Amount borrowed in quote asset for long positions, or base asset for short positions.
    /// </summary>
    public decimal BorrowedAmount { get; private init; }

    /// <summary>
    /// Amount we exchanged the borrowed amount into.
    /// Longs get base. Shorts get quote.
    /// </summary>
    public decimal InitialTradeAmount { get; private init; }

    /// <summary>
    /// Direction for our trade. Should only be long or short.
    /// </summary>
    public TradeType MarginDirection { get; private init; }

    /// <summary>
    /// The price at which the margin position was closed.
    /// Set to 1 or override when no margin position is open.
    /// </summary>
    public decimal LeverageRatio { get; private init; }

    /// <summary>
    /// When the combined liquidity drops below this ratio, the margin position closes at a loss.
    /// </summary>
    public decimal LiquidationRatio { get; private init; }


    /// <summary>
    /// Create a margin position using all baseCollateral and quoteCollateral
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="openPrice"></param>
    /// <param name="baseCollateral">Will be fully used as collateral and still be available for other trades</param>
    /// <param name="quoteCollateral">Will be fully used as collateral and still be available for other trades</param>
    /// <param name="leverageRatio">Ratio of the borrowed amount relative to the collateral. 5x margin on 1000$ is 5000$ position</param>
    /// <param name="liquidationRatio">Tick will liquidate if collateral value is below this ratio of the BorrowedAmount</param>
    /// <returns>Position to be used as a trade</returns>
    internal static MarginPosition GeneratePosition(
        TradeType direction,
        decimal openPrice,
        decimal baseCollateral,
        decimal quoteCollateral,
        decimal leverageRatio = 1,
        decimal liquidationRatio = 0.1m)
    {
        // Determine the starting amount we can use as collateral, as valued in the asset we want to borrow
        AssetType borrowAsset = GetBorrowAssetType(direction);
        decimal collateralValue = ValueAssessment.GetCombinedValue(borrowAsset, baseCollateral, quoteCollateral, openPrice);

        // Apply leverage to determine the amount borrowed
        decimal borrowedAmount = collateralValue * leverageRatio;

        // Determine the amount it exchanges for
        // Longs get base, shorts get quote
        decimal tradedAmount = direction == TradeType.MarginLong ?
            borrowedAmount / openPrice : openPrice * borrowedAmount;

        // Validate
        if (borrowedAmount <= 0)
            throw new ArgumentException("borrowedAmount <= 0.");

        // Return the order type
        return new MarginPosition
        {
            OpenPrice = openPrice,
            BorrowedAmount = borrowedAmount,
            InitialTradeAmount = tradedAmount,
            MarginDirection = direction,
            LeverageRatio = leverageRatio,
            LiquidationRatio = liquidationRatio,
        };
    }

    /// <summary>
    /// Calculate unrealized balances factoring in this margin position.
    /// Returns the balances as they would be if the position were closed right now.
    /// </summary>
    /// <param name="tickPrice"></param>
    /// <param name="baseCollateral"></param>
    /// <param name="quoteCollateral"></param>
    /// <returns></returns>
    internal (bool IsLiquid, decimal UnrealizedBase, decimal UnrealizedQuote)
        CalculateUnrealizedBalances(decimal tickPrice, decimal baseCollateral, decimal quoteCollateral)
    {
        // Determine asset used to borrow
        AssetType borrowedAsset = GetBorrowAssetType(MarginDirection);
        decimal repayableAmountInBorrowedAsset = MarginDirection == TradeType.MarginLong ?
            ValueAssessment.CalcQuote(InitialTradeAmount, tickPrice) : ValueAssessment.CalcBase(InitialTradeAmount, tickPrice);
        decimal borrowedPlOnLoan = repayableAmountInBorrowedAsset - BorrowedAmount;

        // Calculate updated balances
        decimal unrealizedBase = baseCollateral;
        decimal unrealizedQuote = quoteCollateral;
        if (MarginDirection == TradeType.MarginLong)
            unrealizedBase += ValueAssessment.CalcBase(borrowedPlOnLoan, tickPrice);
        else if (MarginDirection == TradeType.MarginShort)
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
        bool isLiquid = combinedCurrentCollateralInBorrowedAsset > BorrowedAmount * LiquidationRatio;

        // Return parsed results
        return (isLiquid, unrealizedBase, unrealizedQuote);
    }


    /// <summary>
    /// Determine the asset type we need to, or have borrowed depending on the direction of our margin trade.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Must be Long or Short</exception>
    private static AssetType GetBorrowAssetType(TradeType direction)
    {
        if (direction != TradeType.MarginLong && direction != TradeType.MarginShort)
            throw new ArgumentException("GetPosition() expects direction to be either MarginLong or MarginShort.");
        return direction == TradeType.MarginLong ? AssetType.Quote : AssetType.Base;
    }

}
