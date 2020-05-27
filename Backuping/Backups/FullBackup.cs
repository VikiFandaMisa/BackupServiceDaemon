using System.IO;

using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public class FullBackup : Backup {
        public FullBackup(string source, string target, IFileSystemAPI fileSystemAPI)
            : base(source, target, fileSystemAPI) { }
        protected override void BackupAlgorithm() {
            Progress.Report(new BackupProgress() { Percentage = 0, Status = "Started full backup" });

            string target = Utils.GetTarget("Full_", Target, Source);

            FileSystemAPI.CreateDirectory(target);
            Utils.CopyChangedFiles(Source, target, new Snapshot(target) { Name = Path.GetFileName(Source)}, FileSystemAPI);

            Progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
    }    
}