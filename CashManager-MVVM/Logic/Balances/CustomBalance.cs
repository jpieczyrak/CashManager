using CashManager.Data.Extensions;

using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Logic.Balances
{
    public class CustomBalance : BaseObservableObject
    {
        private string _name;

        public SearchState[] Searches { get; set; }

        public string Name
        {
            get => _name;
            private set
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