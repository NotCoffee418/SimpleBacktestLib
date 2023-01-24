namespace SimpleBacktestLib.TradingManagers;

/// <summary>
/// Provides shortcut functions for trading to Tick().
/// Should be called only through TickData.Trade.
/// </summary>
public class CommonTradeManager
{
    /// <summary>
    /// Should not be instantiated externally.
    /// These functions can be called through Tick()'s Trade property.
    /// </summary>
    internal CommonTradeManager(BacktestState state)
    {
        Spot = new(state);
        Margin = new(state);
    }

    /// <summary>
    /// Provides access to Spot trading methods.
    /// </summary>
    public SpotManager Spot { get; }

    
    /// <summary>
    /// Provides access to Margin trading methods
    /// </summary>
    public MarginManager Margin { get; }
}
