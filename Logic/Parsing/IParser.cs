using System.Collections.Generic;
using Logic.StocksManagement;
using Logic.TransactionManagement;

namespace Logic.Parsing
{
    public interface IParser
    {
        List<Transaction> Parse(string input, Stock userStock);
    }
}