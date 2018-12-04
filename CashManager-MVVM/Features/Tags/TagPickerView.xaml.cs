using System.Windows.Controls;
using System.Windows.Input;

namespace CashManager_MVVM.Features.Tags
{
    /// <summary>
    /// Interaction logic for TagPicker.xaml
    /// </summary>
    public partial class TagPickerView : UserControl
    {
        public TagPickerView()
        {
            InitializeComponent();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Enter) e.Handled = false;
        }
    }
}
