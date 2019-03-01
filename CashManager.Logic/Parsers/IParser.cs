using System;
using System.Collections.Generic;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public interface IParser
    {
        Transaction[] Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome, TransactionType defaultIncome, bool generateMissingStocks = false);

        Dictionary<Stock, Dictionary<DateTime, decimal>> Balances { get; }
    }
}