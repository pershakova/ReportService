using System;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ReportService.Common;
using Microsoft.Extensions.Logging;
using ReportService.Controllers;

namespace ReportService.Repository
{
    internal class NpgsqlRepository : INpgsqlRepository
    {
        private readonly ILogger<ReportController> _logger;
        private readonly string _connectionString;
        private const string CommandString = "SELECT e.name, e.inn, d.name from emps e join deps d on e.departmentid = d.id where d.active = true";

        public NpgsqlRepository(IOptions<AppSettings> appSettings, ILogger<ReportController> logger)
        {
            _connectionString = Environment.GetEnvironmentVariable("DefaultConnection");//appSettings.Value.ConnectionString;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeDbEntry>> GetEmployeesAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    var command = new NpgsqlCommand(CommandString, connection);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var employees = new List<EmployeeDbEntry>();

                        while (await reader.ReadAsync())
                        {
                            var emp = new EmployeeDbEntry
                            {
                                Name = reader.GetString(0),
                                Inn = reader.GetString(1),
                                Department = reader.GetString(2)
                            };

                            employees.Add(emp);
                        }

                        return employees;
                    }
                }
                catch (Exception)
                {
                    _logger.LogError("Error during working with database");
                    throw;
                }
            }
        }
    }
}
