using CashManager.Data.DTO;
using CashManager.Data.ViewModelState.Setters;

namespace CashManager.Data.ViewModelState
{
    public class MassReplacerState : Dto
    {
        public string Name { get; set; }

        public DateSetter BookDateSetter { get; set; }

        public SingleSetter UserStocksSelector { get; set; }

        public SingleSetter ExternalStocksSelector { get; set; }

        public SingleSetter CategoriesSelector { get; set; }

        public SingleSetter TypesSelector { get; set; }

        public MultiSetter TagsSelector { get; set; }

        public TextSetter TitleSelector { get; set; }

        public TextSetter NoteSelector { get; set; }

        public TextSetter PositionTitleSelector { get; set; }

        public SearchState SearchState { get; set; }
    }
}