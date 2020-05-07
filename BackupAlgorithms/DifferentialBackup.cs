using System;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class DifferentialBackup : IBackup {
        public string Source { get; set; }
        public string Target { get; set; }
        public string LastFull { get; set; }
        public int Retention { get; set; } = 2;
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Differential backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
            this.Backup();
        }
        public void Backup() {
            if (Utils.IsFirst(Target) || Utils.IsLimitReached(Target, Retention)) {
                Directory.CreateDirectory(Target + SettingsService.Settings.PrefixFull  + '_' + Source + '_' + DateTime.Today.ToShortDateString());
                Utils.CopyDirectory(Source, Target + SettingsService.Settings.PrefixFull  + '_' + Source + '_' + DateTime.Today.ToShortDateString());
            }
            else {
                Directory.CreateDirectory(Target + SettingsService.Settings.Prefix  + '_' + Source + '_' + DateTime.Today.ToShortDateString());
                Utils.CopyChangedFiles(Source, Target + SettingsService.Settings.Prefix + '_' + Source + '_' + DateTime.Today.ToShortDateString(), Path.Combine(Target, Utils.FindLastFull(Target, Source)));
            }
        }
    }
}