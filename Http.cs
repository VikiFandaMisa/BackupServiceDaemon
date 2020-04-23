using BackupServiceDaemon.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BackupServiceDaemon
{
    public static class Http
    {
        //VSUDE SEND AUTHORIZATION: Bearer *token*
        public static string URL { get; set; } = "http://localhost:5001/api";
        public static Computer GetComputer()
        {
            using (var computer = new HttpClient())
            {
                computer.BaseAddress = new Uri(URL);

                var responseTask = computer.GetAsync("computers/self");
                responseTask.Wait();
               
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    var readTask = responseTask.Result.Content.ReadAsAsync<Computer>();
                    readTask.Wait();

                    return readTask.Result;
                }
                throw new Exception("Computer not found");
            }
        }

		public static void PostComputer()
        {
            using (var computer = new HttpClient())
            {
                computer.BaseAddress = new Uri(URL);

                var computerRegistration = new ComputerRegistration() { Hostname = PCInfo.GetHostName(), MAC = PCInfo.GetMAC(), IP = PCInfo.GetIP() };

                var Task = computer.PostAsJsonAsync<ComputerRegistration>("computers/registration", computerRegistration);
                Task.Wait();

                if (Task.Result.IsSuccessStatusCode)
                {
                    //CATCH COMPUTER ID
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