﻿using System.Windows;
using System.Windows.Controls;

using CashManager.Model;

namespace CashManager.Features.Transactions
{
    public partial class TransactionListView : UserControl
    {
        public static DependencyProperty TransactionsProperty =
            DependencyProperty.Register(nameof(Transactions), typeof(Transaction[]),
                typeof(TransactionListView), new UIPropertyMetadata(PropertyChangedHandler));

        public Transaction[] Transactions
        {
            get => (Transaction[])GetValue(TransactionsProperty);
            set => SetValue(TransactionsProperty, value);
        }

        public static void PropertyChangedHandler(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((TransactionListView)sender).Transactions = e.NewValue as Transaction[];
        }

        public TransactionListView()
        {
            InitializeComponent();
        }
    }
}