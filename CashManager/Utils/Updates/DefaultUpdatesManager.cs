using System.Threading.Tasks;

namespace CashManager.Utils.Updates
{
    internal class DefaultUpdatesManager : IUpdatesManager
    {
        #region IUpdatesManager

        public void HandleEvents() { }

        public async Task HandleApplicationUpdatesCheck() { }

        #endregion
    }
}