using Client.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.Proxies
{
    public class EAFProxy
    {
        private HttpClient _httpClient;

        public EAFProxy()
        {
            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<PLCDto> GetPlc()
        {
            var response = await _httpClient.GetAsync("GetPlc");
            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();
            PLCDto result = JsonConvert.DeserializeObject<PLCDto>(json);

            return result;

        }

        public async Task UpdatePlc(PLCDto plc)
        {
            string json = JsonConvert.SerializeObject(plc);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("UpdatePlc", content);
            response.EnsureSuccessStatusCode();
        }


    }
}
