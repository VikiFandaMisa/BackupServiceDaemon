using System;

namespace BackupServiceDaemon.Models
{
    public class LogItem {
        public int ID {get; set;}
        public int JobID {get; set;}
        public MessageType Type {get; set;}
        public DateTime Date {get; set;}
        public string Message {get; set;}
    }
}