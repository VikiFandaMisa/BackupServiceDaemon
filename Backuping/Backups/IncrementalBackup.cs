using System.IO;
using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public class IncrementalBackup : RetentionalBackup {
        public IncrementalBackup(string source, string target, int jobID, IFileSystemAPI fileSystemAPI, int retention)
            : base(source, target, jobID, fileSystemAPI, retention) { }
        protected override void BackupAlgorithm() {
            Progress.Report(new BackupProgress() { Percentage = 0, Status = "Started incremental backup" });
            
            string last = null;

            Snapshot snapshot;
            if (last == null)
                snapshot = new Snapshot(Target) { Name = Path.GetFileName(Source)};
            else
                snapshot = LoadSnapshot();

            CopyChangedFiles(Source, snapshot);

            // BROKEN
            SaveSnapshot(new Snapshot(Target) { Name = snapshot.Name });

            Progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
    }
}