using System;
using System.Linq;
using System.Net.NetworkInformation;


namespace BackupServiceDaemon {
    public static class ComputerInfo {
        public static string GetHostname() {
            return Environment.MachineName;
        }

        private static NetworkInterface GetFirstWorkingNIC() {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()) {
                if (nic.OperationalStatus == OperationalStatus.Up)
                    return nic;
            }

            throw new Exception("No NIC found");
        }

        public static string GetIP() {
            return GetFirstWorkingNIC().GetIPProperties().UnicastAddresses[0].Address.ToString();
        }

        public static string GetMAC() {
            NetworkInterface nic = GetFirstWorkingNIC();
            return string.Join(":", (from b in nic.GetPhysicalAddress().GetAddressBytes()
                                     select b.ToString("X2")).ToArray());
        }
    }
}
