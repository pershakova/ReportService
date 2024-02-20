using System.Threading.Tasks;

namespace ReportService.BL.ExternalDataSources
{
    public interface ISalarySource
    {
        Task<string> GetSalary(string inn, string buhCode);
    }
}
