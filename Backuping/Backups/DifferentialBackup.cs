using System.IO;

using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups {
    public class DifferentialBackup : RetentionalBackup {
        public DifferentialBackup(string source, string target, int jobID, IFileSystemAPI fileSystemAPI, int retention)
            : base(source, target, jobID, fileSystemAPI, retention) { }
        protected override void BackupAlgorithm() {
            Progress.Report(new BackupProgress() { Percentage = 0, Status = "Started differential backup" });

            Snapshot snapshot = LoadSnapshot();
            int number = LoadNumber() + 1;
            var first = false;

            if (snapshot == null || number > Retention) {
                first = true;
                number = 1;
                snapshot = new Snapshot("");
            }

            var addedDeleted = CopyChangedFiles(snapshot);

            if (first) {
                SaveSnapshot(addedDeleted.Item1);
            }
            SaveNumber(number);

            Progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
    }
}
