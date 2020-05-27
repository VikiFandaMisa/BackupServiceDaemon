using System.IO;
using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public class IncrementalBackup : RetentionalBackup {
        public IncrementalBackup(string source, string target, IFileSystemAPI fileSystemAPI, int retention)
            : base(source, target, fileSystemAPI, retention) { }
        protected override void BackupAlgorithm() {
            Progress.Report(new BackupProgress() { Percentage = 0, Status = "Started incremental backup" });
            
            string target = Utils.GetTarget("Inc_", Target, Source);
            string last = Utils.GetLastBackup(Target, Path.GetFileName(Source));

            FileSystemAPI.CreateDirectory(target);

            Snapshot snapshot;
            if (last == null)
                snapshot = new Snapshot(target) { Name = Path.GetFileName(Source)};
            else
                snapshot = Utils.LoadSnapshot(last);

            Utils.CopyChangedFiles(Source, target, snapshot, FileSystemAPI);

            // BROKEN
            (new Snapshot(target) { Name = snapshot.Name }).Save(target);

            Progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
    }
}