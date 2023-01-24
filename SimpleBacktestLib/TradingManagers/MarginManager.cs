namespace SimpleBacktestLib.TradingManagers;

public class MarginManager
{
    /// <summary>
    /// Should not be instantiated externally.
    /// These functions can be called through Tick()'s x.Trade.Margin
    /// </summary>
    internal MarginManager(BacktestState state)
    {
        State = state;
    }

    /// <summary>
    /// Reference to the instance state for reading and modifying.
    /// </summary>
    private BacktestState State { get; }

    /// <summary>
    /// Open a margin long position at the current price with the default.
    /// </summary>
    /// <returns>Margin position id needed to close the position.</returns>
    public uint Long()
        => Short(State.SetupConfig.DefaultMarginShortOrderSize);

    /// <summary>
    /// Open a margin long position at the current price with a custom amount.
    /// Amount specified is the input amount to borrow.
    /// </summary>
    /// <param name="amountType"></param>
    /// <param name="inputAmount"></param>
    /// <param name="allowPartial"></param>
    /// <returns></returns>
    public uint Long(AmountType amountType, decimal inputBorrowAmount, bool allowPartial = true)
        => Long(new TradeInput(amountType, inputBorrowAmount, allowPartial));

    /// <summary>
    /// Open a margin long position at the current price with a custom amount.
    /// Amount specified is the input amount to borrow.
    /// </summary>
    /// <param name="tradeInput"></param>
    /// <returns>Margin position id needed to close the position.</returns>
    public uint Long(TradeInput tradeInput)
        => MarginLogic.ExecuteOpenPosition(TradeType.MarginLong, State, tradeInput);
    

    /// <summary>
    /// Open a margin short position at the current price with the default.
    /// </summary>
    /// <returns>Margin position id needed to close the position.</returns>
    public uint Short()
        => Short(State.SetupConfig.DefaultMarginShortOrderSize);

    /// <summary>
    /// Open a margin short position at the current price with a custom amount.
    /// Amount specified is the input amount to borrow.
    /// </summary>
    /// <param name="amountType"></param>
    /// <param name="inputAmount"></param>
    /// <param name="allowPartial"></param>
    /// <returns></returns>
    public uint Short(AmountType amountType, decimal inputBorrowAmount, bool allowPartial = true)
        => Short(new TradeInput(amountType, inputBorrowAmount, allowPartial));

    /// <summary>
    /// Open a margin short position at the current price with a custom amount.
    /// Amount specified is the input amount to borrow.
    /// </summary>
    /// <param name="tradeInput"></param>
    /// <returns>Margin position id needed to close the position.</returns>
    public uint Short(TradeInput tradeInput)
        => MarginLogic.ExecuteOpenPosition(TradeType.MarginShort, State, tradeInput);

    /// <summary>
    /// Close an opened margin position
    /// </summary>
    /// <param name="positionId">Id can be found when creating the position or in State</param>
    public void ClosePosition(uint positionId)
        => MarginLogic.ExecuteClosePosition(State, positionId);
}
