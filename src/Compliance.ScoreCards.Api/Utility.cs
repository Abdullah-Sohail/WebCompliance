using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Compliance.ScoreCards.Api
{
    internal class Utility
    {
        private static Uri _baseUri = new Uri("http://localhost:15425/");
        //private static Uri _baseUri = new Uri("http://api.ccscollect.com");

        public async Task<HttpResponseMessage> PostApiAsync(string apiUrl, object reqCmd, string developerId, string secretKey)
        {
            var client = new HttpClient();

            client.BaseAddress = _baseUri;
            client.Timeout = TimeSpan.FromSeconds(30);

            return client.PostAsJsonAsync(String.Format("/{0}", apiUrl), reqCmd).Result;
           // return client.PostAsJsonAsync(String.Format("/scorecard/{0}", apiUrl), reqCmd).Result;
        }

        public async Task<HttpResponseMessage> GetApiAsync(string apiUrl, string developerId, string secretKey)
        {
            var client = new HttpClient();

            client.BaseAddress = _baseUri;
            client.Timeout = TimeSpan.FromSeconds(30);

            return client.GetAsync(String.Format("/{0}", apiUrl)).Result;
            //return client.GetAsync(String.Format("/scorecard/{0}", apiUrl)).Result;
        }
    }
}
