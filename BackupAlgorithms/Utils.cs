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
                    Directory.CreateDirectory(abs);
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
            string[] pathArr = path.Split(Path.DirectorySeparatorChar);
            List<string> newPath = new List<string>();

            bool save = false;
            for (int i = 0; i < pathArr.Length; i++) {
                if (save)
                    newPath.Add(pathArr[i]);
                if (pathArr[i].Contains(sourceName + '_'))
                    save = true;
            }

            return Path.Combine(newPath.ToArray());
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
        public Snapshot(string path) {
            this.Name = Path.GetFileName(path);
            this.Files = new List<string>();
            this.Directories = new List<Snapshot>();

            foreach (var directory in Directory.GetDirectories(path))
                if (directory != ".BackupService")
                    this.Directories.Add(new Snapshot(Path.Combine(path, directory)));

            foreach (var file in Directory.GetFiles(path))
                this.Files.Add(Path.Combine(file));
        }

        public bool DirExist(string realtivePath) {
            return GetDirectory(realtivePath) != null;
        }

        public Snapshot GetDirectory(string realtivePath) {
            List<string> path = realtivePath.Split(Path.DirectorySeparatorChar).ToList();
            path.RemoveAll(s => s == "");

            return GetDirectory(path);
        }

        public Snapshot GetDirectory(List<string> realtivePath) {
            Snapshot directory = this;
            for(int i = 0; i < realtivePath.Count; i++) {
                directory = directory.Directories.Where(dir => dir.Name == realtivePath[i]).First();
                if (directory == null)
                    return null;
            }

            return directory;
        }

        public bool FileExists(string realtivePath) {
            List<string> path = realtivePath.Split(Path.DirectorySeparatorChar).ToList();
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