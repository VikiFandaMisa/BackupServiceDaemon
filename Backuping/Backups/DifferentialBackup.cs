using System.IO;

using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public class DifferentialBackup : RetentionalBackup {
        public DifferentialBackup(string source, string target, IFileSystemAPI fileSystemAPI, int retention)
            : base(source, target, fileSystemAPI, retention) { }
        protected override void BackupAlgorithm() {
            Progress.Report(new BackupProgress() { Percentage = 0, Status = "Started differential backup" });

            string target = Utils.GetTarget("Diff_", Target, Source);
            string last = Utils.GetLastBackup(Target, Path.GetFileName(Source));

            FileSystemAPI.CreateDirectory(target);

            Snapshot snapshot;
            if (last == null)
                snapshot = new Snapshot(target) { Name = Path.GetFileName(Source)};
            else
                snapshot = Utils.LoadSnapshot(last);
            
            Utils.CopyChangedFiles(Source, target, snapshot, FileSystemAPI);

            FileSystemAPI.CreateDirectory(FileSystemAPI.CombinePath(target, ".BackupService"));

            if (last == null)
                (new Snapshot(target) { Name = snapshot.Name }).Save(target);
            else
                File.Copy(Path.Combine(last, ".BackupService", "snapshot.json"), Path.Combine(target, ".BackupService", "snapshot.json"));

            Progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
    }
}