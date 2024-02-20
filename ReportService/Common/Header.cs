using System.Globalization;
using System;

namespace ReportService.Common
{
    public static class Header
    {
        public static string GetMonthYearString(int year, int month)
        {
            return new DateTime(year, month, 1).ToString("MMMM yyyy", CultureInfo.CurrentCulture);
        }      
    }
}
