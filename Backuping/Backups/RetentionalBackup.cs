using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups {
    public abstract class RetentionalBackup : Backup {
        public int Retention { get; set; }
        public RetentionalBackup(string source, string target, int jobID, IFileSystemAPI fileSystemAPI, int retention)
            : base(source, target, jobID, fileSystemAPI) {
            this.Retention = retention;
        }
    }
}
