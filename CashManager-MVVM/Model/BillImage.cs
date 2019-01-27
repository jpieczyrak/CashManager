using CashManager.Data.Extensions;

using CashManager.WPF.Model.Common;

namespace CashManager.WPF.Model
{
    public class BillImage : BaseObservableObject
    {
        public string DisplayName { get; }

        public byte[] Image { get; }

        public BillImage(string sourceName, string displayName, byte[] image)
        {
            DisplayName = displayName;
            Id = sourceName.GenerateGuid();
            Image = image;
        }
    }
}