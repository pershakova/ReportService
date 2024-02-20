using System.Threading.Tasks;

namespace ReportService.BL.ExternalDataSources
{
    public interface ICodeSource
    {
        Task<string> GetCode(string inn);
    }
}
