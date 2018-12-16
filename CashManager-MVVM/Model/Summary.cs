﻿using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model
{
    public class Summary : ObservableObject
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
    }
}