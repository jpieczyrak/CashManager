using System.Linq;

using CashManager_MVVM.Features.Tags;

namespace CashManager_MVVM.Model
{
    public class Position : BaseObservableObject
    {
        private PaymentValue _value;
        private Category _category;
        private string _title;
        private Tag[] _tags;

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
        public Tag[] Tags
        {
            get => _tags;
            set => Set(nameof(Tags), ref _tags, value);
        }

        public TagPickerViewModel TagViewModel { get; set; }

        public Position()
        {
            Tags = new Tag[0];
            _value = new PaymentValue();
        }
    }
}