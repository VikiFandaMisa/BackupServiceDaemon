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
        public int TemplateID {get; set;}
        public string TemplateName {get; set;}
        public int ID {get; set;}
        public BackupType Type {get; set;}
        public BackupFileType TargetFileType {get; set;}
        public int Retention {get; set;}
        public DateTime[] Schedule {get; set;}
        public Path[] Sources {get; set;}
        public Path[] Targets {get; set;}
    }
}