using System;

namespace CashManager.Data.DTO
{
	public class Dto
	{
        public Guid Id { get; protected set; } = Guid.NewGuid();

	    public DateTime InstanceCreationDate { get; protected set; } = DateTime.Now;

	    public DateTime LastEditDate { get; set; } = DateTime.Now;

        public override bool Equals(object obj) => obj?.GetHashCode() == GetHashCode();

		public override int GetHashCode() => Id.GetHashCode();
    }
}
