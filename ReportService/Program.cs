using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using NLog;
using NLog.Web;
using System;

namespace ReportService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup()
                       .LoadConfigurationFromFile("NLog.config")
                       .GetCurrentClassLogger();
            try
            {

                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Application stopped because of an exception.");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
            
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().UseNLog()
                .Build();
    }
}
