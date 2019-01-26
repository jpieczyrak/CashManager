using System;
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
using CashManager_MVVM.Model.Setters;

namespace CashManager_MVVM.Features.MassReplacer
{
    public class ReplacerState : BaseObservableObject
    {
        private string _name;
        private TextSetter _titleSelector;
        private TextSetter _noteSelector;
        private TextSetter _positionTitleSelector;
        private DateSetter _bookDateSetter;
        private SinglePicker _userStocksSelector;
        private SinglePicker _externalStocksSelector;
        private SinglePicker _categoriesSelector;
        private SinglePicker _typesSelector;
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

        public SinglePicker UserStocksSelector
        {
            get => _userStocksSelector;
            set => Set(nameof(UserStocksSelector), ref _userStocksSelector, value);
        }

        public SinglePicker ExternalStocksSelector
        {
            get => _externalStocksSelector;
            set => Set(nameof(ExternalStocksSelector), ref _externalStocksSelector, value);
        }

        public SinglePicker CategoriesSelector
        {
            get => _categoriesSelector;
            set => Set(nameof(CategoriesSelector), ref _categoriesSelector, value);
        }

        public SinglePicker TypesSelector
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

        public ReplacerState()
        {
            Id = Guid.NewGuid();
            _bookDateSetter = new DateSetter(DateSetterType.BookDate);
            _positionTitleSelector = new TextSetter(TextSetterType.PositionTitle);
            _noteSelector = new TextSetter(TextSetterType.Note);
            _titleSelector = new TextSetter(TextSetterType.Title);

            var defaultSource = new Selectable[0];
            UserStocksSelector = new SinglePicker(MultiPickerType.UserStock, defaultSource);
            ExternalStocksSelector = new SinglePicker(MultiPickerType.ExternalStock, defaultSource);
            CategoriesSelector = new SinglePicker(MultiPickerType.Category, defaultSource);
            TypesSelector = new SinglePicker(MultiPickerType.TransactionType, defaultSource);
            TagsSelector = new MultiSetter(MultiPickerType.Tag, defaultSource);
        }


        public void Update(IQueryDispatcher queryDispatcher)
        {
            var availableStocks = Mapper.Map<Stock[]>(queryDispatcher.Execute<StockQuery, CashManager.Data.DTO.Stock[]>(new StockQuery())).OrderBy(x => x.Name);
            UserStocksSelector.Input = availableStocks.Where(x => x.IsUserStock).Select(x => new Selectable(x)).ToArray();
            ExternalStocksSelector.Input =
                Mapper.Map<Stock[]>(Mapper.Map<CashManager.Data.DTO.Stock[]>(availableStocks)).Select(x => new Selectable(x)).ToArray();

            var categories = Mapper.Map<Category[]>(queryDispatcher.Execute<CategoryQuery, CashManager.Data.DTO.Category[]>(new CategoryQuery()));
            categories = CategoryDesignHelper.BuildGraphicalOrder(categories);
            CategoriesSelector.Input = categories.Select(x => new Selectable(x)).ToArray();

            var types = Mapper.Map<TransactionType[]>(queryDispatcher.Execute<TransactionTypesQuery, CashManager.Data.DTO.TransactionType[]>(new TransactionTypesQuery()).OrderBy(x => x.Name));
            TypesSelector.Input = types.Select(x => new Selectable(x)).ToArray();

            var tags = Mapper.Map<Tag[]>(queryDispatcher.Execute<TagQuery, CashManager.Data.DTO.Tag[]>(new TagQuery()).OrderBy(x => x.Name));
            TagsSelector.SetInput(tags.Select(x => new Selectable(x)).ToArray());
        }
    }
}