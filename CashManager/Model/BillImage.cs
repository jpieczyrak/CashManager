using CashManager.Data.Extensions;
using CashManager.Model.Common;

namespace CashManager.Model
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