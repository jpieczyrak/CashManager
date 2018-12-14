using System.Collections.Specialized;
using System.Linq;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.Transactions
{
    public class PositionListViewModel : ViewModelBase
    {
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

        public RelayCommand PositionEditCommand => new RelayCommand(() => { }, () => false); //todo:

        public Transaction SelectedPosition { get; set; }

        public Summary Summary { get; set; }

        public PositionListViewModel()
        {
            Summary = new Summary();
            Positions = new TrulyObservableCollection<Position>();
        }

        private void PositionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Summary.GrossIncome = Positions.Where(x => x.Income).Sum(x => x.Value.GrossValue);
            Summary.GrossOutcome = Positions.Where(x => x.Outcome).Sum(x => x.Value.GrossValue);
        }
    }
}