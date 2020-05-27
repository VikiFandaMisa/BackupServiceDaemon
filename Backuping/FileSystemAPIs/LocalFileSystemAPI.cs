using System.IO;

namespace BackupServiceDaemon.Backuping.FileSystemAPIs
{
    public class LocalFileSystemAPI : IFileSystemAPI {
        public string[] GetFiles(string directory) {
            return Directory.GetFiles(directory);
        }
        public string[] GetDirectories(string directory) {
            return Directory.GetDirectories(directory);
        }
        public void CreateDirectory(string directory) {
            Directory.CreateDirectory(directory);
        }
        public void CopyFile(string source, string target) {
            File.Copy(source, target);
        }
        public string CombinePath(params string[] path) {
            return Path.Combine(path);
        }
        public string GetFileName(string path) {
            return Path.GetFileName(path);
        }
    }
}