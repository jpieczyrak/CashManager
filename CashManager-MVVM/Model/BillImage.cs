namespace CashManager_MVVM.Model
{
    public class BillImage
    {
        public string DisplayName { get; }

        public byte[] Image { get; }

        public BillImage(string displayName, byte[] image)
        {
            DisplayName = displayName;
            Image = image;
        }
    }
}