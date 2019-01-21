using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main.Settings
{
    internal class WarningsSettings : ObservableObject
    {
        public bool QuestionForCategoryDelete
        {
            get => Properties.Settings.Default.QuestionForCategoryDelete;
            set => Properties.Settings.Default.QuestionForCategoryDelete = value;
        }

        internal WarningsSettings() { }
    }
}