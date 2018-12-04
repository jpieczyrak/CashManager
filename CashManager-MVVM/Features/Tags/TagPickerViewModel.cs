using System.Collections.Generic;
using System.Linq;

using AutoMapper;

using CashManager.Infrastructure.Command;
using CashManager.Infrastructure.Command.Tags;
using CashManager.Infrastructure.Query;
using CashManager.Infrastructure.Query.Tags;

using CashManager_MVVM.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using DtoTag = CashManager.Data.DTO.Tag;

namespace CashManager_MVVM.Features.Tags
{
    /// <summary>
    /// Obsolete
    /// </summary>
    public class TagPickerViewModel : ViewModelBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly TrulyObservableCollection<Tag> _tags;
        private string _text;
        private TrulyObservableCollection<Tag> _observableTags;

        public TrulyObservableCollection<Tag> Tags
        {
            get => _observableTags;
            private set => Set(nameof(Tags), ref _observableTags, value);
        }

        public Tag SelectedTag { get; set; }

        public Tag[] SelectedTags => _tags.Where(x => x.IsSelected).ToArray();

        public string SelectedTagsString => string.Join(", ", SelectedTags.OrderBy(x => x.Name));

        public string Text
        {
            get => _text;
            set
            {
                Set(nameof(Text), ref _text, value);
                var tags = _tags.Where(x => x.Name.ToLower().Contains(_text.ToLower())).OrderBy(x => !x.IsSelected).ThenBy(x => x.Name);
                Tags = new TrulyObservableCollection<Tag>(tags);
            }
        }

        public RelayCommand AddNewTagCommand { get; }

        public TagPickerViewModel(IQueryDispatcher queryDispatcher, ICommandDispatcher commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
            AddNewTagCommand = new RelayCommand(ExecuteAddNewTagCommand, CanExecuteAddNewTagCommand);
            var dtos = _queryDispatcher.Execute<TagQuery, DtoTag[]>(new TagQuery());
            _tags = new TrulyObservableCollection<Tag>(Mapper.Map<Tag[]>(dtos).OrderBy(x => !x.IsSelected).ThenBy(x => x.Name));
            foreach (var tag in _tags) tag.IsSelected = false;
            Tags = new TrulyObservableCollection<Tag>(_tags);
            _tags.CollectionChanged += (sender, args) => RaisePropertyChanged(nameof(SelectedTagsString));
        }

        public void SelectTags(IEnumerable<Tag> tags)
        {
            var common = _tags.Zip(tags, (source, updater) =>
            {
                source.IsSelected = updater.IsSelected;
                return source;
            }).ToArray();
        }

        private bool CanExecuteAddNewTagCommand()
        {
            return SelectedTag == null && !string.IsNullOrWhiteSpace(_text)
                                       && !_tags.Select(x => x.Name.ToLower()).Contains(_text.ToLower());
        }

        private void ExecuteAddNewTagCommand()
        {
            var tag = new Tag { Name = Text, IsSelected = true };
            Tags.Add(tag);
            _tags.Add(tag);
            _commandDispatcher.Execute(new UpsertTagsCommand(Mapper.Map<DtoTag[]>(new[] { tag })));
            Text = string.Empty;
        }
    }
}