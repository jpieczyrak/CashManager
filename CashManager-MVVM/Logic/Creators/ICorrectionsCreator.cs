using CashManager.WPF.Model;

namespace CashManager.WPF.Logic.Creators
{
    public interface ICorrectionsCreator
    {
        void CreateCorrection(Stock stock, decimal diff);
    }
}