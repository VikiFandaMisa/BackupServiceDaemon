using System.IO;

namespace BackupServiceDaemon.Backuping.FileSystemAPIs
{
    public class LocalFileSystemAPI : IFileSystemAPI {
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
        public string GetRelativePath(string path, string basePath) {
            return Utils.LocalRelativePath(path, basePath);
        }
        public string ConvertSeparators(string path) {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }
        public void CreateTarget(string target) {
            CreateDirectory(target);
        }
    }
}