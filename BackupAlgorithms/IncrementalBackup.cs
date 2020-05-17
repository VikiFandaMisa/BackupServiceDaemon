using System;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class IncrementalBackup : IBackup {
        private string _Source { get; set; }
        public string Source { get => this._Source; set => this._Source = Utils.ConvertSeparators(value); }
        private string _Target { get; set; }
        public string Target { get => this._Target; set => this._Target = Utils.ConvertSeparators(value); }
        public int Retention { get; set; }
        public void Run(IProgress<BackupProgress> progress) {
            progress.Report(new BackupProgress() { Percentage = 0, Status = "Started incremental backup" });
            this.Backup();
            progress.Report(new BackupProgress() { Percentage = 100, Status = "Done" });
        }
        public void Backup() {
            string target = Utils.GetTarget("Inc_", Target, Source);
            string last = Utils.GetLastBackup(Target, Path.GetFileName(Source));
            Directory.CreateDirectory(target);

            Snapshot snapshot;
            if (last == null)
                snapshot = new Snapshot(target) { Name = Path.GetFileName(Source)};
            else
                snapshot = Utils.LoadSnapshot(last);

            Utils.CopyChangedFiles(Source, target, snapshot);

            // BROKEN
            (new Snapshot(target) { Name = snapshot.Name }).Save(target);
        }
    }
}