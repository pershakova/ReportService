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
        private static readonly HttpClient Client = new HttpClient();
        
        private readonly UrlsSettings _url;

        public SalarySource(IOptions<AppSettings> appSettings)
        {
            _url = appSettings.Value.Urls;
        }

        public async Task<string> GetSalary(string inn, string buhCode)
        {
            Client.BaseAddress = new Uri($"{_url.SalaryLocalUrl}/{inn}");
            
            var requestData = new { buhCode};
            
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            using (var response = await Client.PostAsync($"/{inn}", content))
            {
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
        }
    }
}
