using CashManager.Data.Extensions;

using CashManager_MVVM.Features.Search;
using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Logic.Balances
{
    public class CustomBalance : BaseObservableObject
    {
        public string Name { get; private set; }

        public SearchState[] Searches { get; set; }

        public CustomBalance(string name)
        {
            Name = name;
            Id = Name.GenerateGuid();
            Searches = new SearchState[0];
        }
    }
}