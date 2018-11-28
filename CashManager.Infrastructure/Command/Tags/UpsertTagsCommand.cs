using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Tags
{
    public class UpsertTagsCommand : ICommand
    {
        public Tag[] Tags { get; }

        public UpsertTagsCommand(Tag[] tags)
        {
            Tags = tags;
        }
    }
}