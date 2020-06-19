using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace BackupServiceDaemon.Backuping.FileSystemAPIs {
    public class FTPFileSystemAPI : IFileSystemAPI {
        public const char SEPARATOR = '/';
        public NetworkCredential Creds { get; set; }
        public string Server { get; set; }
        public void CreateDirectory(string directory) {
            WebRequest request = WebRequest.Create(directory);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = Creds;
        }
        public void CopyFile(string source, string target) {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Server + target);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = Creds;

            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(source)) {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream()) {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }
        }
        public string CombinePath(params string[] path) {
            string result = Server;
            foreach (var item in path) {
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
            if (!this.Server.EndsWith(SEPARATOR))
                Server += SEPARATOR;
        }
    }
}
