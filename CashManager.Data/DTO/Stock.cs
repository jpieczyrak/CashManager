using System;

namespace CashManager.Data.DTO
{
    public class Stock
    {
        public string Name { get; set; }

        public bool IsUserStock { get; set; }
        
        public Guid Id { get; set; }
    }
}