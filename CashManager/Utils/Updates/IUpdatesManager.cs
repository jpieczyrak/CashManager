using System.Threading.Tasks;

namespace CashManager.Utils.Updates
{
    internal interface IUpdatesManager
    {
        Task HandleApplicationUpdatesCheck();

        void HandleEvents();
    }
}