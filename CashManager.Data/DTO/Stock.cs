using System;

namespace CashManager.Data.DTO
{
    public class Stock : Dto
    {
        public string Name { get; set; }

        public bool IsUserStock { get; set; }

        public Balance Balance { get; set; }

        public decimal UserOwnershipPercent { get; set; }

        public Stock()
        {
            Balance = new Balance();
            UserOwnershipPercent = 100;
        }

        public Stock(Guid id) : this()
        {
            Id = id;
        }
    }
}