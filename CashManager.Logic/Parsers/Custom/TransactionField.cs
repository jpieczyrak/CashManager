namespace CashManager.Logic.Parsers.Custom
{
    public enum TransactionField
    {
        Title,
        Note,
        BookDate,
        CreationDate,
        PositionTitle,
        Value,
        ValueAsLost,
        ValueAsProfit,
        UserStock,
        Currency,

        Balance = int.MaxValue,
    }
}