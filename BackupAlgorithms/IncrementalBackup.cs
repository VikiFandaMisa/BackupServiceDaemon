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
            System.Console.WriteLine("Incremental backup");
            this.Backup();
            progress.Report(new BackupProgress() { Percentage = 100 });
        }
        public void Backup() {
            string target = Utils.GetTarget(SettingsService.Settings.PrefixFull, Target, Source);
            Directory.CreateDirectory(target);

            string last = Utils.GetLastBackup(Target, Path.GetFileName(Source));

            Snapshot snapshot;
            if (last == null)
                snapshot = new Snapshot(target);
            else
                snapshot = Utils.LoadSnapshot(last);


            Utils.CopyChangedFiles(Source, target, snapshot);

            Directory.CreateDirectory(Path.Combine(target, ".BackupService"));
            Utils.CreateSnapshot(target);
        }
    }
}