using System.Collections.Generic;

namespace Logic.TransactionManagement
{
    public class Subtransation
    {
        public string Name { get; set; }

        public double Value { get; set; }

        public Category Category { get; set; }

        /// <summary>
        /// List of tags
        /// </summary>
        public List<string> Tags { get; set; }
    }
}