using CashManager.Model;

namespace CashManager.Logic.Creators
{
    public interface ICorrectionsCreator
    {
        void CreateCorrection(Stock stock, decimal diff, string title);
    }
}