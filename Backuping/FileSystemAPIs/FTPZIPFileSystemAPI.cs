using System.IO;
using System.Net;

namespace BackupServiceDaemon.Backuping.FileSystemAPIs {
    public class FTPZIPFileSystemAPI : IFileSystemAPI {
        public const char SEPARATOR = '/';
        public NetworkCredential Creds { get; set; }
        public string Server { get; set; }
        public void CreateDirectory(string directory) {
            throw new System.NotImplementedException();
        }
        public void CopyFile(string source, string target) {
            throw new System.NotImplementedException();
        }
        public string CombinePath(params string[] path) {
            throw new System.NotImplementedException();
        }
        public string GetFileName(string path) {
            throw new System.NotImplementedException();
        }
        public string ConvertSeparators(string path) {
            throw new System.NotImplementedException();
        }
        public void CreateTarget(string target) {
            throw new System.NotImplementedException();
        }
    }
}
