namespace CashManager.Data.DTO
{
    public class PaymentValue : Dto
    {
        public double GrossValue { get; set; }

        public double NetValue { get; set; }

        public double TaxPercentValue { get; set; }
    }
}