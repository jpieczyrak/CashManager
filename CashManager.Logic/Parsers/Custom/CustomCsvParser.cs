using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;
using CashManager.Data.Extensions;

using log4net;

namespace CashManager.Logic.Parsers.Custom
{
    public class CustomCsvParser : IParser
    {
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(CustomCsvParser)));
        private readonly Stock[] _stocks;

        public Rule[] Rules { get; private set; }
        public string ColumnSplitter { get; private set; }

        public Dictionary<Stock, Balance> Balances { get; } = new Dictionary<Stock, Balance>();

        public string Name { get; set; }

        public CustomCsvParser(Rule[] rules, Stock[] stocks = null, string columnSplitter = null)
        {
            Rules = rules.OrderBy(x => x.Property).ToArray();
            _stocks = stocks;
            ColumnSplitter = columnSplitter;
        }

        public Transaction[] Parse(string input, Stock userStock, Stock externalStock, TransactionType defaultOutcome,
            TransactionType defaultIncome, bool generateMissingStocks = false)
        {
            var output = new List<Transaction>();

            var lines = input.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                var elements = ColumnSplitter == null
                                   ? line.Count(x => x == ';') >= (Rules.Any() ? Rules.Max(x => x.Index) : 0)
                                         ? line.Split(';')
                                         : line.Split(new[] { line.Contains("\",\"") ? "\",\"" : "," }, StringSplitOptions.None)
                                   : line.Split(new[] { ColumnSplitter }, StringSplitOptions.None);
                elements = elements.Select(x => x.Replace("\"", string.Empty)).ToArray();
                var transaction = new Transaction(line.GenerateGuid())
                {
                    ExternalStock = externalStock,
                    UserStock = userStock
                };
                transaction.Positions.Add(new Position());
                if (userStock != null) Balances[userStock] = userStock.Balance;

                bool match = Rules.Any();
                foreach (var rule in Rules)
                {
                    if (!MatchRule(rule, elements, transaction, defaultIncome, defaultOutcome, userStock, generateMissingStocks))
                    {
                        match = false;
                        break;
                    }
                }

                if (match) output.Add(transaction);
            }

            return output.ToArray();
        }

        private bool MatchRule(Rule rule, string[] elements, Transaction transaction, TransactionType defaultIncome,
            TransactionType defaultOutcome, Stock defaultUserStock, bool generateMissingStocks)
        {
            if (elements.Length < rule.Column) return false;

            try
            {
                string stringValue = elements[rule.Index].Trim();
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
                        if (transaction.TransactionSourceCreationDate == DateTime.MinValue)
                            transaction.TransactionSourceCreationDate = transaction.BookDate;
                        break;
                    case TransactionField.CreationDate:
                        if (rule.IsOptional && string.IsNullOrWhiteSpace(stringValue)) transaction.TransactionSourceCreationDate = DateTime.Today;
                        else transaction.TransactionSourceCreationDate = DateTime.Parse(stringValue);
                        if (transaction.BookDate == DateTime.MinValue)
                            transaction.BookDate = transaction.TransactionSourceCreationDate;
                        break;
                    case TransactionField.PositionTitle:
                        transaction.Positions[0].Title = stringValue;
                        if (string.IsNullOrWhiteSpace(transaction.Positions[0].Title))
                        {
                            if (rule.IsOptional)
                                transaction.Positions[0].Title = "position 1";
                            else
                                return false;
                        }

                        break;
                    case TransactionField.Value:
                        {
                            decimal value = decimal.Parse(stringValue.Replace(".", ","));
                            transaction.Type = value >= 0 ? defaultIncome : defaultOutcome;
                            transaction.Positions[0].Value.GrossValue = Math.Abs(value);
                        }
                        break;
                    case TransactionField.ValueAsLost:
                        {
                            if (string.IsNullOrWhiteSpace(stringValue)) return true;
                            decimal value = decimal.Parse(stringValue.Replace(".", ","));
                            if (value == 0) return true;
                            if (value < 0) value = Math.Abs(value);
                            transaction.Type = defaultOutcome;
                            transaction.Positions[0].Value.GrossValue = Math.Abs(value);
                        }
                        break;
                    case TransactionField.ValueAsProfit:
                        {
                            if (string.IsNullOrWhiteSpace(stringValue)) return true;
                            decimal value = decimal.Parse(stringValue.Replace(".", ","));
                            if (value == 0) return true;
                            if (transaction.Positions[0].Value.GrossValue > 0 && transaction.Type != null)
                            {
                                decimal sum = transaction.Type.Outcome
                                                  ? value - transaction.Positions[0].Value.GrossValue
                                                  : value + transaction.Positions[0].Value.GrossValue;
                                transaction.Type = sum > 0m
                                                       ? defaultIncome
                                                       : defaultOutcome;
                                transaction.Positions[0].Value.GrossValue = Math.Abs(sum);
                            }
                            else
                            {
                                transaction.Type = value >= 0 ? defaultIncome : defaultOutcome;
                                transaction.Positions[0].Value.GrossValue = Math.Abs(value);
                            }
                        }
                        break;
                    case TransactionField.UserStock:
                        if (_stocks != null && _stocks.Any())
                        {
                            var matching = _stocks.FirstOrDefault(x => x.Name.ToLower().Equals(stringValue.ToLower()));
                            transaction.UserStock = matching;
                        }
                        if (transaction.UserStock == null || (!transaction.UserStock.Name?.Equals(stringValue) ?? true))
                        {
                            transaction.UserStock = generateMissingStocks
                                                        ? new Stock(stringValue.GenerateGuid())
                                                        {
                                                            Name = stringValue,
                                                            IsUserStock = true
                                                        }
                                                        : defaultUserStock;
                        }
                        break;
                    case TransactionField.Balance:
                        if (!Balances.ContainsKey(transaction.UserStock)) Balances[transaction.UserStock] = new Balance();
                        var balance = Balances[transaction.UserStock];
                        if (balance.LastEditDate < transaction.TransactionSourceCreationDate)
                        {
                            balance.Value = decimal.Parse(stringValue.Replace(".", ","));
                            balance.LastEditDate = transaction.TransactionSourceCreationDate;
                        }
                        break;
                    case TransactionField.Currency:
                        //todo:
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.Value.Debug("Parsing failed", e);
                return false;
            }

            return true;
        }
    }
}