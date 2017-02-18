using System;
using System.Collections.ObjectModel;
using Logic;
using Logic.StocksManagement;
using Logic.TransactionManagement;
using Logic.TransactionManagement.TransactionElements;

namespace CashManager
{
    public class MainWindowDataContext
    {
        public Wallet Wallet { get; set; }
        public ObservableCollection<StockStats> StockStats { get; set; } = new ObservableCollection<StockStats>();
        public TimeFrame Timeframe { get; set; } = new TimeFrame(DateTime.MinValue, DateTime.MinValue);
    }
}