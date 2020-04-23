using BackupServiceDaemon.Tables;
using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BackupServiceDaemon
{
    public static class Http
    {
        public static string URL { get; set; } = "http://localhost:5001";
        public static Computers[] GetComputers()
        {
            using (var computers = new HttpClient())
            {
                computers.BaseAddress = new Uri(URL);

                var responseTask = computers.GetAsync("computers");
                responseTask.Wait();
               
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    var readTask = responseTask.Result.Content.ReadAsAsync<Computers[]>();
                    readTask.Wait();

                    return readTask.Result;
                }
                throw new Exception("Computer not found");
            }
        }

		public static void PostComputers()
        {
            using (var computer = new HttpClient())
            {
                computer.BaseAddress = new Uri(URL);
                

                var Computer = new ComputerRegistration() { Hostname = PCInfo.GetHostName(), MAC = PCInfo.GetMAC(), IP = PCInfo.GetIP() };

                var Task = computer.PostAsJsonAsync<ComputerRegistration>("client", Computer);
                Task.Wait();

                if (Task.Result.IsSuccessStatusCode)
                {

                    var readTask = Task.Result.Content.ReadAsAsync<ComputerRegistration>();
                    readTask.Wait();

                    Console.WriteLine("PC info sent");
                }
                else
                {
                    Console.WriteLine(Task.Result.StatusCode);
                }
            }
        }		
    }
}