using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
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
            try
            {
                var employeesWithExternalTasks = (await _npgsqlRepository.GetEmployeesAsync())
                    .Select(async x =>
                    {
                        try
                        {
                            var code = await GetCodeWithRetry(x.Inn);
                            var salary = await GetSalaryWithRetry(x.Inn, code);

                            return new { Employee = x, Code = code, Salary = Convert.ToDecimal(salary)};
                        }
                        catch (Exception)
                        {
                            _logger.LogError($"Exception while processing employee {x.Inn}");
                            throw;
                        }
                    });

                var employeesWithExternalData = await Task.WhenAll(employeesWithExternalTasks);

                var departments = employeesWithExternalData
                    .GroupBy(x => x.Employee.Department)
                    .Select(x => new Department(
                        x.Select(y => new Employee(
                            y.Employee.Name,
                            y.Employee.Inn,
                            Convert.ToDecimal(y.Salary))),
                        x.Key));

                return new Organization(departments);
            }
            catch (Exception)
            {
                _logger.LogError("Exception during creation Organization");
                throw;
            }
        }
        
        private async Task<string> GetCodeWithRetry(string inn)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .RetryAsync(3, (exception, retryCount) =>
                {
                    _logger.LogWarning($"Exception in GetCode. Retry count: {retryCount}. Exception: {exception}");
                });

            return await retryPolicy.ExecuteAsync(async () => await _codeSource.GetCode(inn));
        }

        private async Task<string> GetSalaryWithRetry(string inn, string code)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .RetryAsync(3, (exception, retryCount) =>
                {
                    _logger.LogWarning($"Exception in GetSalary. Retry count: {retryCount}. Exception: {exception}");
                });

            return await retryPolicy.ExecuteAsync(async () => await _salarySource.GetSalary(inn, code));
        }
    }
}