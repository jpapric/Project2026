using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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


    }
}
