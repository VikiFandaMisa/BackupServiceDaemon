using System.Text.Json;

namespace BackupServiceDaemon.Models {
    public enum FTPMode {
        Active = 1,
        Passive = 2
    }

    public enum FTPEncryptionMode {
        None = 1,
        Explicit = 2,
        Implicit = 3
    }

    public class NetworkSettings {
        public string Server { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public FTPMode Mode { get; set; }
        public FTPEncryptionMode Encryption { get; set; }
    }
}
