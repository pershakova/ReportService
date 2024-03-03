using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReportService.Repository
{
    public interface INpgsqlRepository
    {
        Task<IEnumerable<EmployeeDbEntry>>  GetEmployeesAsync();
    }
}
