using System.Collections.Generic;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public interface IParser
    {
        List<Transaction> Parse(string input, Stock userStock, Stock externalStock,
            TransactionType defaultOutcome, TransactionType defaultIncome);
    }
}