# Types: TradeInput \& Fee
These types are used to configure the amount and fees of trades.

## TradeInput
- `AmountType` - The type of amount to use. See below for more information.
- `Amount` - The amount to use as configured by `AmountType`.
- `AllowPartial` - Whether to allow partial trades. If `false`, the backtest will throw an exception if the balance is insufficient to execute the trade.

## Fee
- `AmountType` - The type of amount to use. See below for more information.
- `Amount` - The amount to use as configured by `AmountType`.
- `FeeSource` - The source of the fee. See below for more information.

## AmountType Explained
`AmountType` affects how the `Amount` property is interpreted.  
TradeInput and Fee share the common concept of `AmountType` which can be:

- `AmountType.Max` - The `Amount` property is ignored and the full balance is used.
- `AmountType.Absolute` - The `Amount` property is interpreted as an absolute value. If the balance is insufficient, we use the full balance instead, unless configured otherwise.
- `AmountType.Percentage` - The `Amount` property is interpreted as a percentage of the balance. This must be a value between 0 and 100.


## FeeSource Explained
Fee has a `FeeSource` property which can be:
- `FeeSource.Base`
- `FeeSource.Quote`
- `FeeSource.Input`
- `FeeSource.Output`
- `FeeSource.Both`

Depending on the `FeeSource`, the fee is applied to the base, quote, input, output or split between both currencies of the trade.