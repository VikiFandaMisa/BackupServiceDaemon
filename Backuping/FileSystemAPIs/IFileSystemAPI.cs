namespace BackupServiceDaemon.Backuping.FileSystemAPIs
{
    public interface IFileSystemAPI
    {
        string[] GetFiles(string directory);
        string[] GetDirectories(string directory);
        void CreateDirectory(string directory);
        void CopyFile(string source, string target);
        string CombinePath(params string[] path);
        string GetFileName(string path);
    }
}