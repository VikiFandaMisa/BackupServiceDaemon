using System;

namespace BackupServiceDaemon.Tables
{
    public class Computers
    {
        public int ID { get; set;}
        public string Hostname {get; set;}
        public string Password {get; set;}
        public DateTime LastSeen {get; set;}
        public string IP {get; set;}
        public string MAC {get; set;}
        public int Status {get; set;}
    }
}