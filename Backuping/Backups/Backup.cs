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
        protected string ConfigName { get { return JobID + ".json"; } }
        protected string ConfigDirectory { get { return SettingsService.Settings.ConfigurationFolderName; } }
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
        protected (Snapshot, Snapshot) CopyChangedFiles(Snapshot compareTo) { // ( added, deleted )
            var added = new Snapshot(compareTo.Name);
            var deleted = new Snapshot(compareTo.Name);

            CopyChangedFiles(Source, compareTo, added, deleted);

            return (added, deleted);
        }
        private void CopyChangedFiles(string path, Snapshot compareTo, Snapshot added, Snapshot deleted) {
            foreach (var file in Directory.GetFiles(path)) {
                string absolute = Path.Combine(path, file);
                string relative = Utils.UriRelativePath(absolute, Source);
                if (!compareTo.FileExists(relative)) {
                    added.AddFile(relative);
                    FileSystemAPI.CopyFile(absolute, FileSystemAPI.CombinePath(Target, FileSystemAPI.ConvertSeparators(relative)));
                }
            }

            foreach (var dir in Directory.GetDirectories(path)) {
                string absolute = Path.Combine(path, dir);
                string relative = Utils.UriRelativePath(absolute, Source);
                if (!compareTo.DirExist(relative)) {
                    added.AddDirectory(relative);
                    FileSystemAPI.CreateDirectory(FileSystemAPI.CombinePath(Target, FileSystemAPI.ConvertSeparators(relative)));
                } 
                CopyChangedFiles(path, compareTo, added, deleted);
            }
        }
        protected string GetTarget(string targetDirectory) {
            string date = DateTime.Now.ToString()
                .Replace(':', '-')
                .Replace(' ', '_');
            return FileSystemAPI.CombinePath(targetDirectory, String.Join('_', JobID, FileSystemAPI.GetFileName(Source), date));
        }
        protected Snapshot LoadSnapshot() {
            string path = Path.Combine(Source, ConfigDirectory, ConfigName);
            if (!File.Exists(path))
                return null;
            return JsonSerializer.Deserialize<Snapshot>(File.ReadAllText(path));
        }
        public async void SaveSnapshot(Snapshot snapshot) { 
            string confDir = Path.Combine(Source, ConfigDirectory);
            Directory.CreateDirectory(confDir);
            using (FileStream fs = File.Create(Path.Combine(confDir, ConfigName)))
                await JsonSerializer.SerializeAsync(fs, snapshot);
        }
    }
}