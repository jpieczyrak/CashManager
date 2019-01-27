using System.Collections.Specialized;
using System.Linq;

using CashManager.WPF.Features.Main;
using CashManager.WPF.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager.WPF.Features.Transactions.Positions
{
    public class PositionListViewModel : ViewModelBase
    {
        private readonly ViewModelFactory _factory;
        private TrulyObservableCollection<Position> _positions;

        public TrulyObservableCollection<Position> Positions
        {
            get => _positions;
            set
            {
                if (_positions != null) _positions.CollectionChanged -= PositionsOnCollectionChanged;
                _positions = value;
                _positions.CollectionChanged += PositionsOnCollectionChanged;
                PositionsOnCollectionChanged(this, null);
            }
        }

        public RelayCommand TransactionEditCommand => new RelayCommand(TransactionEdit);

        public Position SelectedPosition { get; set; }

        public TransactionsSummary Summary { get; set; }

        public PositionListViewModel(ViewModelFactory factory)
        {
            _factory = factory;
            Summary = new TransactionsSummary();
            Positions = new TrulyObservableCollection<Position>();
        }

        private void PositionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            var incomes = Positions.Where(x => x.Income).ToArray();
            var outcomes = Positions.Where(x => x.Outcome).ToArray();
            Summary.GrossIncome = incomes.Sum(x => x.Value.GrossValue);
            Summary.GrossOutcome = outcomes.Sum(x => x.Value.GrossValue);
            Summary.IncomesCount = incomes.Length;
            Summary.OutcomesCount = outcomes.Length;
        }

        private void TransactionEdit()
        {
            var applicationViewModel = _factory.Create<ApplicationViewModel>();
            var transactionViewModel = _factory.Create<TransactionViewModel>();
            transactionViewModel.Transaction = SelectedPosition.Parent;
            applicationViewModel.SelectViewModelCommand.Execute(ViewModel.Transaction);
        }
    }
}