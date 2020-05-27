using System;
using System.IO;
using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public class FullBackup : IBackup {
        private string _Source { get; set; }
        public string Source { get => this._Source; set => this._Source = Utils.ConvertSeparators(value); }
        private string _Target { get; set; }
        public string Target { get => this._Target; set => this._Target = Utils.ConvertSeparators(value); }
        public IFileSystemAPI FileSystemAPI { get; set; }
        public void Run(IProgress<BackupProgress> progress) {
            progress.Report(new BackupProgress() { Percentage = 0, Status = "Started full backup" });
            this.Backup();
            progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
        public void Backup() {
            string target = Utils.GetTarget("Full_", Target, Source);

            FileSystemAPI.CreateDirectory(target);
            Utils.CopyChangedFiles(Source, target, new Snapshot(target) { Name = Path.GetFileName(Source)}, FileSystemAPI);
        }
    }    
}