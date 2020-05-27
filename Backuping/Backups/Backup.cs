using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;

using BackupServiceDaemon.Backuping.FileSystemAPIs;

namespace BackupServiceDaemon.Backuping.Backups
{
    public abstract class Backup
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public int JobID { get; set; }
        protected IFileSystemAPI FileSystemAPI { get; set; }
        protected IProgress<BackupProgress> Progress { get; set; }
        public Backup(string source, string target, int jobID, IFileSystemAPI fileSystemAPI) {
            this.Source = source;
            this.JobID = jobID;
            this.FileSystemAPI = fileSystemAPI;
            this.Target = this.GetTarget(target);
        }
        public void Run(IProgress<BackupProgress> progress) {
            Progress = progress;
            FileSystemAPI.CreateTarget(Target);
            BackupAlgorithm();
            FileSystemAPI.Dispose();
        }
        protected abstract void BackupAlgorithm();
        protected void CopyChangedFiles(string path, Snapshot snapshot) {
            foreach (var file in Directory.GetFiles(path)) {
                string abs = Path.Combine(path, file);
                string rel = FileSystemAPI.GetRelativePath(abs, this.Target);
                if (!snapshot.FileExists(rel))
                    FileSystemAPI.CopyFile(abs, FileSystemAPI.CombinePath(Target, rel));
            }

            foreach (var dir in Directory.GetDirectories(path)) {
                string abs = Path.Combine(path, dir);
                string rel = FileSystemAPI.GetRelativePath(abs, this.Target);
                if (!snapshot.DirExist(rel))
                    FileSystemAPI.CreateDirectory(FileSystemAPI.CombinePath(Target, rel));
                CopyChangedFiles(abs, snapshot);
            }
        }
        protected string GetLastBackup() {
            List<string> lastTargets = new List<string>();
            foreach (var dir in Directory.GetDirectories(Target))
                if (dir.Contains(FileSystemAPI.GetFileName(Source) + '_'))
                    lastTargets.Add(dir);

            if (lastTargets.Count == 0)
                return null;

            lastTargets.Sort();

            return Path.Combine(Target, lastTargets.Last());
        }
        protected string GetTarget(string targetDirectory) {
            string date = DateTime.Now.ToString()
                .Replace(':', '-')
                .Replace(' ', '_');
            return FileSystemAPI.CombinePath(targetDirectory, String.Join('_', JobID, FileSystemAPI.GetFileName(Source), date));
        }
        protected Snapshot LoadSnapshot() {
            return JsonSerializer.Deserialize<Snapshot>(File.ReadAllText(Path.Combine(this.Source, ".BackupService", JobID + ".json")));
        }
        public async void SaveSnapshot(Snapshot snapshot) {
            string dir = FileSystemAPI.CombinePath(Source, ".BackupService");
            Directory.CreateDirectory(dir);
            using (FileStream fs = File.Create(FileSystemAPI.CombinePath(dir, JobID + ".json")))
                await JsonSerializer.SerializeAsync(fs, snapshot);
        }
        protected async void CreateSnapshot() {
            Snapshot snapshot = new Snapshot(Source);
            string dir = Path.Combine(Source, ".BackupService");
            Directory.CreateDirectory(dir);
            using (FileStream fs = File.Create(Path.Combine(dir, JobID + ".json")))
            {
                await JsonSerializer.SerializeAsync(fs, snapshot);
            }
        }
    }
}