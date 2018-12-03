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
            // Let ComboBox's original method handle it
            base.OnPreviewKeyDown(e);

            // Manually check if the ComboBox handled an "Enter" key
            // If yes, set e.Handled back to false so that Command binding can consume the same event
            if (e.Key == Key.Enter)
                e.Handled = false;
        }
    }
}
