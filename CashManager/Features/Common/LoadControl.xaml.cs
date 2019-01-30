using System.Windows;
using System.Windows.Controls;

using CashManager.Model.Common;

using GalaSoft.MvvmLight.CommandWpf;

namespace CashManager.Features.Common
{
    public partial class LoadControl : UserControl
    {
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
            nameof(IsOpen), typeof(bool), typeof(LoadControl), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            nameof(Command), typeof(RelayCommand<BaseObservableObject>), typeof(LoadControl), new PropertyMetadata(default(RelayCommand<BaseObservableObject>)));

        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            nameof(Element), typeof(BaseObservableObject), typeof(LoadControl), new PropertyMetadata(default(BaseObservableObject)));

        public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register(
            nameof(Elements), typeof(BaseObservableObject[]), typeof(LoadControl), new PropertyMetadata(default(BaseObservableObject[])));

        public bool IsOpen
        {
            get => (bool) GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        public RelayCommand<BaseObservableObject> Command
        {
            get => (RelayCommand<BaseObservableObject>) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public BaseObservableObject Element
        {
            get => (BaseObservableObject) GetValue(ElementProperty);
            set => SetValue(ElementProperty, value);
        }

        public BaseObservableObject[] Elements
        {
            get => (BaseObservableObject[]) GetValue(ElementsProperty);
            set => SetValue(ElementsProperty, value);
        }

        public LoadControl() { InitializeComponent(); }

        private void ButtonOpenClick(object sender, RoutedEventArgs e) { IsOpen = true; }

        private void ButtonCloseClick(object sender, RoutedEventArgs e) { IsOpen = false; }
    }
}