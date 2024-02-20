using System;

namespace ReportService.BL.Models
{
    public class Employee
    {
        public Employee(string name, string inn, decimal salary)
        {
            if (salary < 0) throw new ArgumentOutOfRangeException(nameof(salary));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Inn = inn ?? throw new ArgumentNullException(nameof(inn));
            Salary = salary;
        }

        public string Name { get; }
        public string Inn { get; }
        public decimal Salary { get; }
    }
}
