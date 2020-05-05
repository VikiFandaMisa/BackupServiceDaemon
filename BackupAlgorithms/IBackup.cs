using System;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public interface IBackup
    {
        void Run(IProgress<BackupProgress> progress);
    }
}