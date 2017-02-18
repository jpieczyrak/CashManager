namespace Logic.TransactionManagement.TransactionElements
{
    /// <summary>
    /// Store category of transaction.
    /// Each transaction can have only one category.
    /// Sum of values of categories should be equal sum of values of all transaction.
    /// </summary>
    public class Category
    {
        public string Value { get; set; }

        public Category(string value)
        {
            Value = value;
        }
    }
}