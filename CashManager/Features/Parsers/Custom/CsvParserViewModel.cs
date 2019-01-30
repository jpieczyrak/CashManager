using System;
using System.Linq;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Parsers;
using CashManager.Infrastructure.Query;
using CashManager.Logic.Parsers.Custom;

using GalaSoft.MvvmLight.CommandWpf;

using DtoStock = CashManager.Data.DTO.Stock;
using Rule = CashManager.Model.Parsers.Rule;

namespace CashManager.Features.Parsers.Custom
{
    public class CsvParserViewModel : ParserViewModelBase
    {
        private TrulyObservableCollection<Rule> _rules;

        private string _elementSplitter;

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

        public string ElementSplitter
        {
            get => _elementSplitter;
            set
            {
                Set(ref _elementSplitter, value);
                UpdateParser();
            }
        }

        public TransactionField[] AvailableProperties => Enum.GetValues(typeof(TransactionField)).OfType<TransactionField>().OrderBy(x => x.ToString()).ToArray();


        public Model.Parsers.CustomCsvParser[] Parsers { get; private set; }

        public RelayCommand<string> ParserSaveCommand { get; }
        public RelayCommand<Model.Parsers.CustomCsvParser> ParserLoadCommand { get; }

        public CsvParserViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, TransactionsProvider transactionsProvider) : base(queryDispatcher,
            commandDispatcher, transactionsProvider)
        {
            _elementSplitter = ";";
            Rules = new TrulyObservableCollection<Rule>
            {
                new Rule { Column = 1, Property = TransactionField.Title, IsOptional = false }
            };
            Rules.CollectionChanged += (sender, args) => UpdateParser();
            AddRuleCommand = new RelayCommand(() => Rules.Add(new Rule
            {
                Property = AvailableProperties.Except(Rules.Select(x => x.Property)).FirstOrDefault(),
                Column = Rules.Last().Column + 1
            }));
            RemoveRuleCommand = new RelayCommand<Rule>(x => Rules.Remove(x));
            ParserSaveCommand = new RelayCommand<string>(x =>
            {
                (Parser as CustomCsvParser).Name = x;
                var parser = Mapper.Map<Data.ViewModelState.Parsers.CustomCsvParser>(Parser);
                _commandDispatcher.Execute(new UpsertCsvParserCommand(parser));
            }, x => !string.IsNullOrWhiteSpace(x));
            ParserLoadCommand = new RelayCommand<Model.Parsers.CustomCsvParser>(x =>
            {
                Parser = Mapper.Map<CustomCsvParser>(x);
                Rules = new TrulyObservableCollection<Rule>(Mapper.Map<Rule[]>(x.Rules));
                ElementSplitter = x.ElementSplitter;
            }, x => x != null);
            //Parsers = _queryDispatcher.Execute<CustomCsvParserQuery, Data.ViewModelState.Parsers.CustomCsvParser[]>(new CustomCsvParserQuery());
        }

        private void UpdateParser() => Parser = new CustomCsvParser(Mapper.Map<Logic.Parsers.Custom.Rule[]>(_rules), Mapper.Map<DtoStock[]>(UserStocks), ElementSplitter);
    }
}