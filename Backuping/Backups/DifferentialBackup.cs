using System;
using System.IO;

using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public class DifferentialBackup : IBackup {
        private string _Source { get; set; }
        public string Source { get => this._Source; set => this._Source = Utils.ConvertSeparators(value); }
        private string _Target { get; set; }
        public string Target { get => this._Target; set => this._Target = Utils.ConvertSeparators(value); }
        public int Retention { get; set; }
        public IFileSystemAPI FileSystemAPI { get; set; }
        public void Run(IProgress<BackupProgress> progress) {
            progress.Report(new BackupProgress() { Percentage = 0, Status = "Started differential backup" });
            this.Backup();
            progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
        public void Backup() {
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
        }
    }
}