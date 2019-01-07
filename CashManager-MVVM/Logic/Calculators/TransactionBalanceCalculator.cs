using System;
using System.Collections.Generic;
using System.Linq;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Selectors;

using OxyPlot;
using OxyPlot.Axes;

namespace CashManager_MVVM.Logic.Calculators
{
    public class TransactionBalanceCalculator
    {
        public DataPoint[] GetWealthValues(IEnumerable<Transaction> transactions, Stock[] selectedStocks, DateFrame dateFilter, Func<Transaction, DateTime> groupingSelector)
        {
            if (selectedStocks == null || !selectedStocks.Any()) return new DataPoint[0];
            if (transactions == null || !transactions.Any()) return new DataPoint[0];

            var values = CalculateBalance(transactions, selectedStocks, dateFilter, groupingSelector);

            return values
                   .Select(x => new DataPoint(DateTimeAxis.ToDouble(x.BookDate), (double) x.Value))
                   .OrderBy(x => x.X)
                   .ToArray();
        }

        public IEnumerable<TransactionBalance> CalculateBalance(IEnumerable<Transaction> transactions, Stock[] selectedStocks, DateFrame dateFilter, Func<Transaction, DateTime> groupingSelector)
        {
            var stockDate = selectedStocks.Max(x => x.LastEditDate).Date;
            decimal stockValue = selectedStocks.Sum(x => x.Balance.Value);

            var firstMatch = transactions
                             .Where(x => selectedStocks.Contains(x.UserStock))
                             .GroupBy(groupingSelector);
            decimal transactionsValueBeforeLastStockUpdate =
                firstMatch.Sum(x => x.Where(z => z.BookDate <= stockDate).Sum(y => y.ValueAsProfit));
            decimal startValue = stockValue - transactionsValueBeforeLastStockUpdate;
            var firstTransactionBookDate = transactions.Min(x => x.BookDate).Date;
            var values = firstMatch
                         .Where(x => x.Key <= stockDate)
                         .OrderBy(x => x.Key)
                         .Select(x =>
                         {
                             startValue += x.Sum(y => y.ValueAsProfit);
                             return new TransactionBalance(x.Key, startValue);
                         })
                         .Where(x => !dateFilter.IsChecked
                                     || x.BookDate >= dateFilter.From && x.BookDate <= dateFilter.To)
                         .Concat(!dateFilter.IsChecked || firstTransactionBookDate > dateFilter.From
                                     ? new[] { new TransactionBalance(firstTransactionBookDate.AddDays(-1), startValue) }
                                     : new TransactionBalance[0])
                         .Concat(!dateFilter.IsChecked || stockDate.Date <= dateFilter.To
                                     ? new[] { new TransactionBalance(stockDate, stockValue) }
                                     : new TransactionBalance[0]);
            return values;
        }
    }
}