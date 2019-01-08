﻿using System.Linq;

using AutoMapper;

using CashManager.Data.Extensions;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.TransactionTypes;

using CashManager_MVVM.CommonData;
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
    public sealed class SearchState : BaseSelectable
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

        public void UpdateSources(IQueryDispatcher queryDispatcher, TransactionsProvider transactionsProvider = null)
        {
            var availableStocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, DtoStock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocksFilter.SetInput(availableStocks.Where(x => x.IsUserStock).ToArray());
            var externalStocks = Mapper.Map<Stock[]>(Mapper.Map<DtoStock[]>(availableStocks)); //we don't want to have same reference in 2 pickers
            ExternalStocksFilter.SetInput(externalStocks);

            var categories = Mapper.Map<Category[]>(queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories);
            CategoriesFilter.SetInput(categories);

            var types = Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, DtoType[]>(new TransactionTypesQuery()).OrderBy(x => x.Name));
            TypesFilter.SetInput(types);

            var tags = Mapper.Map<Tag[]>(queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsFilter.SetInput(tags);

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