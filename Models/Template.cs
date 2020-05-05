using System;

namespace BackupServiceDaemon.Models
{
     public enum BackupType {
        full = 1,
        differential = 2,
        incremental = 3
    }
    public enum BackupFileType {
        plain = 1,
        zip = 2
    }
    public class Template {
        public int ID {get; set;}
        public string Name {get; set;}
        public string Period {get; set;}
        public BackupType Type {get; set;}
        public BackupFileType TargetFileType {get; set;}
        public DateTime Start {get; set;}
        public DateTime End {get; set;}
        public bool Paused {get; set;}
        public int Retention {get; set;}
    }
}