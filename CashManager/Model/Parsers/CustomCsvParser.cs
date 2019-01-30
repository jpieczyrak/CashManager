using CashManager.Model.Common;

namespace CashManager.Model.Parsers
{
    public class CustomCsvParser : BaseObservableObject
    {
        public string ElementSplitter { get; set; }

        public Rule[] Rules { get; set; }
    }
}