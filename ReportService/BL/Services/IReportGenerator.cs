using System.Threading.Tasks;
using ReportService.BL.Models;

namespace ReportService.BL.Services
{
    public interface IReportGenerator
    {
        Task<Report> Create(int year, int month);
    }
}