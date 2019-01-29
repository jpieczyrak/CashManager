using System;
using System.Linq;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Query;
using CashManager.Logic.Parsers;
using CashManager.Logic.Parsers.Custom;

using GalaSoft.MvvmLight.CommandWpf;

using DtoStock = CashManager.Data.DTO.Stock;

namespace CashManager.Features.Parsers.Custom
{
    public class CsvParserViewModel : ParserViewModelBase
    {
        public RelayCommand AddRuleCommand { get; }
        public RelayCommand<Model.Parsers.Rule> RemoveRuleCommand { get; }

        private TrulyObservableCollection<Model.Parsers.Rule> _rules;

        public TrulyObservableCollection<Model.Parsers.Rule> Rules
        {
            get => _rules;
            set
            {
                Set(ref _rules, value);
                UpdateParser();
            }
        }

        private string _elementSplitter;

        public string ElementSplitter
        {
            get => _elementSplitter;
            set
            {
                Set(ref _elementSplitter, value);
                UpdateParser();
            }
        }

        public TransactionField[] AvailableProperties
            => Enum.GetValues(typeof(TransactionField)).OfType<TransactionField>().ToArray();

        public CsvParserViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, TransactionsProvider transactionsProvider) : base(queryDispatcher,
            commandDispatcher, transactionsProvider)
        {
            _elementSplitter = ";";
            Rules = new TrulyObservableCollection<Model.Parsers.Rule>
            {
                new Model.Parsers.Rule { Column = 1, Property = TransactionField.Title, IsOptional = false }
            };
            Rules.CollectionChanged += (sender, args) => UpdateParser();
            AddRuleCommand = new RelayCommand(() => Rules.Add(new Model.Parsers.Rule
            {
                Column = 1,
                Property = AvailableProperties.Except(Rules.Select(x => x.Property)).FirstOrDefault()
            }));
            RemoveRuleCommand = new RelayCommand<Model.Parsers.Rule>(x => Rules.Remove(x));
        }

        private void UpdateParser()
            => Parser = new CustomCsvParser(Mapper.Map<Rule[]>(_rules), Mapper.Map<DtoStock[]>(UserStocks), ElementSplitter);
    }
}