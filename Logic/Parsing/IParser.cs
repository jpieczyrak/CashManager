using System.Collections.Generic;

using Logic.Model;
using Logic.StocksManagement;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace Logic.Parsing
{
    public interface IParser
    {
        List<Transaction> Parse(string input, Stock userStock);
    }
}