using System.Linq;

using AutoMapper;

using CashManager.Data.Extensions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.Features.Categories;
using CashManager_MVVM.Model;
using CashManager_MVVM.Model.Common;
using CashManager_MVVM.Model.Selectors;

using DtoTag = CashManager.Data.DTO.Tag;
using DtoStock = CashManager.Data.DTO.Stock;
using DtoCategory = CashManager.Data.DTO.Category;
using DtoType = CashManager.Data.DTO.TransactionType;

namespace CashManager_MVVM.Features.Search
{
    public class SearchState : BaseObservableObject
    {
        private string _name;
        private const string DEFAULT_NAME = "default";

        public string Name
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

        public DateFrame BookDateFilter { get; private set; }

        public DateFrame CreateDateFilter { get; private set; }

        public DateFrame LastEditDateFilter { get; private set; }

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
            BookDateFilter = new DateFrame(DateFrameType.BookDate);
            CreateDateFilter = new DateFrame(DateFrameType.CreationDate);
            LastEditDateFilter = new DateFrame(DateFrameType.EditDate);
            ValueFilter = new RangeSelector(RangeSelectorType.GrossValue);

            var defaultSource = new BaseSelectable[0];
            UserStocksFilter = new MultiPicker(MultiPickerType.UserStock, defaultSource);
            ExternalStocksFilter = new MultiPicker(MultiPickerType.ExternalStock, defaultSource);
            CategoriesFilter = new MultiPicker(MultiPickerType.Category, defaultSource);
            TypesFilter = new MultiPicker(MultiPickerType.TransactionType, defaultSource);
            TagsFilter = new MultiPicker(MultiPickerType.Tag, defaultSource);

            if (queryDispatcher != null) UpdateSources(queryDispatcher);
        }

        public void UpdateSources(IQueryDispatcher queryDispatcher)
        {
            var availableStocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocksFilter.SetInput(availableStocks.Where(x => x.IsUserStock).ToArray());
            var externalStocks = Mapper.Map<Stock[]>(Mapper.Map<DtoStock[]>(availableStocks)); //we don't want to have same reference in 2 pickers
            ExternalStocksFilter.SetInput(externalStocks);

            var categories = Mapper.Map<Category[]>(queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories).ToArray();
            CategoriesFilter.SetInput(categories);

            var types = Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery()).OrderBy(x => x.Name));
            TypesFilter.SetInput(types);

            var tags = Mapper.Map<Tag[]>(queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsFilter.SetInput(tags);
        }

        public void ApplySearchCriteria(SearchState state)
        {
            TitleFilter = state.TitleFilter;
            NoteFilter = state.NoteFilter;
            PositionTitleFilter = state.PositionTitleFilter;
            BookDateFilter = state.BookDateFilter;
            CreateDateFilter = state.CreateDateFilter;
            LastEditDateFilter = state.LastEditDateFilter;
            ValueFilter = state.ValueFilter;

            foreach (var x in TagsFilter.ComboBox.InternalDisplayableSearchResults)
                x.IsSelected = state.TypesFilter.Selected.Contains(x.Id);
            foreach (var x in CategoriesFilter.ComboBox.InternalDisplayableSearchResults)
                x.IsSelected = state.CategoriesFilter.Selected.Contains(x.Id);
            foreach (var x in TypesFilter.ComboBox.InternalDisplayableSearchResults)
                x.IsSelected = state.TypesFilter.Selected.Contains(x.Id);
            foreach (var x in UserStocksFilter.ComboBox.InternalDisplayableSearchResults)
                x.IsSelected = state.UserStocksFilter.Selected.Contains(x.Id);
            foreach (var x in ExternalStocksFilter.ComboBox.InternalDisplayableSearchResults)
                x.IsSelected = state.ExternalStocksFilter.Selected.Contains(x.Id);
        }
    }
}