namespace CashManager.Data.DTO
{
    public class PaymentValue : Dto
    {
        public decimal GrossValue { get; set; }

        public decimal NetValue { get; set; }

        public decimal TaxPercentValue { get; set; }
    }
}