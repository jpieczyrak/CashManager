namespace CashManager_MVVM.Model
{
    public class Position : BaseObservableObject
    {
        private TrulyObservableCollection<Tag> _tags;

        private PaymentValue _value;
        private Category _category;
        private string _title;

        public string Title
        {
            get => _title;
            set => Set(nameof(Title), ref _title, value);
        }

        public PaymentValue Value
        {
            get => _value;
            set
            {
                Set(nameof(Value), ref _value, value);
                Value.PropertyChanged += (sender, args) => RaisePropertyChanged(nameof(Value));
            }
        }

        public Category Category
        {
            get => _category;
            set => Set(nameof(Category), ref _category, value);
        }

        /// <summary>
        /// Optional tags for whole transaction (like: buying PC 2015)
        /// </summary>
        public TrulyObservableCollection<Tag> Tags
        {
            get => _tags;
            set
            {
                Set(nameof(Tags), ref _tags, value);
                _tags.CollectionChanged += (sender, args) => RaisePropertyChanged(nameof(Tags));
            }
        }

        public string TagsForGUI => string.Join(", ", Tags);
        
        public Position()
        {
            Tags = new TrulyObservableCollection<Tag>();
            Value = new PaymentValue();
        }
    }
}