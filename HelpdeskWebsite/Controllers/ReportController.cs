//using Microsoft.AspNetCore.Mvc;
//using HelpdeskWebsite.Reports;

using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HelpdeskDAL;
using HelpdeskViewModels;
using HelpdeskWebsite.Reports;

namespace HelpdeskWebsite.Controllers
{
    public class ReportController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public ReportController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [Route("api/employeereport")]
        [HttpGet]
        public IActionResult GetEmployeeReport()
        {
            EmployeeReport employees = new();
            employees.GenerateEmployeeReport(_env.WebRootPath);
            return Ok(new { msg = "Report Generated" });
        }

        [Route("api/callreport")]
        [HttpGet]
        public IActionResult GetCallReport()
        {
            CallReport calls = new();
            calls.GenerateCallReport(_env.WebRootPath);
            return Ok(new { msg = "Report Generated" });
        }

    }
}
