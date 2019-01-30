using CashManager.Data.DTO;

namespace CashManager.Data.ViewModelState
{
    public class BaseSelector : Dto
    {
        public bool IsChecked { get; set; }

        public string Description { get; set; }
    }
}