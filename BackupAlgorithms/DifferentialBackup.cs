using System;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class DifferentialBackup : IBackup {
        public string Source { get; set; }
        public string Target { get; set; }
        public string LastFull { get; set; }
        public int Retention { get; set; }
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Differential backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
            this.Backup();
        }
        public void Backup() {
            if (Utils.IsFirst(Target) || Utils.IsLimitReached(Target, Retention)) {
                string target = Utils.GetTarget(SettingsService.Settings.PrefixFull, Target, Source);
                Directory.CreateDirectory(target);
                Utils.CopyDirectory(Source, target);
            }
            else {
                string target = Utils.GetTarget(SettingsService.Settings.Prefix, Target, Source);
                Directory.CreateDirectory(target);
                Utils.CopyChangedFiles(Source, target, Utils.FindLastFull(Target, Path.GetFileName(Source)));
            }
        }
    }
}