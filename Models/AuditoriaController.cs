using Microsoft.AspNetCore.Mvc;
using EntradasApi.Models;

namespace EntradasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditoriaController : ControllerBase
    {
        public static List<Auditoria> logs = new List<Auditoria>();

        [HttpGet]
        public IActionResult GetLogs()
        {
            return Ok(logs);
        }
    }
}