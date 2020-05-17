using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace BackupServiceDaemon.BackupAlgorithms
{
    public static class Utils {
        public static void CopyChangedFiles(string path, string target, Snapshot snapshot) {
            foreach (var file in Directory.GetFiles(path)) {
                string abs = Path.Combine(path, file);
                string rel = GetRelativePath(abs, snapshot.Name);
                if(!snapshot.FileExists(rel))
                    File.Copy(abs, Path.Combine(target, rel));
            }

            foreach (var dir in Directory.GetDirectories(path)) {
                string abs = Path.Combine(path, dir);
                string rel = GetRelativePath(abs, snapshot.Name);
                if(!snapshot.DirExist(rel))
                    Directory.CreateDirectory(Path.Combine(target, rel));
                CopyChangedFiles(abs, target, snapshot);
            }
        }
        public static string GetLastBackup(string target, string sourceName) {
            List<string> lastTargets = new List<string>();
            foreach (var dir in Directory.GetDirectories(target))
                if (dir.Contains(sourceName + '_'))
                    lastTargets.Add(dir);

            if (lastTargets.Count == 0)
                return null;

            lastTargets.Sort();

            return Path.Combine(target, lastTargets.Last());
        }
        public static string GetRelativePath(string path, string sourceName) {
             // FIX TWO DIRECTORIES WITH THE SAME NAME
            return path.Remove(0, path.IndexOf(sourceName)).Replace(sourceName + Path.DirectorySeparatorChar, "");
        }
        public static string GetSuffix() {
            return '_' + DateTime.Now.ToString().Replace(':', '-').Replace(' ', '_');
        }
        public static string GetTarget(string prefix, string target, string sourceName) {
            return Path.Combine(target, prefix + Path.GetFileName(sourceName) + GetSuffix());
        }
        public static string ConvertSeparators(string path) {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }
        public static Snapshot LoadSnapshot(string target) {
            return JsonSerializer.Deserialize<Snapshot>(File.ReadAllText(Path.Combine(target, ".BackupService", "snapshot.json")));
        }
        public static async void CreateSnapshot(string target) {
            Snapshot snapshot = new Snapshot(target);
            string dir = Path.Combine(target, ".BackupService");
            Directory.CreateDirectory(dir);
            using (FileStream fs = File.Create(Path.Combine(dir, "snapshot.json")))
            {
                await JsonSerializer.SerializeAsync(fs, snapshot);
            }
        }
    }

    public class Snapshot {
        public string Name { get; set; }
        public List<string> Files { get; set; }
        public List<Snapshot> Directories { get; set; }
        public Snapshot() { }

        public Snapshot(string path, string relativeTo = null) {
            this.Name = Path.GetFileName(path);
            this.Files = new List<string>();
            this.Directories = new List<Snapshot>();

            LoadStructure(path, relativeTo == null ? this.Name : relativeTo);
        }
        
        private void LoadStructure(string path, string relativeTo) {
            foreach (var directory in Directory.GetDirectories(path))
                if (directory != ".BackupService")
                    this.Directories.Add(new Snapshot(Path.Combine(path, directory), this.Name));

            foreach (var file in Directory.GetFiles(path))
                this.Files.Add(Utils.GetRelativePath(file, relativeTo));
        }

        public async void Save(string target) {
            string dir = Path.Combine(target, ".BackupService");
            Directory.CreateDirectory(dir);
            using (FileStream fs = File.Create(Path.Combine(dir, "snapshot.json")))
                await JsonSerializer.SerializeAsync(fs, this);
        }

        public bool DirExist(string relativePath) {
            return GetDirectory(relativePath) != null;
        }

        public Snapshot GetDirectory(string relativePath) {
            List<string> path = relativePath.Split(Path.DirectorySeparatorChar).ToList();
            path.RemoveAll(s => s == "");

            return GetDirectory(path);
        }

        public Snapshot GetDirectory(List<string> relativePath) {
            Snapshot directory = this;
            for(int i = 0; i < relativePath.Count; i++) {
                var directoryEnumerable = directory.Directories.Where(dir => dir.Name == relativePath[i]);
                if (directoryEnumerable.Count() != 0)
                    directory = directory.Directories.Where(dir => dir.Name == relativePath[i]).First();
                else
                    return null;
            }

            return directory;
        }

        public bool FileExists(string relativePath) {
            List<string> path = relativePath.Split(Path.DirectorySeparatorChar).ToList();
            path.RemoveAll(s => s == "");
            string filename = path.Last();
            path.RemoveAt(path.Count - 1);

            Snapshot directory = GetDirectory(path);
            
            if (directory == null)
                return false;

            return directory.Files.Contains(filename);
        }
    }
}