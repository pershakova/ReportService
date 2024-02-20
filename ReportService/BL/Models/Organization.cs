using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportService.BL.Models
{
    public class Organization
    {
        public Organization(IEnumerable<Department> deparments)
        {
            Deparments = deparments ?? throw new ArgumentNullException(nameof(deparments));
        }

        public IEnumerable<Department> Deparments { get; }

        public decimal GetTotalSalary()
        {
            return Deparments.Sum(x => x.GetTotalSalary());
        }
    }
}