using System;

namespace CashManager.Data.DTO
{
	public class Dto
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public override bool Equals(object obj) => obj?.GetHashCode() == GetHashCode();

		public override int GetHashCode() => Id.GetHashCode();
    }
}
