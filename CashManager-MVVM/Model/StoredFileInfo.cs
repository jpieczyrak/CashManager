using System;
using System.IO;

using CashManager.Data.Extensions;

using CashManager_MVVM.Model.Common;

namespace CashManager_MVVM.Model
{
    public class StoredFileInfo : BaseObservableObject
    {
        private string _displayName;

        /// <summary>
        /// Gui display name
        /// </summary>
        public string DisplayName
        {
            get => _displayName;
            set => Set(nameof(DisplayName), ref _displayName, value);
        }

        /// <summary>
        /// Location where the files is stored in db
        /// </summary>
        public string DbAlias { get; private set; }

        /// <summary>
        /// Original file path used only before file is saved to db
        /// </summary>
        public string SourceName { get; private set; }

        private StoredFileInfo() { }

        public StoredFileInfo(string file, Guid transactionId)
        {
            SourceName = file;
            DisplayName = Path.GetFileNameWithoutExtension(file);
            Id = DisplayName.GenerateGuid();
            DbAlias = CreateDbFileAlias(file, transactionId);
        }

        public static string CreateDbFileAlias(string path, Guid transactionId)
        {
            return $"bills/{transactionId}/{Path.GetFileNameWithoutExtension(path)?.Replace(" ", string.Empty)}";
        }
    }
}