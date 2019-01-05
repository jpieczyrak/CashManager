using CashManager.Data.DTO;

namespace CashManager.Logic.Parsers.Custom
{
    public class Rule
    {
        public TransactionField Property { get; set; }

        public int Column { get; set; }

        public bool IsOptional { get; set; } = true;

        public bool Match(string line, Transaction transaction)
        {
            var elements = line.Split(';');
            if (elements.Length < Column) return false;

            switch (Property)
            {
                case TransactionField.Title:
                    transaction.Title = elements[Index];
                    if (string.IsNullOrWhiteSpace(transaction.Title)) return false;
                    break;
                case TransactionField.Note:
                    break;
                case TransactionField.BookDate:
                    break;
                case TransactionField.CreationDate:
                    break;
            }

            return true;
        }

        private int Index => Column - 1;
    }
}