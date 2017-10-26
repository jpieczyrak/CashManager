using System;

using Logic;
using Logic.Utils;

namespace CashManager
{
    public class MainWindowDataContext
    {
        public Wallet Wallet { get; set; }

        public TimeFrame Timeframe { get; set; }

        public MainWindowDataContext()
        {
            Wallet = new Wallet();
            Timeframe = new TimeFrame(DateTime.MinValue, DateTime.MinValue);
        }
    }
}