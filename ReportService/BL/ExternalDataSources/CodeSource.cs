using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ReportService.Common;

namespace ReportService.BL.ExternalDataSources
{
    internal class CodeSource : ICodeSource
    {
        private static readonly HttpClient Client = new HttpClient();
        
        private readonly UrlsSettings _url;

        public CodeSource(IOptions<AppSettings> appSettings)
        {
            _url = appSettings.Value.Urls;
        }

        public async Task<string> GetCode(string inn)
        {
            using (var response = await Client.GetAsync($"{_url.BuchLocalUrl}/{inn}"))
            {
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
