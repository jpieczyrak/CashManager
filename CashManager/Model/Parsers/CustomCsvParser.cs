using CashManager.Data.Extensions;
using CashManager.Model.Common;

namespace CashManager.Model.Parsers
{
    public class CustomCsvParser : BaseObservableObject
    {
        public string ColumnSplitter { get; set; }

        public Rule[] Rules { get; set; }

        public override string ToString() => Name;

        public override int GetHashCode() { return Name.GenerateGuid().GetHashCode(); }
    }
}