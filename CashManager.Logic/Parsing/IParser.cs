using System.Collections.Generic;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsing
{
    public interface IParser
    {
        List<Transaction> Parse(string input, Stock userStock, Stock externalStock);
    }
}