using System.Collections.Generic;

using Logic.Model;

namespace Logic.Parsing
{
    public interface IParser
    {
        List<Transaction> Parse(string input, Stock userStock, Stock externalStock);
    }
}