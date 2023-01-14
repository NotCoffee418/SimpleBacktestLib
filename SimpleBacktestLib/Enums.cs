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