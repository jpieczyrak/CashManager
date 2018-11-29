namespace CashManager.Data.DTO
{
    public class TransactionType : Dto
    {
        public string Name { get; set; }

        public bool Income { get; set; }
        public bool Outcome { get; set; }
        public bool IsDefault { get; set; }
    }
}