using System.Linq;

using AutoMapper;

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
    public class SearchState
    {
        public TextSelector TitleFilter { get; }

        public TextSelector NoteFilter { get; }

        public TextSelector PositionTitleFilter { get; }

        public DateFrame BookDateFilter { get; }

        public DateFrame CreateDateFilter { get; }

        public DateFrame LastEditDateFilter { get; }

        public MultiPicker UserStocksFilter { get; set; }

        public MultiPicker ExternalStocksFilter { get; set; }

        public MultiPicker CategoriesFilter { get; set; }

        public MultiPicker TypesFilter { get; set; }

        public MultiPicker TagsFilter { get; set; }

        public RangeSelector ValueFilter { get; set; }

        public SearchState(IQueryDispatcher queryDispatcher)
        {
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
            UpdateSources(queryDispatcher);
        }

        public void UpdateSources(IQueryDispatcher queryDispatcher)
        {
            var availableStocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocksFilter.ComboBox.SetInput(availableStocks.Where(x => x.IsUserStock).ToArray(), UserStocksFilter.Results);
            var externalStocks = Mapper.Map<Stock[]>(Mapper.Map<DtoStock[]>(availableStocks)); //we don't want to have same reference in 2 pickers
            ExternalStocksFilter.ComboBox.SetInput(externalStocks, ExternalStocksFilter.Results);

            var categories = Mapper.Map<Category[]>(queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories).ToArray();
            CategoriesFilter.ComboBox.SetInput(categories, CategoriesFilter.Results);

            var types = Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery()).OrderBy(x => x.Name));
            TypesFilter.ComboBox.SetInput(types, TypesFilter.Results);

            var tags = Mapper.Map<Tag[]>(queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsFilter.ComboBox.SetInput(tags, TagsFilter.Results);
        }
    }
}