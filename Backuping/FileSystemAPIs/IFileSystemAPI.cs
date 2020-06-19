using System;

namespace BackupServiceDaemon.Backuping.FileSystemAPIs {
    public interface IFileSystemAPI : IDisposable {
        void CreateDirectory(string directory);
        void CopyFile(string source, string target);
        string CombinePath(params string[] path);
        string GetFileName(string path);
        string ConvertSeparators(string path);
        void CreateTarget(string target);
        void IDisposable.Dispose() { }
    }
}
