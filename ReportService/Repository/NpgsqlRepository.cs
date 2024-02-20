using System;
using Npgsql;
using System.Collections.Generic;
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

        public IEnumerable<EmployeeDbEntry> GetEmployees()
        {       
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    var command = new NpgsqlCommand(CommandString, connection);
                    var reader1 = command.ExecuteReader();

                    var employees = new List<EmployeeDbEntry>();

                    while (reader1.Read())
                    {
                        var emp = new EmployeeDbEntry { Name = reader1.GetString(0), Inn = reader1.GetString(1), Department = reader1.GetString(2) };

                        employees.Add(emp);
                    }
                    return employees;
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
