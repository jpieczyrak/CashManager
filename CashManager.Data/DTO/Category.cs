using System;

namespace CashManager.Data.DTO
{
	public class Category : Dto
	{
	    public Category() { }

	    public Category(Guid id) { Id = id; }

	    public Category Parent { get; set; }

		public string Name { get; set; }
	}
}
