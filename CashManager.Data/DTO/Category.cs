using System;

namespace CashManager.Data.DTO
{
    public class Category
    {
        public Guid Id { get; set; }

        public Category Parent { get; set; }

        public string Value { get; set; }
		
		public override bool Equals(object obj) => obj?.GetHashCode() == GetHashCode();

		public override int GetHashCode() => Id.GetHashCode();
    }
}