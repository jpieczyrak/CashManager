using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

using CashManager.Properties;

using log4net;

namespace CashManager.Utils
{
    public class DatabaseBackuper
    {
        private const string DIR = "BACKUPS";
        private const string DB_FILE_NAME = "results.litedb";
        private static readonly Lazy<ILog> _logger = new Lazy<ILog>(() => LogManager.GetLogger(typeof(DatabaseBackuper)));
        private readonly string _databaseFilepath;

        public DatabaseBackuper(string databaseFilepath) { _databaseFilepath = databaseFilepath; }

        public void Backup()
        {
            string path = Path.Combine(Path.GetDirectoryName(_databaseFilepath), DIR, $"{DateTime.Now:yyyy-MM-dd HH-mm-ss-fff} - results.zip");
            try
            {
                if (!Directory.Exists(DIR)) Directory.CreateDirectory(DIR);

                using (var zip = ZipFile.Open(path, ZipArchiveMode.Create))
                {
                    SafelyAddFileToZip(zip, SafelyReadFileContent(_databaseFilepath));
                }

                _logger.Value.Debug($"Backup created: {path}");
            }
            catch (Exception e)
            {
                _logger.Value.Info("Could not create backup", e);
            }

            CleanupOldBackups();
        }

        private string SafelyReadFileContent(string databaseFilepath)
        {
            try
            {
                using (var stream = File.Open(databaseFilepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Value.Info("Could not read database file", e);
            }

            return string.Empty;
        }

        private static void SafelyAddFileToZip(ZipArchive zip, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return;

            var entry = zip.CreateEntry(DB_FILE_NAME);
            using (var stream = entry.Open())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(content);
                    writer.Flush();
                }
            }
        }

        private static void CleanupOldBackups()
        {
            var files = Directory.GetFiles(DIR, "*.zip");
            if (files.Length > Settings.Default.KeepDatabaseBackupsCount)
            {
                var oldest = files.Select(x => new FileInfo(x)).OrderByDescending(x => x.CreationTime).Skip(Settings.Default.KeepDatabaseBackupsCount).ToArray();

                try
                {
                    foreach (var fileInfo in oldest) fileInfo.Delete();
                }
                catch (Exception e)
                {
                    _logger.Value.Info("Could not delete backups", e);
                }
                _logger.Value.Info($"Removed: {oldest.Length} backups");
            }
        }
    }
}