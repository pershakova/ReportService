using System.Collections.Generic;

namespace ReportService.Repository
{
    public interface INpgsqlRepository
    {
        IEnumerable<EmployeeDbEntry> GetEmployees();
    }
}
