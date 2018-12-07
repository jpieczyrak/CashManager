using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    public class Summary : ObservableObject
    {
        private double _grossIncome;
        private double _grossOutcome;
        private double _grossBalance;

        public double GrossIncome
        {
            get => _grossIncome;
            set
            {
                Set(nameof(GrossIncome), ref _grossIncome, value);
                GrossBalance = GrossIncome - GrossOutcome;
            }
        }

        public double GrossOutcome
        {
            get => _grossOutcome;
            set
            {
                Set(nameof(GrossOutcome), ref _grossOutcome, value);
                GrossBalance = GrossIncome - GrossOutcome;
            }
        }

        public double GrossBalance
        {
            get => _grossBalance;
            set => Set(nameof(GrossBalance), ref _grossBalance, value);
        }
    }
}