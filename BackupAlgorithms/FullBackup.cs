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
            string target = Utils.GetTarget(SettingsService.Settings.PrefixFull, Target, Source);
            System.Console.WriteLine(target);
            Directory.CreateDirectory(target);
            Utils.CopyDirectory(Source, target);
        }
    }    
}