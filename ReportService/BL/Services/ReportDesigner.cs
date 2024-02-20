using System.Text;
using ReportService.BL.Models;

namespace ReportService.BL.Services
{
    public class ReportDesigner : IReportDesigner
    {
        private readonly string _header;
        private readonly Organization _organization;

        public ReportDesigner(string header, Organization organization)
        {
            _header = header;
            _organization = organization;
        }

        private ReportDesigner()
        {
        }

        public string Generate()
        {
            var aggregationSb = new StringBuilder();
            aggregationSb.Append(_header);

            AddHeaderLines(aggregationSb);

            foreach (var department in _organization.Deparments)
            {
                aggregationSb.AppendLine(department.Name);

                foreach (var employee in department.Employees)
                {
                    aggregationSb.Append(employee.Name);
                    aggregationSb.Append(' ', 8);
                    aggregationSb.Append(employee.Salary);
                    aggregationSb.AppendLine("р");
                }

                aggregationSb.Append("Всего по отделу");
                aggregationSb.Append(' ', 8);
                aggregationSb.Append(department.GetTotalSalary());
                aggregationSb.AppendLine("р");
            }

            AddFooter(aggregationSb, _organization.GetTotalSalary().ToString());

            return aggregationSb.ToString();
        }

        private void AddHeaderLines(StringBuilder aggregationSb)
        {
            aggregationSb.AppendLine();
            aggregationSb.Append('_', 44);
            aggregationSb.AppendLine();
        }

        private void AddFooter(StringBuilder aggregationSb, string salarySum)
        {
            aggregationSb.AppendLine();
            aggregationSb.Append('_', 44);
            aggregationSb.AppendLine();
            aggregationSb.Append("Всего по предприятию");
            aggregationSb.Append(' ', 8);
            aggregationSb.Append(salarySum);
            aggregationSb.AppendLine("р");
        }
    }
}
