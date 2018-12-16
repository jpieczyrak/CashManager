using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Utils
{
    public class TreeViewHelper
    {
        public static T GetObjectAtLocation<T>(Point location, TreeView treeView) where T : BaseObservableObject
        {
            var result = default(T);
            var hitTestResults = VisualTreeHelper.HitTest(treeView, location);

            if (hitTestResults.VisualHit is FrameworkElement element)
            {
                var dataObject = element.DataContext;

                if (dataObject is T variable) result = variable;
            }

            return result;
        }
    }
}