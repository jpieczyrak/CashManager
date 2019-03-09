using System.Collections.Specialized;
using System.Linq;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Features.Main;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Model;
using CashManager.Properties;
using CashManager.UserCommunication;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager.Features.Transactions
{
    public class TransactionListViewModel : ViewModelBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly ViewModelFactory _factory;
        private readonly TransactionsProvider _provider;
        private readonly IMessagesService _messagesService;
        private TrulyObservableCollection<Transaction> _transactions;

        public TrulyObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set
            {
                if (_transactions != null) _transactions.CollectionChanged -= TransactionsOnCollectionChanged;
                _transactions = value;
                _transactions.CollectionChanged += TransactionsOnCollectionChanged;
                TransactionsOnCollectionChanged(this, null);
            }
        }

        public RelayCommand TransactionEditCommand => new RelayCommand(TransactionEdit, () => true);
        public RelayCommand TransactionDeleteCommand => new RelayCommand(ExecuteTransactionDelete, CanExecuteTransactionDelete);

        public RelayCommand DuplicateTransactionCommand => new RelayCommand(TransactionDuplicate, CanExecuteTransactionDuplicate);

        public Transaction SelectedTransaction { get; set; }

        public TransactionsSummary Summary { get; set; }

        public TransactionListViewModel()
        {
            Summary = new TransactionsSummary();
            Transactions = new TrulyObservableCollection<Transaction>();
        }

        public TransactionListViewModel(ICommandDispatcher commandDispatcher, ViewModelFactory factory, TransactionsProvider provider, IMessagesService messagesService) : this()
        {
            _commandDispatcher = commandDispatcher;
            _factory = factory;
            _provider = provider;
            _messagesService = messagesService;
        }

        private void TransactionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var incomes = Transactions.Where(x => x.Type.Income).ToArray();
            var outcomes = Transactions.Where(x => x.Type.Outcome).ToArray();
            Summary.GrossIncome = incomes.Sum(x => x.ValueAsProfit);
            Summary.GrossOutcome = -outcomes.Sum(x => x.ValueAsProfit);
            Summary.IncomesCount = incomes.Length;
            Summary.OutcomesCount = outcomes.Length;
        }

        private void TransactionEdit()
        {
            if (SelectedTransaction == null || _factory == null) return;
            var applicationViewModel = _factory.Create<ApplicationViewModel>();
            applicationViewModel.TransactionViewModel.Value.Transaction = SelectedTransaction;
            applicationViewModel.SelectViewModelCommand.Execute(ViewModel.Transaction);
        }

        private void TransactionDuplicate()
        {
            var transaction = Transaction.Copy(SelectedTransaction);
            var dto = Mapper.Map<Data.DTO.Transaction>(transaction);

            _commandDispatcher.Execute(new UpsertTransactionsCommand(dto));
            _provider.AllTransactions.Add(transaction);
            Transactions.Add(transaction);
        }

        private bool CanExecuteTransactionDuplicate() => SelectedTransaction != null
                                                         && _provider != null
                                                         && _commandDispatcher != null;

        private void ExecuteTransactionDelete()
        {
            if (Settings.Default.QuestionForTransactionDelete)
                if (!_messagesService.ShowQuestionMessage(Strings.Question, string.Format(Strings.QuestionDoYouWantToRemoveTransactionFormat, SelectedTransaction.Title)))
                    return;

            var dto = Mapper.Map<Data.DTO.Transaction>(SelectedTransaction);
            _commandDispatcher.Execute(new DeleteTransactionCommand(dto));

            _provider.AllTransactions.Remove(SelectedTransaction);
            Transactions.Remove(SelectedTransaction);
        }

        private bool CanExecuteTransactionDelete() => SelectedTransaction != null;
    }
}