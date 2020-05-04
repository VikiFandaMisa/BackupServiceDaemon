using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class IncrementalBackup : IBackup
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string LastBU { get; set; }
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Incremental backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
        }
        public void Backup() {
            if (IsTimeForReset())
                Utils.CopyDirectory(Source, Target);
            else
                Utils.CopyChangedFiles(Source, Target, LastBU);
        }
        private bool IsTimeForReset() {
            return true;
        }
    }
}