using System.IO;
using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public class IncrementalBackup : RetentionalBackup {
        public IncrementalBackup(string source, string target, int jobID, IFileSystemAPI fileSystemAPI, int retention)
            : base(source, target, jobID, fileSystemAPI, retention) { }
        protected override void BackupAlgorithm() {
            Progress.Report(new BackupProgress() { Percentage = 0, Status = "Started incremental backup" });
            
            Snapshot snapshot = LoadSnapshot();
            
            var addedDeleted = CopyChangedFiles(snapshot);

            snapshot.Union(addedDeleted.Item1);
            snapshot.Subtract(addedDeleted.Item2);

            SaveSnapshot(snapshot);

            Progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
    }
}