using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RemoteCamera.HubClient
{
    public class SessionClient
    {
        private HttpClient _httpClient;

        public SessionClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<string> CreateNewSessionAsync()
        {
            var response = await _httpClient.PostAsync("/session", new StringContent("{}"));
            var responseText = await response.Content.ReadAsStringAsync();

            return responseText;
        }

        public async Task<bool> ConnectToSessionAsync(string sessionName)
        {
            var response = await _httpClient.GetAsync($"/session?sessionName={sessionName}");

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
