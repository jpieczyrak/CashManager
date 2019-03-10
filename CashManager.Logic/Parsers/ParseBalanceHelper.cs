using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers
{
    public static class ParseBalanceHelper
    {
        public static void AdjustMissingBalances(ICollection<Transaction> output, Dictionary<Stock, Dictionary<DateTime, decimal>> balances)
        {
            if (balances.Any(x => x.Value.Any()))
            {
                foreach (var balance in balances.Where(x => x.Value.Any()))
                {
                    var matchingTransactions = output.Where(x => x.UserStock.Id == balance.Key.Id).ToArray();
                    var transactionsWithoutBalance = matchingTransactions.Where(x => !x.Notes.Any(y => y.Contains(" Saldo: "))).ToArray();
                    if (matchingTransactions.Length != transactionsWithoutBalance.Length && transactionsWithoutBalance.Any())
                    {
                        decimal value = transactionsWithoutBalance.Sum(x => x.Positions.Sum(y => y.Value.GrossValue) * (x.Type.Income ? 1m : -1m));
                        balance.Value[transactionsWithoutBalance.Max(x => x.BookDate)] = balance.Value.OrderByDescending(x => x.Key).First().Value + value;
                    }
                }
            }
        }
    }
}