namespace SimpleBacktestLib;

public enum TradeType
{
    NoAction = 0,
    SpotBuy = 1,
    SpotSell = 2,
    MarginLong = 3,
    MarginShort = 4,
    MarginCloseLong = 5,
    MarginCloseShort = 6,
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