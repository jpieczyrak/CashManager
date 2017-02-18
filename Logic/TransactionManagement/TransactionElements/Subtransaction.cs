using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using Logic.Properties;

namespace Logic.TransactionManagement.TransactionElements
{
    [DataContract(Namespace = "")]
    public class Subtransaction : INotifyPropertyChanged
    {
        private double _value;
        
        public Subtransaction()
        {
        }

        public Subtransaction(string name, double value)
        {
            _value = value;
            Name = name;
        }

        public Subtransaction(string name, double value, string category, string tags)
        {
            _value = value;
            Name = name;
            Category = new StringWrapper(category);
            Tags = tags;
        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
        [DataMember]
        public StringWrapper Category { get; set; }
        [DataMember]
        public string Tags { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}