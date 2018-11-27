using System.Collections.Generic;

using LogicOld.Model;

namespace LogicOld.Parsing
{
    public interface IParser
    {
        List<Transaction> Parse(string input, Stock userStock, Stock externalStock);
    }
}