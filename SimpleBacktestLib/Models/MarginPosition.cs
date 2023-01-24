namespace SimpleBacktestLib.Models;

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
    public decimal InitialTradedAmount { get; private init; }

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
    /// Is the position closed?
    /// </summary>
    public bool IsClosed { get; private set; } = false;


    /// <summary>
    /// Shortcut function for GeneratePosition witout specifying the amount.
    /// Instead, values from setup definition can be passed in to get the approperiate amount.
    /// </summary>
    /// <param name="marginAmountType"></param>
    /// <param name="marginAmountRequested">Not literal! Meaning depends on marginAmountType.</param>
    /// <param name="direction"></param>
    /// <param name="openPrice"></param>
    /// <param name="baseCollateral"></param>
    /// <param name="quoteCollateral"></param>
    /// <param name="leverageRatio"></param>
    /// <param name="liquidationRatio"></param>
    /// <returns></returns>
    internal static MarginPosition GeneratePosition(
        TradeType direction,
        decimal openPrice,
        TradeInput tradeInput,
        decimal baseCollateral,
        decimal quoteCollateral,
        decimal leverageRatio,
        decimal liquidationRatio)
    {
        // Calculate borrow amount from an AmountType & amountRequested
        AssetType borrowAsset = MarginLogic.GetBorrowAssetType(direction);
        decimal maxBorrowable = leverageRatio * ValueAssessment.GetCombinedValue(borrowAsset, baseCollateral, quoteCollateral, openPrice);
        (decimal borrowAmount, _) = tradeInput.GetLiteralValue(maxBorrowable);

        // Generate the position
        return GeneratePosition(direction, openPrice, borrowAmount, baseCollateral, quoteCollateral, leverageRatio, liquidationRatio);
    }

    /// <summary>
    /// Create a margin position using all baseCollateral and quoteCollateral
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="openPrice">Price at which to create the position</param>
    /// <param name="borrowAmount">Amount to borrow. Quote for long, base for short.</param>
    /// <param name="baseCollateral">Will be fully used as collateral and still be available for other trades</param>
    /// <param name="quoteCollateral">Will be fully used as collateral and still be available for other trades</param>
    /// <param name="leverageRatio">Ratio of the borrowed amount relative to the collateral. 5x margin on 1000$ is 5000$ position</param>
    /// <param name="liquidationRatio">Tick will liquidate if collateral value is below this ratio of the BorrowedAmount</param>
    /// <returns>Position to be used as a trade</returns>
    internal static MarginPosition GeneratePosition(
        TradeType direction,
        decimal openPrice,
        decimal borrowAmount,
        decimal baseCollateral,
        decimal quoteCollateral,
        decimal leverageRatio,
        decimal liquidationRatio)
    {
        if (direction != TradeType.MarginLong && direction != TradeType.MarginShort)
            throw new ArgumentException("Margin position must be long or short.");

        // Determine the max amount we can borrow.
        AssetType borrowAsset = MarginLogic.GetBorrowAssetType(direction);
        decimal collateralValue = ValueAssessment.GetCombinedValue(borrowAsset, baseCollateral, quoteCollateral, openPrice);
        decimal maxBorrowAmount = collateralValue * leverageRatio;

        // Determine the amount it exchanges for
        // Longs get base, shorts get quote
        decimal tradedAmount = direction == TradeType.MarginLong ?
            borrowAmount / openPrice : openPrice * borrowAmount;

        // Validate
        if (borrowAmount <= 0)
            throw new ArgumentException("borrowedAmount <= 0.");
        if (borrowAmount > maxBorrowAmount)
            throw new ArgumentException("borrowAmount > maxBorrowAmount");

        // Return the order type
        return new MarginPosition
        {
            OpenPrice = openPrice,
            BorrowedAmount = borrowAmount,
            InitialTradedAmount = tradedAmount,
            MarginDirection = direction,
            LeverageRatio = leverageRatio,
            LiquidationRatio = liquidationRatio,
        };
    }


    /// <summary>
    /// Mark a position as closed. Does not include profit/loss logic.
    /// </summary>
    internal void MarkAsClosed()
    {
        if (IsClosed)
            throw new InvalidOperationException("Position is already closed.");
        IsClosed = true;
    }
}
