using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Logic.LogicObjectsProviders;
using Logic.Properties;

namespace Logic.Model
{
    public class Position : INotifyPropertyChanged
    {
        private double _value;

        public string Title { get; set; }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public Category Category { get; set; }
        
        public Guid Id { get; private set; } = Guid.NewGuid();

        private Position() { }

        public Position(string title, double value)
        {
            _value = value;
            Title = title;
        }

        public Position(string title, double value, string category)
        {
            _value = value;
            Title = title;
            Category = CategoryProvider.FindOrCreate(category);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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