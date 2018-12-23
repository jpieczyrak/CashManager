using System;
using System.Linq;

using CashManager_MVVM.Features.Common;
using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    public sealed class Position : BaseObservableObject, IBookable
    {
        private PaymentValue _value;
        private Category _category;
        private string _title;
        private Tag[] _tags;
        private Transaction _parent;

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
                RaisePropertyChanged(nameof(GrossValueGuiString));
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
            set
            {
                Set(nameof(Tags), ref _tags, value);
                RaisePropertyChanged(nameof(TagsGuiString));
            }
        }

        public string TagsGuiString => string.Join(", ", Tags.OrderBy(x => x.Name));

        public MultiComboBoxViewModel TagViewModel { get; set; }

        public DateTime BookDate
        {
            get => Parent?.BookDate ?? DateTime.MinValue;
            set { }
        }

        /// <summary>
        /// Mass replacer purpose only
        /// </summary>
        public Transaction Parent
        {
            get => _parent;
            set => Set(nameof(Parent), ref _parent, value);
        }

        public bool Income => Parent.Type.Income && !Parent.Type.Outcome;

        public bool Outcome => !Parent.Type.Income && Parent.Type.Outcome;

        public string GrossValueGuiString => $"{(Outcome ? "-" : string.Empty)}{Value.GrossValue:F} zł";

        public Position()
        {
            Tags = new Tag[0];
            _value = new PaymentValue();
        }
    }
}