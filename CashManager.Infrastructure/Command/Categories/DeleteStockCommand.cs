using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Categories
{
    public class DeleteCategoryCommand : ICommand
    {
        public Category Category { get; }

        public DeleteCategoryCommand(Category category)
        {
            Category = category;
        }
    }
}