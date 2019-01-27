using System.Linq;

using AutoMapper;

using CashManager.Data.Extensions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager.WPF.CommonData;
using CashManager.WPF.Features.Categories;
using CashManager.WPF.Model;
using CashManager.WPF.Model.Common;
using CashManager.WPF.Model.Selectors;

using DtoTag = CashManager.Data.DTO.Tag;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoType = CashManager.Data.DTO.TransactionType;

namespace CashManager.WPF.Features.Search
{
    public sealed class SearchState : BaseObservableObject
    {
        private string _name;
        public const string DEFAULT_NAME = "default";

        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                Id = _name.GenerateGuid();
            }
        }

        public TextSelector TitleFilter { get; private set; }

        public TextSelector NoteFilter { get; private set; }

        public TextSelector PositionTitleFilter { get; private set; }

        public DateFrameSelector BookDateFilter { get; private set; }

        public DateFrameSelector CreateDateFilter { get; private set; }

        public DateFrameSelector LastEditDateFilter { get; private set; }

        public MultiPicker UserStocksFilter { get; set; }

        public MultiPicker ExternalStocksFilter { get; set; }

        public MultiPicker CategoriesFilter { get; set; }

        public MultiPicker TypesFilter { get; set; }

        public MultiPicker TagsFilter { get; set; }

        public RangeSelector ValueFilter { get; set; }

        public SearchState(IQueryDispatcher queryDispatcher = null)
        {
            Name = DEFAULT_NAME;

            TitleFilter = new TextSelector(TextSelectorType.Title);
            NoteFilter = new TextSelector(TextSelectorType.Note);
            PositionTitleFilter = new TextSelector(TextSelectorType.PositionTitle);
            BookDateFilter = new DateFrameSelector(DateFrameType.BookDate);
            CreateDateFilter = new DateFrameSelector(DateFrameType.CreationDate);
            LastEditDateFilter = new DateFrameSelector(DateFrameType.EditDate);
            ValueFilter = new RangeSelector(RangeSelectorType.GrossValue);

            var defaultSource = new Selectable[0];
            UserStocksFilter = new MultiPicker(MultiPickerType.UserStock, defaultSource);
            ExternalStocksFilter = new MultiPicker(MultiPickerType.ExternalStock, defaultSource);
            CategoriesFilter = new MultiPicker(MultiPickerType.Category, defaultSource);
            TypesFilter = new MultiPicker(MultiPickerType.TransactionType, defaultSource);
            TagsFilter = new MultiPicker(MultiPickerType.Tag, defaultSource);

            if (queryDispatcher != null) UpdateSources(queryDispatcher);
        }

        public void UpdateSources(IQueryDispatcher queryDispatcher, TransactionsProvider transactionsProvider = null)
        {
            var userStocks = queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery()).Where(x => x.IsUserStock).OrderBy(x => x.Name);
            UserStocksFilter.SetInput(Mapper.Map<Stock[]>(userStocks).Select(x => new Selectable(x)).ToArray());

            var exStocks = queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery());
            ExternalStocksFilter.SetInput(Mapper.Map<Stock[]>(exStocks).Select(x => new Selectable(x)).ToArray());

            var categories = Mapper.Map<Category[]>(queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories);
            CategoriesFilter.SetInput(Mapper.Map<Category[]>(categories).Select(x => new Selectable(x)).ToArray());

            var types = Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery()).OrderBy(x => x.Name));
            TypesFilter.SetInput(types.Select(x => new Selectable(x)).ToArray());

            var tags = Mapper.Map<Tag[]>(queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsFilter.SetInput(tags.Select(x => new Selectable(x)).ToArray());

            bool availableTransactions = transactionsProvider?.AllTransactions?.Any() ?? false;
            ValueFilter.MinimumValue = availableTransactions
                                           ? transactionsProvider.AllTransactions.Min(x => x.ValueAsProfit)
                                           : decimal.MinValue;
            ValueFilter.MaximumValue = availableTransactions
                                           ? transactionsProvider.AllTransactions.Max(x => x.ValueAsProfit)
                                           : decimal.MaxValue;
        }

        public void ApplySearchCriteria(SearchState state)
        {
            Name = state.Name;

            TitleFilter.Apply(state.TitleFilter);
            NoteFilter.Apply(state.NoteFilter);
            PositionTitleFilter.Apply(state.PositionTitleFilter);
            BookDateFilter.Apply(state.BookDateFilter);
            CreateDateFilter.Apply(state.CreateDateFilter);
            LastEditDateFilter.Apply(state.LastEditDateFilter);
            ValueFilter.Apply(state.ValueFilter);

            TagsFilter.Apply(state.TagsFilter);
            TypesFilter.Apply(state.TypesFilter);
            CategoriesFilter.Apply(state.CategoriesFilter);
            UserStocksFilter.Apply(state.UserStocksFilter);
            ExternalStocksFilter.Apply(state.ExternalStocksFilter);

            RaisePropertyChanged();
        }
    }
}