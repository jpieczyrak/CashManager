using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using Logic.Properties;

namespace Logic.Model
{
    [DataContract(Namespace = "")]
    public class Payment : INotifyPropertyChanged
    {
        private double _value;
        private Stock _source;
        private Stock _target;

        public Stock Source
        {
            get { return _source; }
            set
            {
                _source = value;
                OnPropertyChanged(nameof(Source));
            }
        }

        public Stock Target
        {
            get { return _target; }
            set
            {
                _target = value;
                OnPropertyChanged(nameof(Target));
            }
        }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        
        public Payment(Stock source, Stock target, double value)
        {
            _source = source;
            _target = target;
            _value = value;
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
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + Value.GetHashCode();
                hash = hash * 23 + Target.GetHashCode();
                hash = hash * 23 + Source.GetHashCode();

                return hash;
            }
        }

        #endregion
    }
}