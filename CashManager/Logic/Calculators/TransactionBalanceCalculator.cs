using System;
using System.Collections.Generic;
using System.Linq;

using CashManager.Model;
using CashManager.Model.Selectors;

using OxyPlot;
using OxyPlot.Axes;

namespace CashManager.Logic.Calculators
{
    public class TransactionBalanceCalculator
    {
        public DataPoint[] GetWealthValues(IEnumerable<Transaction> transactions, Stock[] selectedStocks, DateFrameSelector dateFilter, Func<Transaction, DateTime> groupingSelector, bool showTransfers)
        {
            if (selectedStocks == null || !selectedStocks.Any()) return new DataPoint[0];
            if (transactions == null || !transactions.Any()) return new DataPoint[0];

            var values = CalculateBalance(transactions, selectedStocks, dateFilter, groupingSelector, showTransfers);

            return values
                   .Select(x => new DataPoint(DateTimeAxis.ToDouble(x.BookDate), (double) x.Value))
                   .OrderBy(x => x.X)
                   .ToArray();
        }

        public IEnumerable<TransactionBalance> CalculateBalance(IEnumerable<Transaction> transactions, Stock[] selectedStocks, DateFrameSelector dateFilter,
            Func<Transaction, DateTime> groupingSelector, bool showTransfers)
        {
            var stockDate = selectedStocks.Max(x => x.Balance.BookDate).Date;
            if (stockDate == DateTime.MinValue) stockDate = DateTime.Today;
            decimal stockValue = selectedStocks.Sum(x => x.Balance.Value);

            var firstMatch = transactions
                             .Where(x => selectedStocks.Contains(x.UserStock));
            decimal transactionsValueBeforeLastStockUpdate =
                firstMatch.Where(z => z.BookDate <= stockDate).Sum(y => y.ValueAsProfit);
            decimal startValue = stockValue - transactionsValueBeforeLastStockUpdate;
            var firstTransactionBookDate = transactions.Min(x => x.BookDate).Date;
            var values = firstMatch
                         .GroupBy(groupingSelector)
                         .Where(x => x.Key <= stockDate)
                         .OrderBy(x => x.Key)
                         .Select(x =>
                         {
                             startValue += showTransfers ? x.Sum(y => y.ValueWithSign) : x.Sum(y => y.ValueAsProfit);
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