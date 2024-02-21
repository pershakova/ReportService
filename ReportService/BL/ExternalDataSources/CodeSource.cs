using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ReportService.Common;

namespace ReportService.BL.ExternalDataSources
{
    internal class CodeSource : ICodeSource
    {
        private readonly HttpClient _client;
        
        private readonly UrlsSettings _url;

        public CodeSource(HttpClient client, IOptions<AppSettings> appSettings)
        {
            _url = appSettings.Value.Urls;
            _client = client;
        }

        public async Task<string> GetCode(string inn)
        {
            using (var response = await _client.GetAsync($"{_url.BuchLocalUrl}/{inn}"))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
