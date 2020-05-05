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
            if (Utils.IsFirst(Target) || Utils.IsLimitReached(Target, Retention)) {
                Directory.CreateDirectory(Target + SettingsService.Settings.PrefixFull + DateTime.Today.ToShortDateString());
                Utils.CopyDirectory(Source, Target + SettingsService.Settings.Prefix + DateTime.Today.ToShortDateString());
            }
            else {
                Directory.CreateDirectory(Target + SettingsService.Settings.Prefix + DateTime.Today.ToShortDateString());
                this.Backup();
            }
        }
        public void Backup() {
            Utils.CopyChangedFiles(Source, Target + SettingsService.Settings.Prefix + DateTime.Today.ToShortDateString(), Path.Combine(Target, FindLastFull(Target)));
        }
        public static string FindLastFull(string Target) {
            DateTime Date = Directory.GetLastWriteTime(Path.Combine(Target, Directory.GetDirectories(Target)[0]));
            string Last = null;
            foreach (var dir in Directory.GetDirectories(Target)) {
                if (Directory.GetLastWriteTime(Path.Combine(Target, dir)) > Date && dir.Contains(SettingsService.Settings.PrefixFull))				
                    Last = dir;
            }
            return Last;
        }
    }
}