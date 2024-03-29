﻿namespace SimpleBacktestLib;

/// <summary>
/// Defines the asset type of a trade.
/// </summary>
public enum TradeType
{
    SpotBuy = 1,
    SpotSell = 2,
    MarginLong = 3,
    MarginShort = 4,
    MarginCloseLong = 5,
    MarginCloseShort = 6,
}

/// <summary>
/// Simple definition of a trade operation.
/// For more specific options see TradeType
/// </summary>
public enum TradeOperation
{
    UnspecifiedOrNotApplicable = 0,
    Buy = 1,
    Sell = 2,
}


public enum AssetType
{
    None = 0,
    Base = 1,
    Quote = 2,
}

/// <summary>
/// Value is represented in seconds
/// </summary>
public enum Timeframe
{
    OneSec = 1,
    FiveSec = 5,
    FifteenSec = 15,
    ThirtySec = 30,
    OneMin = 60,
    FiveMin = 300,
    FifteenMin = 900,
    ThirtyMin = 1800,
    OneHour = 3600,
    FourHour = 14400,
    OneDay = 86400,
    OneWeek = 604800,
    OneMonth = 2592000,
    OneYear = 31536000
}

/// <summary>
/// Can be used to define at which pricepoint during the lifetime of a candle to buy.
/// Used by BacktestCandle.GetPrice()
/// </summary>
public enum PriceTime
{
    AtOpen = 0,
    AtClose = 1,
    AtHigh = 2,
    AtLow = 3,
    AtRandom = 4,
}

/// <summary>
/// Status of a trade request or evaluated trade.
/// </summary>
public enum TradeStatus
{
    Pending = 0,
    Executed = 1,
    Cancelled = 2,
}

/// <summary>
/// Define the meaning of a user-specified amount for trading.
/// </summary>
public enum AmountType
{
    Max = 0, // Trades the entire available balance. Amount is ignored.
    Absolute = 1, // Specify a fixed amount to trade
    Percentage, // Specify a percentage of the remaining available balance to trade 
}

/// <summary>
/// Source from which to take the trading fee.
/// </summary>
public enum FeeSource
{
    Base = 0,
    Quote = 1,
    Input = 2,
    Output = 3,
    Both = 4,
}