using System;

namespace BackupServiceDaemon.Models
{
     public enum BackupType {
        Full = 1,
        Differential = 2,
        Incremental = 3
    }
    public enum BackupFileType {
        Plain = 1,
        Zip = 2
    }
    public class Job {
        public int ID {get; set;}
        public string Name {get; set;}
        public string Period {get; set;}
        public BackupType Type {get; set;}
        public BackupFileType TargetFileType {get; set;}
        public DateTime Start {get; set;}
        public DateTime End {get; set;}
        public bool Paused {get; set;}
        public int Retention {get; set;}
        public Path[] Sources {get; set;}
        public Path[] Targets {get; set;}
    }
}