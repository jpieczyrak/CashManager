﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CashManager.Features;
using CashManager.Features.Transactions;
using CashManager.Features.TransactionTypes;
using CashManager.Model;
using CashManager.Properties;

using log4net;

namespace CashManager.Logic.Creators
{
    public class CorrectionsCreator : ICorrectionsCreator
    {
        private readonly ViewModelFactory _factory;
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(CorrectionsCreator)));

        private readonly Lazy<TransactionViewModel> _transactionCreator;
        private readonly Lazy<TransactionTypesViewModel> _typesProvider;

        public CorrectionsCreator(ViewModelFactory factory)
        {
            _factory = factory;
            _transactionCreator = new Lazy<TransactionViewModel>(() => _factory.Create<TransactionViewModel>());
            _typesProvider = new Lazy<TransactionTypesViewModel>(() => _factory.Create<TransactionTypesViewModel>());
        }

        public void CreateCorrection(Stock stock, decimal diff, string title)
        {
            if (diff == 0m) return;

            var incomeTypes = _typesProvider.Value.TransactionTypes
                                            .Where(x => x.Income && !x.IsTransfer)
                                            .OrderByDescending(x => x.IsDefault);
            var outcomeTypes = _typesProvider.Value.TransactionTypes
                                             .Where(x => x.Outcome && !x.IsTransfer)
                                             .OrderByDescending(x => x.IsDefault);
            var transaction = new Transaction
            {
                Title = Strings.Correction,
                Notes = new TrulyObservableCollection<Note> { new Note { Value = title } },
                BookDate = DateTime.Today,
                Type = diff > 0
                           ? incomeTypes.FirstOrDefault()
                           : outcomeTypes.FirstOrDefault(),
                UserStock = stock
            };
            if (transaction.Type == null) _logger.Value.Info("Could not create correction transaction, no matching types!");
            decimal abs = Math.Abs(diff);
            var position = new Position
            {
                Title = title,
                BookDate = DateTime.Today,
                Value = new PaymentValue(abs, abs, 0m),
                Parent = transaction
            };
            transaction.Positions.Add(position);
            _transactionCreator.Value.ShouldGoBack = false;
            _transactionCreator.Value.Transaction = transaction;
            _transactionCreator.Value.Update();
            _transactionCreator.Value.SetUpdateMode(TransactionEditModes.NoChange);
            _transactionCreator.Value.SaveTransactionCommand.Execute(null);
            _transactionCreator.Value.ShouldGoBack = true;
        }
    }
}