using CashManager_MVVM.Model.Selectors;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Transactions
{
    public class MassReplacerViewModel : ViewModelBase
    {
        private readonly ViewModelFactory _factory;
        private TextSelector _titleSelector;

        public TransactionSearchViewModel TransactionsSearchViewModel { get; private set; }

        public TextSelector TitleSelector
        {
            get => _titleSelector;
            set => Set(nameof(TitleSelector), ref _titleSelector, value);
        }

        public MassReplacerViewModel(ViewModelFactory factory)
        {
            TitleSelector = new TextSelector("Title");
            _factory = factory;
            TransactionsSearchViewModel = _factory.Create<TransactionSearchViewModel>();
        }
    }
}