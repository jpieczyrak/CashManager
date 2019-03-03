using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

using CashManager.Features.Common;

using Microsoft.Win32;

namespace CashManager.Features.Transactions.Bills
{
    public partial class BillWindow : CustomWindow
    {
        public BillWindow(byte[] image)
        {
            InitializeComponent();
            DataContext = image;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var fileDialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".jpg",
                Filter = "JPG (.jpg)|*.jpg"
            };

            if (fileDialog.ShowDialog() == true)
                Image.FromStream(new MemoryStream(DataContext as byte[])).Save(fileDialog.FileName, ImageFormat.Jpeg);
        }
    }
}