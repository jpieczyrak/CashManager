using GalaSoft.MvvmLight;

namespace CashManager.Model
{
    public class Note : ObservableObject
    {
        private string _value = string.Empty;

        public string Value
        {
            get => _value;
            set => Set(ref _value, value);
        }

        public Note(string value) { Value = value; }

        public Note() { }

        #region Override

        public override bool Equals(object obj) => (obj as Note)?.Value == Value;

        public override int GetHashCode() => Value?.GetHashCode() ?? 0;

        public override string ToString() => Value;

        #endregion
    }
}