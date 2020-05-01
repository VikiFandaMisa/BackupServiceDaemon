using System;
using System.Collections.Generic;
using System.IO;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class FullBackup : IBackup
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Full backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
        }
        public void Backup() {
            Utils.CopyDirectory(Source, Target);
        }
        
    }
    
}