using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public abstract class RetentionalBackup : Backup
    {
        public int Retention { get; set; }
        public RetentionalBackup(string source, string target, IFileSystemAPI fileSystemAPI, int retention) : base(source, target, fileSystemAPI) {
            this.Retention = retention;
        }
    }
}