using GalaSoft.MvvmLight;

namespace CashManager.Model
{
    public class TransactionsSummary : ObservableObject
    {
        private decimal _grossIncome;
        private decimal _grossOutcome;
        private decimal _grossBalance;

        public decimal GrossIncome
        {
            get => _grossIncome;
            set
            {
                Set(nameof(GrossIncome), ref _grossIncome, value);
                GrossBalance = GrossIncome - GrossOutcome;
            }
        }

        public decimal GrossOutcome
        {
            get => _grossOutcome;
            set
            {
                Set(nameof(GrossOutcome), ref _grossOutcome, value);
                GrossBalance = GrossIncome - GrossOutcome;
            }
        }

        public decimal GrossBalance
        {
            get => _grossBalance;
            set => Set(nameof(GrossBalance), ref _grossBalance, value);
        }

        public string Name { get; set; }

        private int _incomesCount;

        public int IncomesCount
        {
            get => _incomesCount;
            set
            {
                Set(ref _incomesCount, value);
                TotalCount = IncomesCount + OutcomesCount;
            }
        }

        private int _outcomesCount;

        public int OutcomesCount
        {
            get => _outcomesCount;
            set
            {
                Set(ref _outcomesCount, value);
                TotalCount = IncomesCount + OutcomesCount;
            }
        }

        private int _totalCount;

        public int TotalCount
        {
            get => _totalCount;
            private set => Set(ref _totalCount, value);
        }

        public TransactionsSummary Copy()
        {
            return new TransactionsSummary
            {
                GrossIncome = GrossIncome,
                GrossOutcome = GrossOutcome,
                GrossBalance = GrossBalance,
                IncomesCount =  IncomesCount,
                OutcomesCount = OutcomesCount
            };
        }
    }
}