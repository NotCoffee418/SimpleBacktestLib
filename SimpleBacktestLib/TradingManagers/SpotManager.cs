using SimpleBacktestLib.Models;

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
    public BacktestState State { get; }

    /// <summary>
    /// Buy an amount as specified by the settings.
    /// </summary>
    public void Buy()
    {
        // Extract trade amount
    }

    /// <summary>
    /// Buy a custom amount
    /// </summary>
    /// <param name="amountType">Defines the meaning of the amount parameter</param>
    /// <param name="amount">Amount for the amountType parameter</param>
    /// <param name="allowPartial">
    /// Throws an exception if the balance is insufficient for trade request if false.
    /// Otherwise, it uses whatever remaining balance there is.
    /// </param>
    public void Buy(AmountType amountType, decimal inputAmount, bool allowPartial = true)
        => Buy(new TradeInput(amountType, inputAmount, allowPartial));

    /// <summary>
    /// Buy a custom amount by directly specifying the TradeInput
    /// </summary>
    /// <param name="tradeInput"></param>
    public void Buy(TradeInput tradeInput)
    {

    }



    public void Sell()
    {
        throw new NotImplementedException();
    }
}
