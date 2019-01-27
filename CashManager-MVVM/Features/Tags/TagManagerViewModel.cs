using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Tags;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Tags;

using CashManager.WPF.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoTag = CashManager.Data.DTO.Tag;

namespace CashManager.WPF.Features.Tags
{
    public class TagManagerViewModel : ViewModelBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private TrulyObservableCollection<Tag> _observableTags;
        private string _tagName;

        public TrulyObservableCollection<Tag> Tags
        {
            get => _observableTags;
            private set => Set(nameof(Tags), ref _observableTags, value);
        }

        public Tag SelectedTag { get; set; }

        public RelayCommand AddCommand { get; }

        public RelayCommand<Tag> RemoveCommand { get; }

        public string TagName
        {
            get => _tagName;
            set => Set(nameof(_tagName), ref _tagName, value);
        }

        public TagManagerViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;

            AddCommand = new RelayCommand(ExecuteAddCommand, CanExecuteAddCommand);
            RemoveCommand = new RelayCommand<Tag>(ExecuteRemoveCommand);

            var dtos = queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery());
            _observableTags = new TrulyObservableCollection<Tag>(Mapper.Map<Tag[]>(dtos).OrderBy(x => x.Name));
            Tags = new TrulyObservableCollection<Tag>(_observableTags);
        }

        private void ExecuteRemoveCommand(Tag tag)
        {
            _commandDispatcher.Execute(new DeleteTagCommand(Mapper.Map<DtoTag>(tag)));
            Tags.Remove(tag);
        }

        private bool CanExecuteAddCommand()
        {
            return SelectedTag == null && !string.IsNullOrWhiteSpace(_tagName)
                                       && !_observableTags.Select(x => x.Name.ToLower()).Contains(_tagName.ToLower());
        }

        private void ExecuteAddCommand()
        {
            var tag = new Tag { Name = _tagName };
            Tags.Add(tag);
            _commandDispatcher.Execute(new UpsertTagsCommand(Mapper.Map<DtoTag[]>(new[] { tag })));
        }
    }
}