using System;

namespace BackupServiceDaemon.Models
{
    public class Computer
    {
        public int ID { get; set;}
        public string Hostname {get; set;}
        public DateTime LastSeen {get; set;}
        public string IP {get; set;}
        public string MAC {get; set;}
        public int Status {get; set;}
        public override string ToString() {
            return String.Format(
                "ID = {0}\nHostname = {1}\nLastSeen = {2}\nIP = {3}\nMAC = {4}\nStatus = {5}\n",
                this.ID,
                this.Hostname,
                this.LastSeen.ToString(),
                this.IP,
                this.MAC,
                this.Status.ToString()
            );
        }
    }
}