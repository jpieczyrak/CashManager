using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Model.Filters
{
    public class BaseFilter : ObservableObject
    {
        public string Title { get; protected set; }
    }
}