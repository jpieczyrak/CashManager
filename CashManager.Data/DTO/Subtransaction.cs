using System;
using System.Collections.Generic;

namespace CashManager.Data.DTO
{
    public class Subtransaction
    {
        public string Title { get; set; }

        public PaymentValue Value { get; set; }
        
        public Guid Id { get; set; }

        public List<Tag> Tags { get; set; }

        public Category Category { get; set; }
    }
}