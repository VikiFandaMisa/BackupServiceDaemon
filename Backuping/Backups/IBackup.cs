using System;

namespace BackupServiceDaemon.Backuping.Backups
{
    public interface IBackup
    {
        void Run(IProgress<BackupProgress> progress);
    }
}