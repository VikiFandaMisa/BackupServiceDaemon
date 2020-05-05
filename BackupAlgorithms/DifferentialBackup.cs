using System;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class DifferentialBackup : IBackup
    {
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Differential backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
        }
    }
}