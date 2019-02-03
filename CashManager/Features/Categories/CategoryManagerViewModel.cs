using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Categories;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Categories;
using CashManager.Logic.DefaultData.InputParsers;
using CashManager.Model;
using CashManager.Properties;
using CashManager.UserCommunication;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using GongSolutions.Wpf.DragDrop;

using DtoCategory = CashManager.Data.DTO.Category;

namespace CashManager.Features.Categories
{
    public class CategoryManagerViewModel : ViewModelBase, IDropTarget
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMessagesService _messagesService;
        private string _input;
        private ExpandableCategory _selectedCategory;

        public TrulyObservableCollection<ExpandableCategory> Categories { get; private set; }

        public RelayCommand AddCategoryCommand => new RelayCommand(ExecuteAddCategoryCommand);

        public RelayCommand RemoveCategoryCommand => new RelayCommand(ExecuteRemoveCategoryCommand, CanExecuteRemoveCategoryCommand);
        public RelayCommand LoadCategoriesCommand => new RelayCommand(ExecuteLoadCategoriesCommand);

        public ExpandableCategory SelectedCategory
        {
            get => _selectedCategory;
            private set
            {
                Set(ref _selectedCategory, value);
                RaisePropertyChanged(nameof(RemoveCategoryCommand));
            }
        }

        public string Input
        {
            get => _input;
            set => Set(nameof(Input), ref _input, value);
        }

        public CategoryManagerViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher, IMessagesService messagesService)
        {
            _input = Strings.NewCategory;
            _commandDispatcher = commandDispatcher;
            _messagesService = messagesService;
            Categories = new TrulyObservableCollection<ExpandableCategory>();
            var categories = queryDispatcher.Execute<CategoryQuery, DtoCategory[]>(new CategoryQuery())
                                             .Select(Mapper.Map<ExpandableCategory>)
                                             .ToArray();

            AddCategoriesToTree(categories);

            SelectedCategory = categories.FirstOrDefault(x => x.IsSelected);
        }

        private void AddCategoriesToTree(ExpandableCategory[] categories)
        {
            foreach (var category in categories)
            {
                category.Children = new TrulyObservableCollection<ExpandableCategory>(categories.Where(x => x.Parent?.Id == category.Id).OrderBy(x => x.Name));
                category.PropertyChanged += CategoryOnPropertyChanged;
            }

            //find the root(s)
            Categories.AddRange(categories.Where(x => x.Parent == null && !Categories.Contains(x) && x.Id != Category.Default.Id)
                                          .OrderBy(x => x.Name));
        }

        private void Move(ExpandableCategory sourceCategory, ExpandableCategory targetCategory)
        {
            if (sourceCategory == null || targetCategory == null) return;
            if (sourceCategory.Id == targetCategory.Id) return;

            //target can not be a (grand)children
            if (Find(sourceCategory.Children.ToArray(), targetCategory.Id) != null) return;

            var sourceParentId = sourceCategory.Parent?.Id;

            //swap
            sourceCategory.Parent = targetCategory;
            targetCategory.Children.Add(sourceCategory);

            //remove from old position
            if (sourceParentId == null)
                Categories.Remove(sourceCategory);
            else
                Find(Categories.ToArray(), sourceParentId.Value)?.Children.Remove(sourceCategory);

            //update parent in db
            UpsertCategory(sourceCategory);
        }

        private ExpandableCategory Find(ExpandableCategory[] categories, Guid id)
        {
            foreach (var category in categories)
            {
                if (category.Id == id) return category;
                if (category.Children?.Any() ?? false)
                {
                    var result = Find(category.Children.ToArray(), id);
                    if (result != null) return result;
                }
            }

            return null;
        }

        private void ExecuteAddCategoryCommand()
        {
            var parent = SelectedCategory;
            var category = new ExpandableCategory { Name = Input };
            category.PropertyChanged += CategoryOnPropertyChanged;
            if (parent != null)
            {
                parent.Children.Add(category);
                category.Parent = parent;
            }
            else Categories.Add(category);
            UpsertCategory(category);
        }

        private void ExecuteRemoveCategoryCommand()
        {
            if (SelectedCategory?.Parent != null)
            {
                if (Settings.Default.QuestionForCategoryDelete)
                    if (!_messagesService.ShowQuestionMessage(Strings.Question, string.Format(Strings.QuestionDoYouWantToDeleteCategoryFormat, SelectedCategory.Name)))
                        return;

                var input = Categories.ToArray();
                var selected = Find(input, SelectedCategory.Id);
                var parent = Find(input, selected.Parent.Id);
                foreach (var child in selected.Children)
                {
                    child.Parent = parent;
                    parent.Children.Add(child);
                }

                parent.Children.Remove(selected);
                UpsertCategories(selected.Children.ToArray());
                _commandDispatcher.Execute(new DeleteCategoryCommand(Mapper.Map<DtoCategory>(selected)));
                //todo: find all tran.positions which used selected category and change it for parent. then save.
            }
        }

        private bool CanExecuteRemoveCategoryCommand() => SelectedCategory?.Parent != null;

        private void ExecuteLoadCategoriesCommand()
        {
            var parser = new CategoryParser();
            var result = parser.Parse(Input);
            if (result != null && result.Any())
            {
                var categories = Mapper.Map<ExpandableCategory[]>(result);
                AddCategoriesToTree(categories);
                UpsertCategories(result);
            }
        }

        private void UpsertCategory(ExpandableCategory category)
        {
            var categories = new [] { category };
            UpsertCategories(categories);
        }

        private void UpsertCategories(DtoCategory[] categories)
        {
            _commandDispatcher.Execute(new UpsertCategoriesCommand(categories));
        }

        private void UpsertCategories(ExpandableCategory[] categories)
        {
            UpsertCategories(Mapper.Map<DtoCategory[]>(categories));
        }

        private void CategoryOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is ExpandableCategory category && category.IsSelected) SelectedCategory = category;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.Effects = dropInfo.Data is ExpandableCategory
                                   ? DragDropEffects.Copy
                                   : DragDropEffects.None;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is ExpandableCategory source && dropInfo.TargetItem is ExpandableCategory target)
            {
                if (Settings.Default.QuestionForCategoryMove)
                    if (!_messagesService.ShowQuestionMessage(Strings.Question, string.Format(Strings.QuestionDoYouWantToMoveCategoryFormat, source.Name, target.Name)))
                        return;

                Move(source, target);
            }
        }
    }
}