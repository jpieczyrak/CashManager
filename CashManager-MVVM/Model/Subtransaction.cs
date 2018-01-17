using System;

using GalaSoft.MvvmLight;

using Logic.Model;

namespace CashManager_MVVM.Model
{
    public class Subtransaction : ObservableObject
    {
        private double _value;
        private Category _category;
        private string _title;

        public string Title
        {
            get => _title;
            set => Set(nameof(Title), ref _title, value);
        }

        public double Value
        {
            get => _value;
            set => Set(nameof(Value), ref _value, value);
        }

        public Category Category
        {
            get => _category;
            set => Set(nameof(Category), ref _category, value);
        }

        public Guid Id { get; private set; } = Guid.NewGuid();

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