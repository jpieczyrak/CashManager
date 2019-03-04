using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Features.MassReplacer;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Parsers;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Parsers;
using CashManager.Logic.Creators;
using CashManager.Logic.Parsers.Custom;
using CashManager.Model.Common;

using GalaSoft.MvvmLight.CommandWpf;

using DtoStock = CashManager.Data.DTO.Stock;
using Rule = CashManager.Model.Parsers.Rule;

namespace CashManager.Features.Parsers.Custom
{
    public class CsvParserViewModel : ParserViewModelBase
    {
        private const string DEFAULT_COLUMN_SPLITTER = ";";
        private TrulyObservableCollection<Rule> _rules;

        private string _columnSplitter;

        public RelayCommand AddRuleCommand { get; }

        public RelayCommand<Rule> RemoveRuleCommand { get; }

        public TrulyObservableCollection<Rule> Rules
        {
            get => _rules;
            set
            {
                Set(ref _rules, value);
                UpdateParser();
            }
        }

        public string ColumnSplitter
        {
            get => _columnSplitter;
            set
            {
                Set(ref _columnSplitter, value);
                UpdateParser();
            }
        }

        public TransactionField[] AvailableProperties => Enum.GetValues(typeof(TransactionField)).OfType<TransactionField>().OrderBy(x => x.ToString()).ToArray();

        public ObservableCollection<BaseObservableObject> Parsers { get; private set; }

        public RelayCommand ClearCommand { get; }

        public RelayCommand<string> ParserSaveCommand { get; }

        public RelayCommand<BaseObservableObject> ParserLoadCommand { get; }

        public CsvParserViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, ICorrectionsCreator correctionsCreator, TransactionsProvider transactionsProvider, MassReplacerViewModel replacer) : base(queryDispatcher,
            commandDispatcher, correctionsCreator, transactionsProvider, replacer)
        {
            AddRuleCommand = new RelayCommand(() => Rules.Add(new Rule
            {
                Property = NotUsedProperties.FirstOrDefault(),
                Column = (Rules.LastOrDefault()?.Column ?? 0) + 1
            }), () => NotUsedProperties.Any());
            RemoveRuleCommand = new RelayCommand<Rule>(x => Rules.Remove(x));
            ParserSaveCommand = new RelayCommand<string>(name =>
            {
                (Parser as CustomCsvParser).Name = name;
                var parser = Mapper.Map<Data.ViewModelState.Parsers.CustomCsvParser>(Parser);
                _commandDispatcher.Execute(new UpsertCsvParserCommand(parser));

                var modelParser = Mapper.Map<Model.Parsers.CustomCsvParser>(Parser);
                Parsers.Remove(modelParser);
                Parsers.Add(modelParser);
            }, name => !string.IsNullOrWhiteSpace(name));
            ParserLoadCommand = new RelayCommand<BaseObservableObject>(selected =>
            {
                var parser = selected as Model.Parsers.CustomCsvParser;
                Parser = Mapper.Map<CustomCsvParser>(selected);
                Rules.Clear();
                Rules.AddRange(Mapper.Map<Rule[]>(parser.Rules).OrderBy(x => x.Column));
                ColumnSplitter = parser.ColumnSplitter;
                UpdateParser();
            }, selected => selected != null);
            ClearCommand = new RelayCommand(Clear);

            var customCsvParsers = _queryDispatcher.Execute<CustomCsvParserQuery, Data.ViewModelState.Parsers.CustomCsvParser[]>(new CustomCsvParserQuery()).OrderBy(x => x.Name);
            Parsers = new ObservableCollection<BaseObservableObject>(Mapper.Map<Model.Parsers.CustomCsvParser[]>(customCsvParsers));
            Clear();
            Rules.CollectionChanged += (sender, args) =>
            {
                UpdateParser();
                ValidateStockGenerationOption();
            };
        }

        private void ValidateStockGenerationOption()
        {
            bool canUpdate = _rules.Any(x => x.Property == TransactionField.UserStock);
            CanGenerateMissingStocks = canUpdate;
            if (!canUpdate) GenerateMissingStocks = false;
        }

        private void Clear()
        {
            Rules = new TrulyObservableCollection<Rule>
            {
                new Rule { Column = 1, Property = TransactionField.Title, IsOptional = false }
            };
            ColumnSplitter = DEFAULT_COLUMN_SPLITTER;
        }

        private IEnumerable<TransactionField> NotUsedProperties => AvailableProperties.Except(Rules.Select(x => x.Property));

        private void UpdateParser() => Parser = new CustomCsvParser(Mapper.Map<Logic.Parsers.Custom.Rule[]>(_rules), Mapper.Map<DtoStock[]>(UserStocks), ColumnSplitter);
    }
}