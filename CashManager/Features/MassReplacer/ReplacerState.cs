﻿using System;
using System.Linq;

using AutoMapper;

using CashManager.Data.Extensions;
using CashManager.Features.Categories;
using CashManager.Features.Search;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Infrastructure.Query.Stocks;
using CashManager.Infrastructure.Query.Tags;
using CashManager.Infrastructure.Query.TransactionTypes;
using CashManager.Model;
using CashManager.Model.Common;
using CashManager.Model.Selectors;
using CashManager.Model.Setters;

namespace CashManager.Features.MassReplacer
{
    public class ReplacerState : BaseObservableObject
    {
        private string _name;
        private TextSetter _titleSelector;
        private TextSetter _noteSelector;
        private TextSetter _positionTitleSelector;
        private DateSetter _bookDateSetter;
        private SingleSetter _userStocksSelector;
        private SingleSetter _externalStocksSelector;
        private SingleSetter _categoriesSelector;
        private SingleSetter _typesSelector;
        private MultiSetter _tagsSelector;

        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                Id = _name.GenerateGuid();
            }
        }

        public DateSetter BookDateSetter
        {
            get => _bookDateSetter;
            set => Set(nameof(BookDateSetter), ref _bookDateSetter, value);
        }

        public SingleSetter UserStocksSelector
        {
            get => _userStocksSelector;
            set => Set(nameof(UserStocksSelector), ref _userStocksSelector, value);
        }

        public SingleSetter ExternalStocksSelector
        {
            get => _externalStocksSelector;
            set => Set(nameof(ExternalStocksSelector), ref _externalStocksSelector, value);
        }

        public SingleSetter CategoriesSelector
        {
            get => _categoriesSelector;
            set => Set(nameof(CategoriesSelector), ref _categoriesSelector, value);
        }

        public SingleSetter TypesSelector
        {
            get => _typesSelector;
            set => Set(nameof(TypesSelector), ref _typesSelector, value);
        }

        public MultiSetter TagsSelector
        {
            get => _tagsSelector;
            set => Set(nameof(TagsSelector), ref _tagsSelector, value);
        }

        public TextSetter TitleSelector
        {
            get => _titleSelector;
            set => Set(nameof(TitleSelector), ref _titleSelector, value);
        }

        public TextSetter NoteSelector
        {
            get => _noteSelector;
            set => Set(nameof(NoteSelector), ref _noteSelector, value);
        }

        public TextSetter PositionTitleSelector
        {
            get => _positionTitleSelector;
            set => Set(nameof(PositionTitleSelector), ref _positionTitleSelector, value);
        }

        public SearchState SearchState { get; set; }

        public ReplacerState()
        {
            Id = Guid.NewGuid();
            _bookDateSetter = new DateSetter(DateSetterType.BookDate);
            _positionTitleSelector = new TextSetter(TextSetterType.PositionTitle);
            _noteSelector = new TextSetter(TextSetterType.Note);
            _titleSelector = new TextSetter(TextSetterType.Title);

            var defaultSource = new Selectable[0];
            UserStocksSelector = new SingleSetter(MultiPickerType.UserStock, defaultSource);
            ExternalStocksSelector = new SingleSetter(MultiPickerType.ExternalStock, defaultSource);
            CategoriesSelector = new SingleSetter(MultiPickerType.Category, defaultSource);
            TypesSelector = new SingleSetter(MultiPickerType.TransactionType, defaultSource);
            TagsSelector = new MultiSetter(MultiPickerType.Tag, defaultSource);

            SearchState = new SearchState();
        }

        public void Update(IQueryDispatcher queryDispatcher)
        {
            var availableStocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, Data.DTO.Stock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocksSelector.Input = availableStocks.Where(x => x.IsUserStock).Select(x => new Selectable(x)).ToArray();
            ExternalStocksSelector.Input =
                Mapper.Map<Stock[]>(Mapper.Map<Data.DTO.Stock[]>(availableStocks)).Select(x => new Selectable(x)).ToArray();

            var categories = Mapper.Map<Category[]>(queryDispatcher.Execute<CategoryQuery, Data.DTO.Category[]>(new CategoryQuery()));
            CategoriesSelector.Input = categories.OrderBy(x => x.Name).Select(x => new Selectable(x)).ToArray();

            var types = Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, Data.DTO.TransactionType[]>(new TransactionTypesQuery()).OrderBy(x => x.Name));
            TypesSelector.Input = types.Select(x => new Selectable(x)).ToArray();

            var tags = Mapper.Map<Tag[]>(queryDispatcher.Execute<TagQuery, Data.DTO.Tag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsSelector.SetInput(tags.Select(x => new Selectable(x)).ToArray());
        }

        public void ApplyReplaceCriteria(ReplacerState state)
        {
            Name = state.Name;

            TitleSelector.Apply(state.TitleSelector);
            NoteSelector.Apply(state.NoteSelector);
            PositionTitleSelector.Apply(state.PositionTitleSelector);

            BookDateSetter.Apply(state.BookDateSetter);

            TagsSelector.Apply(state.TagsSelector);

            TypesSelector.Apply(state.TypesSelector);
            CategoriesSelector.Apply(state.CategoriesSelector);
            UserStocksSelector.Apply(state.UserStocksSelector);
            ExternalStocksSelector.Apply(state.ExternalStocksSelector);

            RaisePropertyChanged();
        }

        public override string ToString() => Name;

        public void Clear()
        {
            ApplyReplaceCriteria(new ReplacerState());
        }
    }
}