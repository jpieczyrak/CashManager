using System;
using System.Collections.ObjectModel;

using Logic;
using Logic.StocksManagement;
using Logic.Utils;

namespace CashManager
{
    public class MainWindowDataContext
    {
        public Wallet Wallet { get; set; }

        public ObservableCollection<StockStats> StockStats { get; set; }

        public TimeFrame Timeframe { get; set; }

        public MainWindowDataContext()
        {
            Wallet = new Wallet();
            StockStats = new ObservableCollection<StockStats>();
            Timeframe = new TimeFrame(DateTime.MinValue, DateTime.MinValue);
        }
    }
}