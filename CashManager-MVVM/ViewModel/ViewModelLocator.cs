/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:CashManager_MVVM"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CashManager_MVVM.Model.DataProviders;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

namespace CashManager_MVVM.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

			if (ViewModelBase.IsInDesignModeStatic) SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
			else SimpleIoc.Default.Register<IDataService, DataService>();


            SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<TransactionViewModel>();
			SimpleIoc.Default.Register<CategoriesViewModel>();
        }
		
		/// <summary>
		/// Gets the Main property.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
		public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
		public TransactionViewModel Transaction => SimpleIoc.Default.GetInstance<TransactionViewModel>();

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
			"CA1822:MarkMembersAsStatic", Justification = "This non-static member is needed for data binding purposes.")]
		public CategoriesViewModel Categories => SimpleIoc.Default.GetInstance<CategoriesViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}