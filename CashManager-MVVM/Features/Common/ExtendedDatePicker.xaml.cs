using System;
using System.Windows;
using System.Windows.Controls;

namespace CashManager_MVVM.Features.Common
{
    public partial class ExtendedDatePicker : UserControl
    {
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(DateTime),
                typeof(ExtendedDatePicker), new FrameworkPropertyMetadata(DateTime.Today, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedValuePropertyChanged));

        public DateTime SelectedValue
        {
            get => (DateTime)GetValue(SelectedValueProperty);
            set
            {
                SetCurrentValue(SelectedValueProperty, value); //does not update source (by design)
                SetValue(SelectedValueProperty, value); //should update source (by design)
            }
        }

        public ExtendedDatePicker()
        {
            DataContext = new ExtendedDatePickerViewModel(this);
            InitializeComponent();
        }

        private static void OnSelectedValuePropertyChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            var control = source as ExtendedDatePicker;
            var date = (DateTime)e.NewValue;
            var vm = control.DataContext as ExtendedDatePickerViewModel;

            //todo: random update source manually?
        }
    }
}
