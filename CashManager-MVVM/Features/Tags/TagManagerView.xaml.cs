using System.Windows.Controls;
using System.Windows.Input;

namespace CashManager.WPF.Features.Tags
{
    public partial class TagManagerView : UserControl
    {
        public TagManagerView()
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
