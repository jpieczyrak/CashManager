using CashManager.Data.DTO;

namespace CashManager.Infrastructure.Command.Tags
{
    public class DeleteTagCommand : ICommand
    {
        public Tag Tag { get; }

        public DeleteTagCommand(Tag tag)
        {
            Tag = tag;
        }
    }
}