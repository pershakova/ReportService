using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportService.BL.Models
{
    public class Department
    {
        public Department(IEnumerable<Employee> employees, string name)
        {
            Employees = employees ?? throw new ArgumentNullException(nameof(employees));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        public IEnumerable<Employee> Employees { get; }
        public string Name { get; }
        public decimal GetTotalSalary()
        {
            return Employees.Sum(x => x.Salary);
        }
    }
}