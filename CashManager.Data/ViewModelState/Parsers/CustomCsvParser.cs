using CashManager.Data.DTO;
using CashManager.Data.Extensions;

namespace CashManager.Data.ViewModelState.Parsers
{
    public class CustomCsvParser : Dto
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                Id = value.GenerateGuid();
            }
        }

        public Rule[] Rules { get; set; }

        public string ColumnSplitter { get; set; }
    }
}