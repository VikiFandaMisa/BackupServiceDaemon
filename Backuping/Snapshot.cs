using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace BackupServiceDaemon.Backuping {
    public class Snapshot {
        public string Name { get; set; }
        public List<string> Files { get; set; }
        public List<Snapshot> Directories { get; set; }
        public Snapshot() { }
        public Snapshot(string name) {
            this.Name = name;
            this.Files = new List<string>();
            this.Directories = new List<Snapshot>();
        }
        public void AddFile(string relativePath) {
            var pathFilename = ParseFilename(relativePath);

            Snapshot directory = AddDirectory(pathFilename.Item1);
            directory.Files.Add(pathFilename.Item2);
        }
        public Snapshot AddDirectory(string relativePath) {
            var path = ParsePath(relativePath);
            return AddDirectory(path);
        }
        public Snapshot AddDirectory(List<string> relativePath) {
            Snapshot directory = this;
            for (int i = 0; i < relativePath.Count; i++) {
                var old = directory;
                directory = directory.Directories.SingleOrDefault(dir => dir.Name == relativePath[i]);
                if (directory != null) {
                    var toAdd = new Snapshot(relativePath[i]);
                    directory.Directories.Add(toAdd);
                    directory = toAdd;
                }
            }

            return directory;
        }
        public bool DirExist(string relativePath) {
            return GetDirectory(relativePath) != null;
        }
        private List<string> ParsePath(string relativePath) {
            List<string> path = relativePath.Split(Path.DirectorySeparatorChar).ToList();
            path.RemoveAll(s => s == "");
            return path;
        }
        private (List<string>, string) ParseFilename(string relativePath) {
            var path = ParsePath(relativePath);
            string filename = path.Last();
            path.RemoveAt(path.Count - 1);
            return (path, filename);
        }
        public Snapshot GetDirectory(string relativePath) {
            var path = ParsePath(relativePath);
            return GetDirectory(path);
        }
        public Snapshot GetDirectory(List<string> relativePath) {
            Snapshot directory = this;
            for (int i = 0; i < relativePath.Count; i++) {
                var directoryEnumerable = directory.Directories.Where(dir => dir.Name == relativePath[i]);
                if (directoryEnumerable.Count() != 0)
                    directory = directory.Directories.Where(dir => dir.Name == relativePath[i]).First();
                else
                    return null;
            }

            return directory;
        }
        public bool FileExists(string relativePath) {
            var pathFilename = ParseFilename(relativePath);

            Snapshot directory = GetDirectory(pathFilename.Item1);

            if (directory == null)
                return false;

            return directory.Files.Contains(pathFilename.Item2);
        }
        public void Union(Snapshot snapshot) {
            foreach (string file in snapshot.Files)
                this.Files.Add(file);

            foreach (Snapshot directory in snapshot.Directories)
                this.AddDirectory(directory.Name).Union(directory);
        }
        public void Subtract(Snapshot snapshot) {
            foreach (string file in snapshot.Files)
                this.Files.Remove(file);

            foreach (Snapshot directory in snapshot.Directories)
                if (directory.Files.Count == 0)
                    this.Directories.RemoveAll(s => s.Name == directory.Name);
                else
                    this.Directories.Single(s => s.Name == directory.Name).Subtract(directory);
        }
    }
}
