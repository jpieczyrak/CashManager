using GalaSoft.MvvmLight;

using OxyPlot;

namespace CashManager_MVVM.Features.Summary
{
    public class SummaryViewModel : ViewModelBase
    {
        private DataPoint[] _balanceSource;

        public DataPoint[] BalanceSource
        {
            get => _balanceSource;
            set => Set(ref _balanceSource, value);
        }
    }
}