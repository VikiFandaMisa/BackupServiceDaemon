using System;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class IncrementalBackup : IBackup
    {
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Incremental backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
        }
    }
}