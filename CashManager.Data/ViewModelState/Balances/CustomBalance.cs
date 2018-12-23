using CashManager.Data.DTO;
using CashManager.Data.Extensions;

namespace CashManager.Data.ViewModelState.Balances
{
    public class CustomBalance : Dto
    {
        public string Name { get; set; }

        public SearchState[] Searches { get; set; }

        private CustomBalance()
        {
            Searches = new SearchState[0];
        }

        public CustomBalance(string name) : this()
        {
            Name = name;
            Id = Name.GenerateGuid();
        }
    }
}