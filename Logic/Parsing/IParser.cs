using System.Collections.Generic;
using System.Collections.ObjectModel;
using Logic.StocksManagement;
using Logic.TransactionManagement;

namespace Logic.Parsing
{
    public interface IParser
    {
        List<Transaction> Parse(string input, ObservableCollection<Stock> stocks);
    }
}