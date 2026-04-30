using Microsoft.AspNetCore.Mvc;
using EntradasApi.Data;
using EntradasApi.Models;

namespace EntradasApi.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var events = _context.Events.ToList();
            return Ok(events);
        }
    }
}