
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace BackupServiceDaemon.Backuping.FileSystemAPIs
{
    public class FTPFileSystemAPI : IFileSystemAPI {
        public const char SEPARATOR = '/';
        public NetworkCredential creds { get; set; }
        public string server { get; set; }
        public string[] GetFiles(string directory) {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(server + directory);
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            request.Credentials = creds;
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();            
            var list = new List<string>();
            using (StreamReader reader = new StreamReader(responseStream)) {
                string file;
                while ((file = reader.ReadLine()) != null) {
                    list.Add(file);
                }
            }
            return list.ToArray();
        }
        public string[] GetDirectories(string directory) {
            //nejde
            throw new System.NotImplementedException();
        }
        public void CreateDirectory(string directory) {
            WebRequest request = WebRequest.Create(directory);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = creds;
        }
        public void CopyFile(string source, string target) {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(server + target);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = creds;

            byte[] fileContents;
            using (StreamReader sourceStream = new StreamReader(source))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }
        }
        public string CombinePath(params string[] path) {
            string result = server;
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
        public string GetRelativePath(string path, string basePath) {
            return Utils.UriRelativePath(path, basePath);
        }
        public string ConvertSeparators(string path) {
            return path;
        }
        public void CreateTarget(string target) {
            if (!this.server.EndsWith(SEPARATOR))
                server += SEPARATOR;
        }
    }
}