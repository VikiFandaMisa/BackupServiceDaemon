using System;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class FullBackup : IBackup {
        private string _Source { get; set; }
        public string Source { get => this._Source; set => this._Source = Utils.ConvertSeparators(value); }
        private string _Target { get; set; }
        public string Target { get => this._Target; set => this._Target = Utils.ConvertSeparators(value); }
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Full backup");
            this.Backup();
            progress.Report(new BackupProgress() { Percentage = 100 });
        }
        public void Backup() {
            string target = Utils.GetTarget(SettingsService.Settings.PrefixFull, Target, Source);
            Directory.CreateDirectory(target);
            
            Utils.CopyChangedFiles(Source, target, new Snapshot(target));
        }
    }    
}