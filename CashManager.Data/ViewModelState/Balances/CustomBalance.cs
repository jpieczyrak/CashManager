using CashManager.Data.DTO;
using CashManager.Data.Extensions;

namespace CashManager.Data.ViewModelState.Balances
{
    public class CustomBalance : Dto
    {
        public string Name { get; set; }

        public SearchState[] Searches { get; set; }

        public CustomBalance(string name)
        {
            Name = name;
            Id = Name.GenerateGuid();
            Searches = new SearchState[0];
        }
    }
}