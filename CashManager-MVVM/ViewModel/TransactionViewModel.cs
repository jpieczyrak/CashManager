using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CashManager_MVVM.Model;
using CashManager_MVVM.Model.DataProviders;
using CashManager_MVVM.View;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Logic.TransactionManagement.TransactionElements;

namespace CashManager_MVVM.ViewModel
{
    public class TransactionViewModel : ViewModelBase
    {
        private IEnumerable<Stock> _stocks;
        private Transaction _transaction;

        public IEnumerable<eTransactionType> TransactionTypes => Enum.GetValues(typeof(eTransactionType)).Cast<eTransactionType>();

        public Transaction Transaction
        {
            get => _transaction;
            set => Set(nameof(Transaction), ref _transaction, value);
        }

        public IEnumerable<Stock> ExternalStocks => _stocks.Where(x => !x.IsUserStock);

        public IEnumerable<Stock> UserStocks => _stocks.Where(x => x.IsUserStock);

        public RelayCommand<Subtransaction> ChooseCategoryCommand { get; set; }

        public TransactionViewModel(IDataService dataService)
        {
            dataService.GetStocks((stocks, exception) => { _stocks = stocks; });
            ChooseCategoryCommand = new RelayCommand<Subtransaction>(subtransaction =>
            {
                var window = new CategoryPickerView(subtransaction.Category);
                window.Show();
                window.Closing += (sender, args) => { subtransaction.Category = window.treeView.SelectedItem as Category; };
            });
        }
    }
}