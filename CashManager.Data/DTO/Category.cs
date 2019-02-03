using System;

using CashManager.Data.Properties;

namespace CashManager.Data.DTO
{
    public class Category : Dto
    {
        public static readonly Category Default = new Category(new Guid(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)) { Name = Strings.DefaultCategoryName };

        public Category Parent { get; set; }

        public string Name { get; set; }

        public Category() { }

        public Category(Guid id) { Id = id; }
    }
}