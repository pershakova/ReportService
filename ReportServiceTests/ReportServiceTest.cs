using Moq;
using ReportService.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportService.BL.ExternalDataSources;
using ReportService.BL.Models;
using ReportService.BL.Services;
using ReportService.Controllers;
using ReportService.Repository;

namespace ReportServiceTests
{
    public class ReportServiceTest
    {
        [Fact]
        public void CheckReportDesigner()
        {
            var header = Header.GetMonthYearString(2022, 12);

            var organization = GetOrganization();

            var report = new ReportDesigner(header, organization).Generate();

            Assert.Equal("декабрь 2022\r\n"+
                         "____________________________________________\r\n"+
                         "Бухгалтерия\r\n"+
                         "Михаил Андреевич Суслов        35000р\r\n"+
                         "Григорий Евсеевич Зиновьев        65000р\r\n"+
                         "Всего по отделу        100000р\r\n"+
                         "ИТ\r\n"+
                         "Демьян Сергеевич Коротченко        55000р\r\n"+
                         "Всего по отделу        55000р\r\n"+
                         "\r\n____________________________________________\r\n"+
                         "Всего по предприятию        155000р\r\n", 
                report);
        }
        
        [Fact]
        public async Task CheckReportGenerator()
        {
            // Arrange
            var salarySourceMock = new Mock<ISalarySource>();
            var codeSourceMock = new Mock<ICodeSource>();
            var npgsqlRepositoryMock = new Mock<INpgsqlRepository>();
            var loggerMock = new Mock<ILogger<ReportController>>();

            var reportGenerator = new ReportGenerator(
                salarySourceMock.Object,
                codeSourceMock.Object,
                npgsqlRepositoryMock.Object,
                loggerMock.Object
            );
            
            var employeeDbEntries = GetEmployeeDbEntry();
            npgsqlRepositoryMock.Setup(x => x.GetEmployees()).Returns(employeeDbEntries);

            codeSourceMock.Setup(x => x.GetCode(It.IsAny<string>()))
                .ReturnsAsync((string inn) => $"{inn}");
            
            salarySourceMock.Setup(x => x.GetSalary(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string inn, string buchCode) => $"{inn}"); // mocked salary
            
            var year = 2024;
            var month = 2;

            // Act
            var result = await reportGenerator.Create(year, month);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("report.txt", result.Name);
            Assert.Equal(
                "февраль 2024\r\n" + 
                "____________________________________________\r\n" + 
                "ИТ\r\n"+
                "Михаил Андреевич Суслов        1000р\r\n"+
                "Василий Васильевич Кузнецов        2000р\r\n"+
                "Всего по отделу        3000р\r\n"+
                "Бухгалерия\r\n"+
                "Демьян Сергеевич Коротченко        1500р\r\n"+
                "Фрол Романович Козлов         1300р\r\n"+
                "Всего по отделу        2800р\r\n"+
                "\r\n____________________________________________\r\n"+
                "Всего по предприятию        5800р\r\n",
                result.Content);
        }
        
        [Fact]
        public async Task Download_ReturnsOkResult()
        {
            // Arrange
            var reportGeneratorMock = new Mock<IReportGenerator>();
            var loggerMock = new Mock<ILogger<ReportController>>();

            var reportController = new ReportController(reportGeneratorMock.Object, loggerMock.Object);

            var year = 2024;
            var month = 2;

            var reportContent = GetReport();
            var reportName = "report.txt";
            var report = new Report { Name = reportName, Content = reportContent };
            reportGeneratorMock.Setup(x => x.Create(year, month)).ReturnsAsync(report);

            // Act
            var result = await reportController.Download(year, month);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            Assert.IsType<FileStreamResult>(okResult.Value); 
        }
        
        [Theory]
        [InlineData(0, 2)]
        [InlineData(2024, 13)]
        public async Task Download_WithInvalidInputs_ReturnsBadRequest(int year, int month)
        {
            // Arrange
            var reportGeneratorMock = new Mock<IReportGenerator>();
            var loggerMock = new Mock<ILogger<ReportController>>();

            var reportController = new ReportController(reportGeneratorMock.Object, loggerMock.Object);

            // Act
            var result = await reportController.Download(year, month);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.NotNull(badRequestResult.Value);
        }
        
        private Organization GetOrganization()
        {
            var emp1 = new Employee("Михаил Андреевич Суслов", string.Empty, 35000);
            var emp2 = new Employee("Григорий Евсеевич Зиновьев", string.Empty, 65000);
            var emp3 = new Employee( "Демьян Сергеевич Коротченко", string.Empty, 55000);

            var department1 = new Department( new List<Employee>{emp1, emp2},"Бухгалтерия");
            var department2 = new Department( new List<Employee>{emp3},"ИТ");

            var organization = new Organization(new List<Department> { department1, department2 });

            return organization;
        }
        
        private List<EmployeeDbEntry> GetEmployeeDbEntry()
        {
            var entry1 = new EmployeeDbEntry{Department = "ИТ", Inn = "1000", Name = "Михаил Андреевич Суслов"};
            var entry2 = new EmployeeDbEntry{Department = "Бухгалерия", Inn = "1500", Name = "Демьян Сергеевич Коротченко"};
            var entry3 = new EmployeeDbEntry{Department = "ИТ", Inn = "2000", Name = "Василий Васильевич Кузнецов"};
            var entry4 = new EmployeeDbEntry{Department = "Бухгалерия", Inn = "1300", Name = "Фрол Романович Козлов "};

            var list = new List<EmployeeDbEntry> { entry1, entry2, entry3, entry4 };
            return list;
        }

        private string GetReport()
        {
            return "декабрь 2022\r\n" +
                   "____________________________________________\r\n" +
                   "Бухгалтерия\r\n" +
                   "Михаил Андреевич Суслов        35000р\r\n" +
                   "Григорий Евсеевич Зиновьев        65000р\r\n" +
                   "Всего по отделу        100000р\r\n" +
                   "ИТ\r\n" +
                   "Демьян Сергеевич Коротченко        55000р\r\n" +
                   "Всего по отделу        55000р\r\n" +
                   "\r\n____________________________________________\r\n" +
                   "Всего по предприятию        155000р\r\n";
        }
    }
}