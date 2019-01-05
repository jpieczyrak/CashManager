using System;
using System.Collections.Generic;

using CashManager.Data.DTO;
using CashManager.Data.Extensions;

namespace CashManager.Logic.Parsers.Custom
{
    public class CustomCsvParser : IParser
    {
        private readonly Rule[] _rules;

        public Balance Balance { get; private set; }

        public CustomCsvParser(Rule[] rules)
        {
            _rules = rules;
        }

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome,
            TransactionType defaultIncome)
        {
            var output = new List<Transaction>();
            input = input.Replace("\"", string.Empty);

            var lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                var transaction = new Transaction(line.GenerateGuid());
                foreach (var rule in _rules)
                {
                    if (!rule.IsOptional && !rule.Match(line, transaction)) continue;

                    output.Add(transaction);
                }
            }

            return output.ToArray();
        }
    }
}