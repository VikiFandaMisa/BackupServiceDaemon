using System.IO;
using Ionic.Zip;

namespace BackupServiceDaemon.Backuping.FileSystemAPIs
{
    public class LocalZIPFileSystemAPI : IFileSystemAPI {
        public ZipFile Zip { get; set; }
        public string Target { get; set; }
        public const char SEPARATOR = '/';
        public void CreateDirectory(string directory) {            
            Zip.AddDirectory(directory);            
        }
        public void CopyFile(string source, string target) {
            string t = target.Replace(Target,"");
            Zip.AddFile(source, t);
        }
        public string CombinePath(params string[] path) {
            string result = "";
            foreach (var item in path)
            {
                if (!item.EndsWith(SEPARATOR))
                    result += SEPARATOR + item;
            }
            return result + SEPARATOR;
        }
        public string GetFileName(string path) {
            string name = path;
            if (name.EndsWith(SEPARATOR))
                name = name.Substring(0, name.Length - 1);
            name = name.Substring(name.LastIndexOf(SEPARATOR));
            return name;
        }
        public string ConvertSeparators(string path) {
            return path;
        }
        public void CreateTarget(string target) {
            this.Target = target;
        }
        public void Dispose() {
            Zip.Save(Target + ".zip");
        }
    }
}