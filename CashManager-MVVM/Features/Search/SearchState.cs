using CashManager_MVVM.Model.Selectors;

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

        public SearchState()
        {
            TitleFilter = new TextSelector(TextSelectorType.Title);
            NoteFilter = new TextSelector(TextSelectorType.Note);
            PositionTitleFilter = new TextSelector(TextSelectorType.PositionTitle);
            BookDateFilter = new DateFrame(DateFrameType.BookDate);
            CreateDateFilter = new DateFrame(DateFrameType.CreationDate);
            LastEditDateFilter = new DateFrame(DateFrameType.EditDate);
        }
    }
}