using Bogus;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public class CustomCsvParser : IParser
    {
        private readonly object _rules;

        public Balance Balance { get; private set; }

        public CustomCsvParser(Rule[] rules)
        {
            _rules = rules;
        }

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome,
            TransactionType defaultIncome)
        {
            input = input.Replace("\"", string.Empty);
            return new Transaction[] { };
        }
    }

    public class Rule
    {

    }
}