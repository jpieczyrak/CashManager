using System.Linq;

using AutoMapper;

using CashManager.CommonData;
using CashManager.Data.Extensions;
using CashManager.Features.Categories;
using CashManager.Features.MassReplacer;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.TransactionTypes;
using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Model.Selectors;

using DtoTag = CashManager.Data.DTO.Tag;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoType = CashManager.Data.DTO.TransactionType;

namespace CashManager.Features.Search
{
    public sealed class SearchState : BaseObservableObject
    {
        private string _name;

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
            Name = "default";

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

        public void Clear()
        {
            ApplySearchCriteria(new SearchState());
        }

        public override string ToString() => Name;

        public void ApplyReverseReplaceCriteria(ReplacerState state)
        {
            Clear();
            TitleFilter.Value = state.TitleSelector.Value;
            TitleFilter.IsChecked = state.TitleSelector.IsChecked;

            NoteFilter.Value = state.NoteSelector.Value;
            NoteFilter.IsChecked = state.NoteSelector.IsChecked;

            PositionTitleFilter.Value = state.PositionTitleSelector.Value;
            PositionTitleFilter.IsChecked = state.PositionTitleSelector.IsChecked;

            BookDateFilter.From = BookDateFilter.To = state.BookDateSetter.Value;
            BookDateFilter.IsChecked = state.BookDateSetter.IsChecked;

            foreach (var result in CategoriesFilter.ComboBox.InternalDisplayableSearchResults)
                result.IsSelected = result.Id == state.CategoriesSelector.Selected?.Id;
            CategoriesFilter.IsChecked = state.CategoriesSelector.IsChecked;

            foreach (var result in TypesFilter.ComboBox.InternalDisplayableSearchResults)
                result.IsSelected = result.Id == state.TypesSelector.Selected?.Id;
            TypesFilter.IsChecked = state.TypesSelector.IsChecked;

            foreach (var result in UserStocksFilter.ComboBox.InternalDisplayableSearchResults)
                result.IsSelected = result.Id == state.UserStocksSelector.Selected?.Id;
            UserStocksFilter.IsChecked = state.UserStocksSelector.IsChecked;

            foreach (var result in ExternalStocksFilter.ComboBox.InternalDisplayableSearchResults)
                result.IsSelected = result.Id == state.ExternalStocksSelector.Selected?.Id;
            ExternalStocksFilter.IsChecked = state.ExternalStocksSelector.IsChecked;

            foreach (var result in TagsFilter.ComboBox.InternalDisplayableSearchResults)
                result.IsSelected = state.TagsSelector.Results?.Any(x => x.Id == result.Id) ?? false;
            TagsFilter.IsChecked = state.TagsSelector.IsChecked;
        }
    }
}