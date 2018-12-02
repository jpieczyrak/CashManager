using System;

namespace CashManager.Data.DTO
{
    public class Balance : Dto
    {
        public double Value { get; set; }

        public Balance(DateTime date, double value)
        {
            Value = value;
            LastEditDate = date;
        }

        public Balance() { }
    }
}