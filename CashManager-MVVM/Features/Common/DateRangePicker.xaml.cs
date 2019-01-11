using System;
using System.Windows;
using System.Windows.Controls;

using CashManager_MVVM.Model.Selectors;

namespace CashManager_MVVM.Features.Common
{
    public partial class DateRangePicker : UserControl
    {
        private const string DATE_FRAME_PROPERTY_NAME = "DateFrame";

        public static readonly DependencyProperty DateFrameProperty = DependencyProperty.Register(
            DATE_FRAME_PROPERTY_NAME,
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