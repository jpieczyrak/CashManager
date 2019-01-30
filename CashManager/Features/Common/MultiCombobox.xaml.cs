using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace CashManager.Features.Common
{
    public partial class MultiComboBox : UserControl
    {
        public MultiComboBox() { InitializeComponent(); }

        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (!(DataContext is MultiComboBoxViewModel vm)) return;

                var matching = vm.InternalDisplayableSearchResults.Where(x => x.Name.ToLower().Contains(vm.Text.ToLower()));
                foreach (var selectable in matching) selectable.IsSelected = true;

                return;
            }

            OnPreviewKeyDown(e);
        }
    }
}