using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

using BackupServiceDaemon.Models;

namespace BackupServiceDaemon
{
    public static class APIService
    {
		public static string Token { get; set; }
        
        public static Computer GetSelf() {
            if (Token == null)
                throw new Exception("No token");

            using (var handler = new HttpClientHandler()) {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                using (var client = new HttpClient(handler)) {
                    var request = new HttpRequestMessage() {
                        RequestUri = new Uri(SettingsService.Settings.Server + "computers/self"),
                        Method = HttpMethod.Get,
                    };
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                    var requestTask = client.SendAsync(request);
                    requestTask.Wait();
                    
                    if (requestTask.Result.IsSuccessStatusCode) {
                        var readTask = requestTask.Result.Content.ReadAsAsync<Computer>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    throw new Exception(requestTask.Result.ToString());
                }
            }
        }

        public static Job[] GetJobs() {
            if (Token == null)
                throw new Exception("No token");

            using (var handler = new HttpClientHandler()) {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                using (var client = new HttpClient(handler)) {
                    var request = new HttpRequestMessage() {
                        RequestUri = new Uri(SettingsService.Settings.Server + "jobs/computer"),
                        Method = HttpMethod.Get,
                    };
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

                    var requestTask = client.SendAsync(request);
                    requestTask.Wait();
                    
                    if (requestTask.Result.IsSuccessStatusCode) {
                        var readTask = requestTask.Result.Content.ReadAsAsync<Job[]>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    throw new Exception(requestTask.Result.ToString());
                }
            }
        }
        
		public static string GetToken(TokenRequest tokenRequest) {
            using (var handler = new HttpClientHandler()) {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

                using (var client = new HttpClient(handler)) {
                    var request = new HttpRequestMessage() {
                        RequestUri = new Uri(SettingsService.Settings.Server + "token/computer"),
                        Method = HttpMethod.Post,
                    };
                    request.Content = new ObjectContent(typeof(TokenRequest), tokenRequest, new JsonMediaTypeFormatter(), new MediaTypeHeaderValue("application/json"));

                    var requestTask = client.SendAsync(request);
                    requestTask.Wait();
                    
                    if (requestTask.Result.IsSuccessStatusCode) {
                        var readTask = requestTask.Result.Content.ReadAsAsync<TokenResponse>();
                        readTask.Wait();

                        return readTask.Result.Token;
                    }
                    throw new Exception(requestTask.Result.ToString());
                }
            }
		}

		public static Computer Register(ComputerRegistration registration)
        {
            using (var handler = new HttpClientHandler()) {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                
                using (var client = new HttpClient(handler)) {
                    var request = new HttpRequestMessage() {
                        RequestUri = new Uri(SettingsService.Settings.Server + "computers/register"),
                        Method = HttpMethod.Post,
                    };
                    request.Content = new ObjectContent(typeof(ComputerRegistration), registration, new JsonMediaTypeFormatter(), new MediaTypeHeaderValue("application/json"));

                    var requestTask = client.SendAsync(request);
                    requestTask.Wait();
                    
                    if (requestTask.Result.IsSuccessStatusCode) {
                        var readTask = requestTask.Result.Content.ReadAsAsync<Computer>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    throw new Exception(requestTask.Result.ToString());
                }
            }
        }
        public static void SendReport(LogItem report) {
            using (var handler = new HttpClientHandler()) {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                
                using (var client = new HttpClient(handler)) {
                    var request = new HttpRequestMessage() {
                        RequestUri = new Uri(SettingsService.Settings.Server + "log"),
                        Method = HttpMethod.Post,
                    };
                    request.Content = new ObjectContent(typeof(LogItem), report, new JsonMediaTypeFormatter(), new MediaTypeHeaderValue("application/json"));

                    var requestTask = client.SendAsync(request);
                    requestTask.Wait();
                    
                    if (requestTask.Result.IsSuccessStatusCode) {
                        var readTask = requestTask.Result.Content.ReadAsAsync<Computer>();
                        readTask.Wait();

                        return readTask.Result;
                    }
                    throw new Exception(requestTask.Result.ToString());
                }
            }
        }
    }
}