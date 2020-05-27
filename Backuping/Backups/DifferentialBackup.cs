using System.IO;

using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public class DifferentialBackup : RetentionalBackup {
        public DifferentialBackup(string source, string target, int jobID, IFileSystemAPI fileSystemAPI, int retention)
            : base(source, target, jobID, fileSystemAPI, retention) { }
        protected override void BackupAlgorithm() {
            Progress.Report(new BackupProgress() { Percentage = 0, Status = "Started differential backup" });

            string last = null;

            Snapshot snapshot;
            if (last == null)
                snapshot = new Snapshot(Target) { Name = FileSystemAPI.GetFileName(Source)};
            else
                snapshot = LoadSnapshot();
            
            CopyChangedFiles(Source, snapshot);

            FileSystemAPI.CreateDirectory(FileSystemAPI.CombinePath(Target, ".BackupService"));

            SaveSnapshot(new Snapshot(Target) { Name = snapshot.Name });

            Progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
    }
}