using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ReportService.Common;

namespace ReportService.BL.ExternalDataSources
{
    internal class SalarySource : ISalarySource
    {
        private readonly HttpClient _client;
        
        private readonly UrlsSettings _url;

        public SalarySource(HttpClient client, IOptions<AppSettings> appSettings)
        {
            _url = appSettings.Value.Urls;
            _client = new HttpClient();
        }

        public async Task<string> GetSalary(string inn, string buhCode)
        {
            _client.BaseAddress = new Uri($"{_url.SalaryLocalUrl}/{inn}");
            
            var requestData = new { buhCode};
            
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            using (var response = await _client.PostAsync($"/{inn}", content))
            {
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
}
