# Type: BacktestCandle

The `BacktestCandle` class is a record that represents a single candle in a backtest. It contains the following properties:

- `Time`: The timestamp of the candle, represented as a `DateTime` object.
- `Open`: The opening price of the candle.
- `High`: The highest price of the candle.
- `Low`: The lowest price of the candle.
- `Close`: The closing price of the candle.
- `Volume`: The volume of the candle. Should be set to `0` if not available.

All properties are of type `decimal` and have both a getter and an init-only setter, meaning that they can be set only during object initialization and cannot be modified afterwards.
