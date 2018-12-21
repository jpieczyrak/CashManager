using CashManager.Data.DTO;
using CashManager.Data.ViewModelState.Selectors;

namespace CashManager.Data.ViewModelState
{
    public class SearchState : Dto
    {
        public string Name { get; set; }

        public TextSelector TitleFilter { get; set; }

        public TextSelector NoteFilter { get; set; }

        public TextSelector PositionTitleFilter { get; set; }

        public DateFrame BookDateFilter { get; set; }

        public DateFrame CreateDateFilter { get; set; }

        public DateFrame LastEditDateFilter { get; set; }

        public MultiPicker UserStocksFilter { get; set; }

        public MultiPicker ExternalStocksFilter { get; set; }

        public MultiPicker CategoriesFilter { get; set; }

        public MultiPicker TypesFilter { get; set; }

        public MultiPicker TagsFilter { get; set; }

        public RangeSelector ValueFilter { get; set; }
    }
}