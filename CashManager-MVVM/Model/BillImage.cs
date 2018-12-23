using CashManager.Data.Extensions;

using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
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