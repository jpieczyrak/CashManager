namespace Logic
{
    public class TransactionPartPayment
    {
        public TransactionPartPayment(string stock, double value, ePaymentType paymentType)
        {
            Stock = stock;
            Value = value;
            PaymentType = paymentType;
        }

        public string Stock { get; set; }
        public double Value { get; set; }

        public ePaymentType PaymentType { get; set; }
    }
}