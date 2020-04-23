using BackupServiceDaemon.Models;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using static System.Environment;
using System.Security.Cryptography;
using System.Net.Http.Headers;


namespace BackupServiceDaemon
{
    public static class Http
    {
        //VSUDE SEND AUTHORIZATION: Bearer *token*
        public static string URL { get; set; } = "http://localhost:5001/api";
		public static string Token { get; set; }
		
        public static string ApplicationData { get; set; }
        public static Computer GetComputer() {
            using (var computer = new HttpClient())
            {
                computer.BaseAddress = new Uri(URL);
				computer.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);

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
		public static void SetToken() {
			using (var tokenRequest = new HttpClient())
            {
			tokenRequest.BaseAddress = new Uri(URL);

                var responseTask = tokenRequest.GetAsync("token/computer");
                responseTask.Wait();
               
                if (responseTask.Result.IsSuccessStatusCode)
                {
                    var readTask = responseTask.Result.Content.ReadAsAsync<TokenRequest>();
                    readTask.Wait();

                    Token = responseTask.Result.Content.ToString();
                }
                throw new Exception("Error");
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
					
					//var httpResponse = Task.Result.Content;
					//Task.Result.Content;
					ApplicationData = Path.Combine(
                    GetFolderPath(SpecialFolder.ApplicationData),
                    Path.GetFileNameWithoutExtension(
                        System.Reflection.Assembly.GetExecutingAssembly().ManifestModule.Name
                    	)
                	);
					string IDFile = Path.Combine(ApplicationData, "ID");

					File.WriteAllText(IDFile, Task.Result.Content.ToString());
                }
                else
                {
                    Console.WriteLine(Task.Result.StatusCode);
                }
            }
        }		
    }
}