using System;
using System.IO;


namespace BackupServiceDaemon.Backuping
{
    public static class Utils {
        public static string LocalRelativePath(string path, string basePath) {
            return UriRelativePath(path, basePath).Replace('/', Path.DirectorySeparatorChar);
        }
        public static string UriRelativePath(string path, string basePath) {
            Uri pathUri = new Uri(path);
            if (!basePath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                basePath += Path.DirectorySeparatorChar;
            }
            Uri basePathUri = new Uri(basePath);
            return Uri.UnescapeDataString(basePathUri.MakeRelativeUri(pathUri).ToString());
        }
    }
}