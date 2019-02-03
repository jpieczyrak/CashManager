using System;

namespace CashManager.Data.DTO
{
	public class Category : Dto
	{
	    public static readonly Category Default = new Category(new Guid(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

        public Category() { }

	    public Category(Guid id) { Id = id; }

	    public Category Parent { get; set; }

		public string Name { get; set; }
	}
}
