using System.IO;

using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public class FullBackup : Backup {
        public FullBackup(string source, string target, int jobID, IFileSystemAPI fileSystemAPI)
            : base(source, target, jobID, fileSystemAPI) { }
        protected override void BackupAlgorithm() {
            Progress.Report(new BackupProgress() { Percentage = 0, Status = "Started full backup" });

            CopyChangedFiles(new Snapshot(Path.GetFileName(Source)));

            Progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
    }    
}