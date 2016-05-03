using System.Collections.Generic;

namespace Logic.TransactionManagement
{
    public class Subtransaction
    {
        public string Name { get; set; }

        public double Value { get; set; }

        public Category Category { get; set; }

        public string Tags { get; set; }
    }
}