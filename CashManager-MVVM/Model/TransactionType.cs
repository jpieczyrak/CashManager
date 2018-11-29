namespace CashManager_MVVM.Model
{
    public class TransactionType : BaseObservableObject
    {
        public string Name { get; set; }

        public bool Income { get; set; }

        public bool Outcome { get; set; }

        public bool IsDefault { get; set; }

    }
}