using System;

using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public abstract class Backup
    {
        public string Source { get; set; }
        public string Target { get; set; }
        protected IFileSystemAPI FileSystemAPI { get; set; }
        protected IProgress<BackupProgress> Progress { get; set; }
        public Backup(string source, string target, IFileSystemAPI fileSystemAPI) {
            this.Source = source;
            this.Target = target;
            this.FileSystemAPI = fileSystemAPI;
        }
        public void Run(IProgress<BackupProgress> progress) {
            Progress = progress;
            BackupAlgorithm();
            FileSystemAPI.Dispose();
        }
        protected abstract void BackupAlgorithm();
    }
}