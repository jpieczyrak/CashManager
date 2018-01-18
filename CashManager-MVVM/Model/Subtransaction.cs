using System;

using GalaSoft.MvvmLight;

using Logic.Utils;

namespace CashManager_MVVM.Model
{
    public class Subtransaction : ObservableObject
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

        public Guid Id { get; private set; } = Guid.NewGuid();

        public Subtransaction()
        {
            Tags = new TrulyObservableCollection<Tag>();
        }

        #region Override

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetHashCode().Equals(GetHashCode());
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        #endregion
    }
}