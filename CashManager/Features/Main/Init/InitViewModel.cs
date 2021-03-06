﻿using System;
using System.Globalization;
using System.Threading;

using Autofac;

using CashManager.Data.DTO;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Categories;
using CashManager.Infrastructure.Command.Stocks;
using CashManager.Infrastructure.Command.Tags;
using CashManager.Infrastructure.Command.Transactions;
using CashManager.Infrastructure.Command.TransactionTypes;
using CashManager.Infrastructure.Modules;
using CashManager.Logic.DefaultData;
using CashManager.Logic.Extensions;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager.Features.Main.Init
{
    public class InitViewModel : ViewModelBase
    {
        private readonly ContainerBuilder _builder;
        private readonly string _databaseFilepath;
        private readonly Action _closeWindowAction;
        private bool _generateCategories;

        private bool _generateTypes;

        private bool _generateTransactions;

        private string _password;

        private bool _generateStocks;

        private bool _generateTags;

#if BETA
        public bool CanGenerateTransactions { get; } = true;
#else
        public bool CanGenerateTransactions { get; } = false;
#endif

        public bool GenerateCategories
        {
            get => _generateCategories;
            set => Set(ref _generateCategories, value);
        }

        public bool GenerateTypes
        {
            get => _generateTypes;
            set => Set(ref _generateTypes, value);
        }

        public bool GenerateTransactions
        {
            get => _generateTransactions;
            set
            {
                if (value)
                {
                    GenerateCategories = true;
                    GenerateStocks = true;
                    GenerateTags = true;
                    GenerateTypes = true;
                }

                Set(ref _generateTransactions, value);
            }
        }

        public string Password
        {
            get => _password;
            set => Set(ref _password, value);
        }

        public RelayCommand StartCommand { get; }

        public bool GenerateStocks
        {
            get => _generateStocks;
            set => Set(ref _generateStocks, value);
        }

        public bool GenerateTags
        {
            get => _generateTags;
            set => Set(ref _generateTags, value);
        }

        public string SelectedLocalization
        {
            get => Properties.Settings.Default.Localization;
            set
            {
                Properties.Settings.Default.Localization = value;
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(value);
            }
        }

        public string[] Localizations { get; } = { "pl-PL", "en-US" };

        private bool _moreData;

        public bool MoreData
        {
            get => _moreData;
            set => Set(ref _moreData, value);
        }

        public bool CanStartApplication { get; private set; }

        public InitViewModel(ContainerBuilder builder, string databaseFilepath, Action closeWindowAction)
        {
            _generateCategories = _generateTypes = true;

            StartCommand = new RelayCommand(ExecuteStartCommand);
            _builder = builder;
            _databaseFilepath = databaseFilepath;
            _closeWindowAction = closeWindowAction;
        }

        public void GenerateData(ICommandDispatcher commandDispatcher)
        {
#if BETA
            var defaultDataProvider = new TestDataProvider(MoreData);
#else
            var defaultDataProvider = new DefaultDataProvider();
#endif

            var stocks = defaultDataProvider.GetStocks();
            if (GenerateStocks) commandDispatcher.Execute(new UpsertStocksCommand(stocks));

            var categories = defaultDataProvider.GetCategories();
            if (GenerateCategories) commandDispatcher.Execute(new UpsertCategoriesCommand(categories));
            else commandDispatcher.Execute(new UpsertCategoriesCommand(Category.Default));

            var types = defaultDataProvider.GetTransactionTypes();
            if (GenerateTypes) commandDispatcher.Execute(new UpsertTransactionTypesCommand(types));

            var tags = defaultDataProvider.GetTags();
            if (GenerateTags) commandDispatcher.Execute(new UpsertTagsCommand(tags));

            var transactions = defaultDataProvider.GetTransactions(stocks, categories, types, tags);
            if (GenerateTransactions) commandDispatcher.Execute(new UpsertTransactionsCommand(transactions));
        }

        private void ExecuteStartCommand()
        {
            bool passwordExists = !string.IsNullOrWhiteSpace(Password);
            Properties.Settings.Default.IsPasswordNeeded = passwordExists;

            string connectionString = $"Filename={_databaseFilepath};Journal=true";
            if (passwordExists) connectionString += $";password={Password.Encrypt()}";

            _builder.Register(x => connectionString).Keyed<string>(DatabaseCommunicationModule.DB_KEY);

            CanStartApplication = true;

            _closeWindowAction.Invoke();
        }
    }
}