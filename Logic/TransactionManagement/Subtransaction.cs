using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logic.Annotations;

namespace Logic.TransactionManagement
{
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

        public string Category { get; set; }

        public string Tags { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}