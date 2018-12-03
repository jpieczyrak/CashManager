using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using CashManager_MVVM.Model;
using CashManager_MVVM.Utils;

namespace CashManager_MVVM.Features.Categories
{
    public partial class CategoryManagerView : UserControl
    {
        private bool _isDragging;

        public CategoryManagerView()
        {
            InitializeComponent();
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(typeof(Category)) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Category)))
            {
                var sourceCategory = (Category) e.Data.GetData(typeof(Category));
                if (sourceCategory != null)
                {
                    var targetCategory = TreeViewHelper.GetObjectAtLocation<Category>(e.GetPosition(this), treeView);

                    (DataContext as CategoryManagerViewModel)?.Move(sourceCategory, targetCategory);
                }
            }
        }

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                DragDrop.DoDragDrop(treeView, treeView.SelectedValue, DragDropEffects.Move);
            }

            if (_isDragging && e.LeftButton != MouseButtonState.Pressed) _isDragging = false;
        }
    }
}