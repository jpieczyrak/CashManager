using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main.Settings
{
    internal class WarningsSettings : ObservableObject
    {
        public bool? AllSelected
        {
            get => QuestionForCategoryDelete;
            set
            {
                if (AllSelected == value) return;
                QuestionForCategoryDelete = value ?? false;
                RaisePropertyChanged();
            }
        }

        public bool QuestionForCategoryDelete
        {
            get => Properties.Settings.Default.QuestionForCategoryDelete;
            set
            {
                if (Properties.Settings.Default.QuestionForCategoryDelete == value) return;
                Properties.Settings.Default.QuestionForCategoryDelete = value;
                RaisePropertyChanged(nameof(QuestionForCategoryDelete));
                RaisePropertyChanged(nameof(AllSelected));
            }
        }

        internal WarningsSettings() { }
    }
}