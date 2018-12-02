using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public interface IParser
    {
        Transaction[] Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome, TransactionType defaultIncome);

        Balance Balance { get; }
    }
}