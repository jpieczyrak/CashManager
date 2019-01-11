using System.Windows;
using System.Windows.Controls;

using CashManager_MVVM.Model.Selectors;

namespace CashManager_MVVM.Features.Common
{
    public partial class DateRangePicker : UserControl
    {
        public static readonly DependencyProperty DateFrameProperty = DependencyProperty.Register(
            nameof(DateFrame),
            typeof(DateFrame),
            typeof(DateRangePicker));

        public DateFrame DateFrame
        {
            get => (DateFrame) GetValue(DateFrameProperty);
            set => SetValue(DateFrameProperty, value);
        }

        public DateRangePicker()
        {
            InitializeComponent();
        }
    }
}