using System;

namespace BackupServiceDaemon.Models
{
    public class ComputerRegistration
    {
        public string Hostname { get; set; }
        public string Password { get; set; }
        public string MAC { get; set; }
        public string IP { get; set; }
    }
}