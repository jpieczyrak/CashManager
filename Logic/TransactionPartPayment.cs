using System.Collections.ObjectModel;

namespace Logic
{
    public class TransactionPartPayment
    {
        public TransactionPartPayment(string stock, double value)
        {
            Stock = stock;
            Value = value;
        }

        public string Stock { get; set; }
        public double Value { get; set; }
    }
}