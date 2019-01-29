namespace CashManager.Data.ViewModelState.Parsers
{
    public class Rule
    {
        public int Property { get; set; }

        public int Column { get; set; }

        public bool IsOptional { get; set; }
    }
}