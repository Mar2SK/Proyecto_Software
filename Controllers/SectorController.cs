using Microsoft.AspNetCore.Mvc;
using EntradasApi.Data;

namespace EntradasApi.Controllers
{
    [ApiController]
    [Route("api/events/{eventId}/sectores")]
    public class SectorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SectorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetSectors(int eventId)
        {
            var sectores = _context.Sectors
                .Where(s => s.EventId == eventId)
                .ToList();

            return Ok(sectores);
        }
    }
}