using System;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class IncrementalBackup : IBackup {
        public string Source { get; set; }
        public string Target { get; set; }
        public string LastBU { get; set; }
        public int Retention { get; set; }
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Incremental backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
            this.Backup();
        }
        public void Backup() {
            if (Utils.IsLimitReached(Target, Retention) || Utils.IsFirst(Target)) {
                string target = Utils.GetTarget(SettingsService.Settings.PrefixFull, Target, Source);
                Directory.CreateDirectory(target);
                Utils.CopyDirectory(Source, target);
            }
            else {
                string target = Utils.GetTarget(SettingsService.Settings.Prefix, Target, Source);
                Directory.CreateDirectory(target);
                Utils.CopyChangedFiles(Source, target, Utils.FindLast(Target, Source));
            }
        }
    }
}