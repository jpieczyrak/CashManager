using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Data.Extensions;

namespace CashManager.Logic.Parsers.Custom
{
    public class CustomCsvParser : IParser
    {
        private readonly Rule[] _rules;
        private readonly Stock[] _stocks;

        public Balance Balance { get; private set; }

        public CustomCsvParser(Rule[] rules, Stock[] stocks = null)
        {
            _rules = rules;
            _stocks = stocks;
            Balance = new Balance();
        }

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome,
            TransactionType defaultIncome)
        {
            var output = new List<Transaction>();
            input = input.Replace("\"", string.Empty);

            var lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                var transaction = new Transaction(line.GenerateGuid())
                {
                    ExternalStock = externalStock,
                    UserStock = userStock
                };
                transaction.Positions.Add(new Position());

                bool match = _rules.Any();
                foreach (var rule in _rules)
                {
                    if (!MatchRule(rule, line, transaction, defaultIncome, defaultOutcome))
                    {
                        match = false;
                        break;
                    }
                }

                if (match) output.Add(transaction);
            }

            return output.ToArray();
        }

        private bool MatchRule(Rule rule, string line, Transaction transaction, TransactionType defaultIncome,
            TransactionType defaultOutcome)
        {
            var elements = line.Split(';');
            if (elements.Length < rule.Column) return false;

            try
            {
                string stringValue = elements[rule.Index];
                switch (rule.Property)
                {
                    case TransactionField.Title:
                        transaction.Title = stringValue;
                        if (string.IsNullOrWhiteSpace(transaction.Title)) return false;
                        break;
                    case TransactionField.Note:
                        transaction.Note = stringValue;
                        if (string.IsNullOrWhiteSpace(transaction.Note) && !rule.IsOptional) return false;
                        break;
                    case TransactionField.BookDate:
                        if (rule.IsOptional && string.IsNullOrWhiteSpace(stringValue)) transaction.BookDate = DateTime.Today;
                        else transaction.BookDate = DateTime.Parse(stringValue);
                        break;
                    case TransactionField.CreationDate:
                        if (rule.IsOptional && string.IsNullOrWhiteSpace(stringValue)) transaction.TransactionSourceCreationDate = DateTime.Today;
                        else transaction.TransactionSourceCreationDate = DateTime.Parse(stringValue);
                        break;
                    case TransactionField.PositionTitle:
                        transaction.Positions[0].Title = stringValue;
                        if (string.IsNullOrWhiteSpace(transaction.Positions[0].Title) && !rule.IsOptional) return false;
                        else transaction.Positions[0].Title = "position 1";
                        break;
                    case TransactionField.Value:
                        decimal value = decimal.Parse(stringValue);
                        transaction.Type = value >= 0 ? defaultIncome : defaultOutcome;
                        transaction.Positions[0].Value.GrossValue = Math.Abs(value);
                        break;
                    case TransactionField.UserStock:
                        if (_stocks != null)
                        {
                            var matching = _stocks.FirstOrDefault(x => x.Name.ToLower().Equals(stringValue.ToLower()));
                            if (matching != null)
                                transaction.UserStock = matching;
                            else
                                return false;
                        }
                        break;
                    case TransactionField.Balance:
                        Balance.Value = decimal.Parse(stringValue);
                        //todo: get date etc
                        break;
                    case TransactionField.Currency:
                        //todo:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

            return true;
        }
    }
}