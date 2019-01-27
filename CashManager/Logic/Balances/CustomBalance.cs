using CashManager.Data.Extensions;
using CashManager.Features.Search;
using CashManager.Model.Common;

namespace CashManager.Logic.Balances
{
    public sealed class CustomBalance : BaseObservableObject
    {
        private string _name;

        public SearchState[] Searches { get; set; }

        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                Id = Name.GenerateGuid();
            }
        }

        private CustomBalance()
        {
            Searches = new SearchState[0];
        }

        public CustomBalance(string name) : this()
        {
            Name = name;
        }
    }
}