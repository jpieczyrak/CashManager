using CashManager.Features.Common;

namespace CashManager.Features.Transactions.Bills
{
    public partial class BillWindow : CustomWindow
    {
        public BillWindow(byte[] image)
        {
            InitializeComponent();
            DataContext = image;
        }
    }
}
