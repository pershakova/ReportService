namespace ReportService.Common
{
    internal class AppSettings
    {
        public string ConnectionString { get; set; }
        
        public UrlsSettings Urls { get; set; }
    }
    
    internal class UrlsSettings
    {
        public string SalaryLocalUrl { get; set; }
        public string BuchLocalUrl { get; set; }
    }
}
