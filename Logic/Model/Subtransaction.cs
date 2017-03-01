using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using Logic.LogicObjectsProviders;
using Logic.Properties;

namespace Logic.Model
{
    [DataContract(Namespace = "")]
    public class Subtransaction : INotifyPropertyChanged
    {
        private double _value;

        public string Name { get; set; }

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

        public string Tags { get; set; }

        public Guid Id { get; private set; } = Guid.NewGuid();

        private Subtransaction() { }

        public Subtransaction(string name, double value)
        {
            _value = value;
            Name = name;
        }

        public Subtransaction(string name, double value, string category, string tags)
        {
            _value = value;
            Name = name;
            Category = CategoryProvider.FindOrCreate(category);
            Tags = tags;
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