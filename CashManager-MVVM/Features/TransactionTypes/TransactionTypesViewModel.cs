using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.TransactionTypes;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager_MVVM.Features.TransactionTypes
{
    public class TransactionTypesViewModel : ViewModelBase
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public TrulyObservableCollection<TransactionType> TransactionTypes { get; }

        public RelayCommand AddTransactionTypeCommand { get; set; }

        public RelayCommand<TransactionType> RemoveCommand { get; set; }

        public TransactionTypesViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;

            var transactionTypes = Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, CashManager.Data.DTO.TransactionType[]>(new TransactionTypesQuery()));
            TransactionTypes = new TrulyObservableCollection<TransactionType>(transactionTypes);
            TransactionTypes.CollectionChanged += TransactionTypesOnCollectionChanged;

            AddTransactionTypeCommand = new RelayCommand(() => { TransactionTypes.Add(new TransactionType()); });
            RemoveCommand = new RelayCommand<TransactionType>(x =>
            {
                _commandDispatcher.Execute(new DeleteTransactionTypeCommand(Mapper.Map<CashManager.Data.DTO.TransactionType>(x)));
                TransactionTypes.Remove(x);
            });
        }

        private void TransactionTypesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var transactionTypes = TransactionTypes.Select(Mapper.Map<CashManager.Data.DTO.TransactionType>).ToArray();
            _commandDispatcher.Execute(new UpsertTransactionTypesCommand(transactionTypes));
        }
    }
}