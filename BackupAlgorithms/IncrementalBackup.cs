using System;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class IncrementalBackup : IBackup {
        public string Source { get; set; }
        public string Target { get; set; }
        public string LastBU { get; set; }
        public int Retention { get; set; } = 2;
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Incremental backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
        }
        public void Backup() {
            if (Utils.IsLimitReached(Target, Retention) || Utils.IsFirst(Target)) {
                Directory.CreateDirectory(Target + SettingsService.Settings.PrefixFull + DateTime.Today.ToShortDateString());
                Utils.CopyDirectory(Source, Target + SettingsService.Settings.PrefixFull + DateTime.Today.ToShortDateString());
            }
            else {
                Directory.CreateDirectory(Target + SettingsService.Settings.Prefix + DateTime.Today.ToShortDateString());
                Utils.CopyChangedFiles(Source, Target + SettingsService.Settings.Prefix + DateTime.Today.ToShortDateString(), Path.Combine(Target, FindLast(Target)));
            }
        }
        public static string FindLast(string Target) {
            DateTime Date = Directory.GetLastWriteTime(Path.Combine(Target, Directory.GetDirectories(Target)[0]));
            string Last = null;
            foreach (var dir in Directory.GetDirectories(Target)) {
                if (Directory.GetLastWriteTime(Path.Combine(Target, dir)) > Date)				
                    Last = dir;
            }
            return Last;
        }
    }
}