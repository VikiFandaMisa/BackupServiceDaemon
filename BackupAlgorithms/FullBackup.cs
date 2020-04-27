using System;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public class FullBackup : IBackup
    {
        public void Run(IProgress<BackupProgress> progress) {
            System.Console.WriteLine("Full backup");
            progress.Report(new BackupProgress() { Percentage = 100 });
        }
    }
}