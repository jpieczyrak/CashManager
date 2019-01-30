using System.Windows;
using System.Windows.Controls;

using CashManager.Model.Selectors;

namespace CashManager.Features.Common
{
    public partial class DateRangePicker : UserControl
    {
        public static readonly DependencyProperty DateFrameSelectorProperty = DependencyProperty.Register(
            nameof(DateFrameSelector),
            typeof(DateFrameSelector),
            typeof(DateRangePicker));

        public DateFrameSelector DateFrameSelector
        {
            get => (DateFrameSelector) GetValue(DateFrameSelectorProperty);
            set => SetValue(DateFrameSelectorProperty, value);
        }

        public DateRangePicker()
        {
            ViewModel = new DateRangePickerViewModel(this);
            InitializeComponent();
        }

        public DateRangePickerViewModel ViewModel { get; private set; }
    }
}