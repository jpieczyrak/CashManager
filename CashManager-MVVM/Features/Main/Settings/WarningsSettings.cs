using System.Linq;

using GalaSoft.MvvmLight;

namespace CashManager_MVVM.Features.Main.Settings
{
    internal class WarningsSettings : ObservableObject
    {
        public bool? AllSelected
        {
            get
            {
                var allWarnings = new[]
                {
                    QuestionForCategoryDelete,
                    QuestionForPositionDelete,
                    QuestionForTransactionDelete
                };
                return allWarnings.All(x => x)
                           ? true
                           : allWarnings.Any(x => x)
                               ? (bool?) null
                               : false;
            }
            set
            {
                if (AllSelected == value) return;
                QuestionForTransactionDelete = QuestionForPositionDelete = QuestionForCategoryDelete = value ?? false;
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

        public bool QuestionForPositionDelete
        {
            get => Properties.Settings.Default.QuestionForPositionDelete;
            set
            {
                if (Properties.Settings.Default.QuestionForPositionDelete == value) return;
                Properties.Settings.Default.QuestionForPositionDelete = value;
                RaisePropertyChanged(nameof(QuestionForPositionDelete));
                RaisePropertyChanged(nameof(AllSelected));
            }
        }

        public bool QuestionForTransactionDelete
        {
            get => Properties.Settings.Default.QuestionForTransactionDelete;
            set
            {
                if (Properties.Settings.Default.QuestionForTransactionDelete == value) return;
                Properties.Settings.Default.QuestionForTransactionDelete = value;
                RaisePropertyChanged(nameof(QuestionForTransactionDelete));
                RaisePropertyChanged(nameof(AllSelected));
            }
        }
    }
}