using SimpleBacktestLib.Internal.Margin;

namespace SimpleBacktestLib;

/// <summary>
/// State at a given point in the backtest. Modified by the <see cref="Engine"/> class.
/// </summary>
public class BacktestState
{
    /// <summary>
    /// Current balance of base asset, excluding margin positions.
    /// </summary>
    public decimal BaseBalance { get; internal set; }

    /// <summary>
    /// Current balance of quote asset, excluding margin positions.
    /// </summary>
    public decimal QuoteBalance { get; internal set; }

    /// <summary>
    /// Describes an open margin position
    /// </summary>
    public MarginPosition? ActiveMarginPosition { get; internal set; } = null;    
}
