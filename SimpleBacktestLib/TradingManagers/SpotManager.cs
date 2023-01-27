namespace SimpleBacktestLib.TradingManagers;

public class SpotManager
{
    /// <summary>
    /// Should not be instantiated externally.
    /// These functions can be called through Tick()'s x.Trade.Margin
    /// </summary>
    internal SpotManager(BacktestState state) 
    {
        State = state;
    }

    /// <summary>
    /// Reference to the instance state for reading and modifying.
    /// </summary>
    private BacktestState State { get; }

    /// <summary>
    /// Buy an amount as specified by the settings.
    /// </summary>
    /// <returns>Trade successfully executed?</returns>
    public bool Buy()
        => Buy(State.SetupConfig.DefaultSpotBuyOrderSize);

    /// <summary>
    /// Buy a custom amount
    /// </summary>
    /// <param name="amountType">Defines the meaning of the amount parameter</param>
    /// <param name="inputAmount">Amount for the amountType parameter</param>
    /// <param name="allowPartial">
    /// Throws an exception if the balance is insufficient for trade request if false.
    /// Otherwise, it uses whatever remaining balance there is.
    /// </param>
    /// <returns>Trade successfully executed?</returns>
    public bool Buy(AmountType amountType, decimal inputAmount, bool allowPartial = true)
        => Buy(new TradeInput(amountType, inputAmount, allowPartial));

    /// <summary>
    /// Buy a custom amount by directly specifying the TradeInput
    /// </summary>
    /// <param name="tradeInput">See documentation or use other parameters</param>
    /// <returns>Trade successfully executed?</returns>
    public bool Buy(TradeInput tradeInput)
        => SpotAccess.ExecuteBuy(State, tradeInput);

    /// <summary>
    /// Sell an amount as specified by the settings.
    /// </summary>
    /// <returns>Trade successfully executed?</returns>
    public bool Sell()
        => Sell(State.SetupConfig.DefaultSpotSellOrderSize);
    
    /// <summary>
    /// Sell a custom amount
    /// </summary>
    /// <param name="amountType">Defines the meaning of the amount parameter</param>
    /// <param name="inputAmount">Amount for the amountType parameter<</param>
    /// <param name="allowPartial">
    /// Throws an exception if the balance is insufficient for trade request if false.
    /// Otherwise, it uses whatever remaining balance there is.</param>
    /// <returns>Trade successfully executed?</returns>
    public bool Sell(AmountType amountType, decimal inputAmount, bool allowPartial = true)
        => Sell(new TradeInput(amountType, inputAmount, allowPartial));
    
    /// <summary>
    /// Sell a custom amount by directly specifying the TradeInput
    /// </summary>
    /// <param name="tradeInput">See documentation or use other parameters</param>
    /// <returns>Trade successfully executed?</returns>
    public bool Sell(TradeInput tradeInput)
        => SpotAccess.ExecuteSell(State, tradeInput);
}
