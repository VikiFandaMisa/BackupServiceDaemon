using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace BackupServiceDaemon.Backuping
{
    public class Snapshot {
        public string Name { get; set; }
        public List<string> Files { get; set; }
        public List<Snapshot> Directories { get; set; }
        public Snapshot() { }

        public Snapshot(string path) {
            this.Name = Path.GetFileName(path);
            this.Files = new List<string>();
            this.Directories = new List<Snapshot>();

            LoadStructure(path);
        }
        
        private void LoadStructure(string path) {
            foreach (var directory in Directory.GetDirectories(path))
                if (directory != ".BackupService")
                    this.Directories.Add(new Snapshot(Path.Combine(path, directory)));

            foreach (var file in Directory.GetFiles(path))
                this.Files.Add(file);
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