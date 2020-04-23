using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BackupServiceDaemon
{
    public static class PCInfo
    {
        public static string GetHostName() {
            return Environment.MachineName;
        }

        public static string GetIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
                
            }
            return "NO ADDRESS FOUND";
        }

        public static string GetMAC()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                    return string.Join(":", (from b in nic.GetPhysicalAddress().GetAddressBytes()
                                            select b.ToString("X2")).ToArray());
            }

            return "NO ADDRESS FOUND";
        }
    }
}