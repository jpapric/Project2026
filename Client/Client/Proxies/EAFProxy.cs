using Client.Models;
using System;
﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Client.Models;

namespace Client.Proxies
{
    public class EAFProxy
    {
        private HttpClient _httpClient;

        public EAFProxy()
        {
            string baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<EAFDto> GetEafDataFromPlcAsync()
        {
            var response = await _httpClient.GetAsync("GetEafDataFromPlc");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<EAFDto>(json);
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


        public async Task LoadScrapAsync()
        {
            var response = await _httpClient.PostAsync("LoadScrap", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task TapAsync()
        {
            var response = await _httpClient.PostAsync("Tap", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task ResetAsync()
        {
            var response = await _httpClient.PostAsync("Reset", null);
            response.EnsureSuccessStatusCode();
        }

        public async Task SetCurrentAsync(float current)
        {
            var content = new StringContent(
                current.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("SetCurrent", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task SetAngleAsync(float angle)
        {
            var content = new StringContent(
                angle.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("SetAngle", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<PLCDto> GetPlcAsync()
        {
            var response = await _httpClient.GetAsync("GetPlc");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<PLCDto>(json);
        }

        public async Task UpdatePlcAsync(PLCDto plc)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(plc);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("UpdatePlc", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<EventDto>> GetEventsAsync()
        {
            var response = await _httpClient.GetAsync("GetEvents");
            response.EnsureSuccessStatusCode();
            string json = await response.Content.ReadAsStringAsync();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<EventDto>>(json);
        }

        public async Task EventDetectionAsync()
        {
            var response = await _httpClient.PostAsync("Event_detection", null);
            response.EnsureSuccessStatusCode();
        }
    }
}