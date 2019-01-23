using CashManager_MVVM.Model;

namespace CashManager_MVVM.Logic.Creators
{
    public interface ICorrectionsCreator
    {
        void CreateCorrection(Stock stock, decimal diff);
    }
}