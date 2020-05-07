using System;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class FullBackup : IBackup {
        public string Source { get; set; }
        public string Target { get; set; }
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Full backup");
            this.Backup();
            progress.Report(new BackupProgress() { Percentage = 100 });
        }
        public void Backup() {
            Directory.CreateDirectory(Target + SettingsService.Settings.PrefixFull + '_' + Source + '_' + DateTime.Today.ToShortDateString());
            Utils.CopyDirectory(Source, Target + SettingsService.Settings.PrefixFull + '_' + Source + '_' + DateTime.Today.ToShortDateString());
        }
    }    
}