using System;
using System.Windows;
using System.Windows.Controls;

namespace CashManager_MVVM.Features.Common
{
    public partial class ExtendedDatePicker : UserControl
    {
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(nameof(SelectedValue), typeof(DateTime), typeof(ExtendedDatePicker),
                new FrameworkPropertyMetadata(DateTime.Today, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedValuePropertyChanged));

        public DateTime SelectedValue
        {
            get => (DateTime) GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value); //should update source (by design)
        }

        public ExtendedDatePickerViewModel ViewModel { get; private set; }

        public ExtendedDatePicker()
        {
            ViewModel = new ExtendedDatePickerViewModel(this);
            InitializeComponent();
        }

        private static void OnSelectedValuePropertyChanged(DependencyObject source,
            DependencyPropertyChangedEventArgs e)
        {
            //var control = source as ExtendedDatePicker;
            //var date = (DateTime) e.NewValue;
        }
    }
}