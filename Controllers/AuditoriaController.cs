using Microsoft.AspNetCore.Mvc;
using EntradasApi.Data;

namespace EntradasApi.Controllers
{
    [ApiController]

    [Route("api/[controller]")]
    public class AuditoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditoriaController(
            AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetLogs()
        {
            var logs =
                _context.Auditorias
                .OrderByDescending(a => a.Fecha)
                .ToList();

            return Ok(logs);
        }
    }
    
}