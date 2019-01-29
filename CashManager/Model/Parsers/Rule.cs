using CashManager.Logic.Parsers.Custom;

using GalaSoft.MvvmLight;

namespace CashManager.Model.Parsers
{
    public class Rule : ObservableObject
    {
        private TransactionField _property;
        private int _column;
        private bool _isOptional = true;

        public TransactionField Property
        {
            get => _property;
            set => Set(ref _property, value);
        }

        public int Column
        {
            get => _column;
            set => Set(ref _column, value);
        }

        public bool IsOptional
        {
            get => _isOptional;
            set => Set(ref _isOptional, value);
        }
    }
}