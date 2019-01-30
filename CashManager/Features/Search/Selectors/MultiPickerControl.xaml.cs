using System.Windows;
using System.Windows.Controls;

namespace CashManager.Features.Search.Selectors
{
    public partial class MultiPickerControl : UserControl
    {
        public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            nameof(BorderThickness), typeof(Thickness), typeof(MultiPickerControl), new PropertyMetadata(default(Thickness)));

        public Thickness BorderThickness
        {
            get => (Thickness) GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }
        public MultiPickerControl()
        {
            InitializeComponent();
        }
    }
}
