namespace CashManager.Data.DTO
{
    public class StoredFileInfo : Dto
    {
        public string DisplayName { get; set; }

        public string DbAlias { get; set; }

        public string SourceName { get; set; }
    }
}