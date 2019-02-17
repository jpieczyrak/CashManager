using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.TransactionTypes;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.TransactionTypes;
using CashManager.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoType = CashManager.Data.DTO.TransactionType;

namespace CashManager.Features.TransactionTypes
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

            var query = new TransactionTypesQuery();
            var types = queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(query);
            var transactionTypes = Mapper.Map<TransactionType[]>(types);
            TransactionTypes = new TrulyObservableCollection<TransactionType>(transactionTypes);
            TransactionTypes.CollectionChanged += TransactionTypesOnCollectionChanged;

            AddTransactionTypeCommand = new RelayCommand(() => { TransactionTypes.Add(new TransactionType { Outcome = true }); });
            RemoveCommand = new RelayCommand<TransactionType>(x =>
            {
                _commandDispatcher.Execute(new DeleteTransactionTypeCommand(Mapper.Map<DtoType>(x)));
                TransactionTypes.Remove(x);
            });
        }

        private void TransactionTypesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var transactionTypes = TransactionTypes.Select(Mapper.Map<DtoType>).ToArray();
            _commandDispatcher.Execute(new UpsertTransactionTypesCommand(transactionTypes));
        }
    }
}