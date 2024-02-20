using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReportService.BL.Services;

namespace ReportService.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportGenerator _reportGenerator;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportGenerator reportGenerator, ILogger<ReportController> logger)
        {
            _reportGenerator = reportGenerator;
            _logger = logger;
        }

        [HttpGet("{year:int}/{month:int}")]
        public async Task<IActionResult> Download([Range(1, 9999, ErrorMessage = "Year must be between 1 and 9999")]
            int year,
            [Range(1, 12, ErrorMessage = "Month must be between 1 and 12")] int month)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            { 
               var report = await _reportGenerator.Create( year,  month);

               var response = GenerateFileStreamFromString(report.Content, report.Name);

                return Ok(response);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"{ex}");
                return BadRequest("Service is not working");
            }        
        }

        private FileStreamResult GenerateFileStreamFromString(string content, string name)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                return File(stream, "application/octet-stream", name);
            }
        }
    }
}
