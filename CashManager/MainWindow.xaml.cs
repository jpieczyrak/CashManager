using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Logic.FindingFilters;
using Logic.LogicObjectsProviders;
using Logic.Mapping;
using Logic.Model;
using Logic.Utils;

namespace CashManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowDataContext _dataContext = new MainWindowDataContext();
        public MainWindow()
        {
            MapperConfiguration.Configure();

            _dataContext.Timeframe = new TimeFrame(DateTime.Now.AddYears(-5), DateTime.Now);

            InitializeComponent();
            Title += " " + Assembly.GetExecutingAssembly().GetName().Version;

            AddHandler(Keyboard.KeyDownEvent, (KeyEventHandler)HandleKeyDownEvent);

            DataContext = _dataContext;
            DataGridTransactions.ItemsSource = _dataContext.Wallet.Transactions;

            //AnswerMyQuestions();
        }

        private static void AnswerMyQuestions()
        {
            //todo: remove
            //pytania jejka - na razie bez gui:
            string output = "";
            var trans = TransactionProvider.Transactions.OrderByDescending(x => x.BookDate);

            output += "Obiady:\r\n";
            var dinners = trans.Where(x => x.Title != null && x.Title.Contains("Obiad")).OrderByDescending(x => x.BookDate);
            var grouped = dinners.GroupBy(t => $"{t.BookDate.Month}.{t.BookDate.Year}");
            output += $"Avg: {grouped.Select(x => x.Sum(y => y.Value)).Average()}\r\n";
            foreach (var @group in grouped)
            {
                output += $"{@group.Key,8} : {@group.Sum(x => x.Value)}\r\n";
            }

            output += "Wyplaty:\r\n";
            var wyplaty = trans.Where(x => x.Title != null && x.Title.ToLower().Contains("wynagrodzenie")).OrderByDescending(x => x.BookDate);
            foreach (var transaction in wyplaty)
            {
                output = output + $"{transaction.BookDate:yyyy-MM-dd} : {transaction.ValueAsProfit}\r\n";
            }

            output += "Bilans:\r\n";
            var bymonth = trans.GroupBy(t => $"{t.BookDate.Month}.{t.BookDate.Year}");
            var avgBilans = bymonth.Average(x => x.Sum(y => y.ValueAsProfit));
            foreach (var m in bymonth)
            {
                var costs = m.Where(x => x.ValueAsProfit < 0).OrderByDescending(x => x.BookDate);
                var incoms = m.Where(x => x.ValueAsProfit > 0).OrderByDescending(x => x.BookDate);
                double income = incoms.Sum(x => x.Value);
                double outcome = costs.Sum(x => x.Value);
                output += $"{m.Key,8:0.#} : +{income,8:0.#}\t{-outcome,8:0.#}\t{income - outcome,8:0.#}\tavg:\t{avgBilans,8:0.#}\r\n";
            }

            output += "Bilans progresywyny:\r\n";
            bymonth = trans.OrderBy(x => x.BookDate).GroupBy(t => $"{t.BookDate.Month}.{t.BookDate.Year}");
            var older = new List<double>();
            var components = new List<string>();
            foreach (var m in bymonth)
            {
                var costs = m.Where(x => x.ValueAsProfit < 0).OrderByDescending(x => x.BookDate);
                var incoms = m.Where(x => x.ValueAsProfit > 0).OrderByDescending(x => x.BookDate);
                double income = incoms.Sum(x => x.Value);
                double outcome = costs.Sum(x => x.Value);
                double diff = income - outcome;
                older.Add(diff);
                older.Reverse();
                var last3Elements = older.Take(3).ToList();
                older.Reverse();
                components.Add($"{m.Key,8:0.0} : value: {diff,8:0.0}     avg(3): {last3Elements.Average(),8:0.0}    sum: {older.Sum(),8:0.0}\r\n");
            }

            components.Reverse();
            foreach (string component in components)
            {
                output += component;
            }

            File.WriteAllText("answers.txt", output);
        }

        private void AddTransactionButtonClick(object sender, RoutedEventArgs e)
        {
            string name = (sender as Button)?.Name;
            Transaction transaction = new Transaction();
            _dataContext.Wallet.Transactions.Add(transaction);
            TransactionWindow window = null;

            if (name.ToLower().Contains("income"))
            {
                window =  new TransactionWindow(transaction, _dataContext.Wallet, eTransactionDirection.Income);
            }
            else if (name.ToLower().Contains("outcome"))
            {
                window = new TransactionWindow(transaction, _dataContext.Wallet, eTransactionDirection.Outcome);
            }
            else if (name.ToLower().Contains("transfer"))
            {
                window = new TransactionWindow(transaction, _dataContext.Wallet, eTransactionDirection.Transfer);
            }
            
            window?.Show();
        }

        private void EditButtonClick(object sender, RoutedEventArgs e)
        {
            TransactionWindow window = new TransactionWindow((Transaction)DataGridTransactions.SelectedItem, _dataContext.Wallet, eTransactionDirection.Uknown);
            window.Show();
        }

        private void DataGridTransactions_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e)
        {
            _dataContext.Wallet.Save();
        }

        private void DataGridTransactions_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DataGridTransactions.SelectedIndex = -1;
        }

        private void buttonParse_Click(object sender, RoutedEventArgs e)
        {
            ParserWindow window = new ParserWindow(_dataContext.Wallet);
            window.Show();
        }
        private void HandleKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.S)
            {
                _dataContext.Wallet.Save();
            }
        }

        private void buttonManageStocks_Click(object sender, RoutedEventArgs e)
        {
            ManageStocks window = new ManageStocks(_dataContext.Wallet);
            window.Show();
        }

        private void buttonManageCategories_Click(object sender, RoutedEventArgs e)
        {
            ManageCategoriesWindow window = new ManageCategoriesWindow();
            window.Show();
        }
    }
}
