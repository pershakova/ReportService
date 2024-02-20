using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReportService.BL.ExternalDataSources;
using ReportService.BL.Models;
using ReportService.Common;
using ReportService.Controllers;
using ReportService.Repository;

namespace ReportService.BL.Services
{
    public class ReportGenerator : IReportGenerator
    {
        private readonly ICodeSource _codeSource;
        private readonly ISalarySource _salarySource;
        private readonly INpgsqlRepository _npgsqlRepository;
        private readonly ILogger<ReportController> _logger;

        public ReportGenerator(ISalarySource salarySource, ICodeSource codeSource, INpgsqlRepository npgsqlRepository, ILogger<ReportController> logger)
        {
            _salarySource = salarySource;
            _codeSource = codeSource;
            _npgsqlRepository = npgsqlRepository;
            _logger = logger;
        }

        public async Task<Report> Create(int year, int month)
        {
            var organization = await CreateOrganization();
            
            var header = Header.GetMonthYearString(year, month);
            
            var reportContent = new ReportDesigner(header ,organization).Generate();

            var report = new Report { Name = "report.txt", Content = reportContent };

            return report;
        }

        private async Task<Organization> CreateOrganization()
        {
                var employeeDbEntries = _npgsqlRepository.GetEmployees();
                try
                {
                    var employeesWithExternalTasks = employeeDbEntries
                        .Select(async x =>
                        {
                            try
                            {
                                var code = await _codeSource.GetCode(x.Inn);
                                var salary = await _salarySource.GetSalary(x.Inn, code);

                                return new { Employee = x, Code = code, Salary = salary };
                            }
                            catch (Exception)
                            {
                                _logger.LogError($"Exception while processing employee {x.Inn}");
                                throw;
                            }
                        })
                        .ToArray();
            
                    var employeesWithExternalData = await Task.WhenAll(employeesWithExternalTasks);
            
                    var departments = employeesWithExternalData
                        .GroupBy(x => x.Employee.Department)
                        .Select(x =>
                            new Department(x.Select(y =>
                                new Employee(y.Employee.Name,
                                    y.Employee.Inn,
                                    Convert.ToDecimal(y.Salary))), x.Key));

                    return new Organization(departments);
                }
                catch (Exception)
                {
                    _logger.LogError("Exception during creation Organization");
                    throw;
                }
           
        }
    }
}