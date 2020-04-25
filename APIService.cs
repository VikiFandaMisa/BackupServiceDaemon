using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

using BackupServiceDaemon.Models;

namespace BackupServiceDaemon
{
    public static class APIService
    {
        public static string Server { get; set; }
		private static string Token { get; set; }
        public static Computer GetSelf() {
            using (var client = new HttpClient()) {
                var request = new HttpRequestMessage() {
                    RequestUri = new Uri(Server + "computers/self"),
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
		public static string GetToken(TokenRequest tokenRequest) {
			using (var client = new HttpClient()) {
                var request = new HttpRequestMessage() {
                    RequestUri = new Uri(Server + "token/computer"),
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
		public static Computer Register(ComputerRegistration registration)
        {
            using (var client = new HttpClient()) {
                var request = new HttpRequestMessage() {
                    RequestUri = new Uri(Server + "computers/register"),
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
}