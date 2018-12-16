using System;

namespace CashManager.Data.DTO
{
    public class Balance : Dto
    {
        public decimal Value { get; set; }

        public Balance(DateTime date, decimal value)
        {
            Value = value;
            LastEditDate = date;
        }

        public Balance() { }
    }
}