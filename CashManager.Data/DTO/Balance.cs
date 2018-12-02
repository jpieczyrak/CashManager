using System;

namespace CashManager.Data.DTO
{
    public class Balance : Dto
    {
        public double Value { get; set; }

        public DateTime Date { get; set; }
    }
}