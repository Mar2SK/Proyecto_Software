using Microsoft.AspNetCore.Mvc;
using EntradasApi.Models;
using EntradasApi.Data;

namespace EntradasApi.Controllers
{
    [ApiController]
    [Route("api/events/{eventId}/[controller]")]
    public class AsientoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AsientoController(AppDbContext context)
        {
            _context = context;
        }

        public static List<Seat> seats = new List<Seat>
        {
            new Seat { Id = 1, EventId = 1, Number = 1 },
            new Seat { Id = 2, EventId = 1, Number = 2 },
            new Seat { Id = 3, EventId = 1, Number = 3 },
            new Seat { Id = 4, EventId = 2, Number = 1 },
            new Seat { Id = 5, EventId = 2, Number = 2 }
        };

        [HttpGet]
        public IActionResult GetSeats(int eventId)
        {
            var result = _context.Seats
            .Where(s => s.EventId == eventId)
            .ToList();
            return Ok(result);
        }

        [HttpPost("{id}/reservar")]
        public IActionResult ReservarAsiento(int eventId, int id)
        {
            //DOBLE VALIDACION
            var asiento = _context.Seats
            .Where(s => s.Id == id && s.EventId == eventId)
            .FirstOrDefault();

            if (asiento == null)
            return NotFound("Asiento no encontrado");

            if (asiento.Status != "Disponible")
            return Conflict("El asiento ya está reservado o vendido");

            asiento.Status = "Reservado";
            asiento.ReservationTime = DateTime.Now;
            _context.SaveChanges();

            // AUDITORA
            AuditoriaController.logs.Add(new Auditoria
            {
                Accion = "Reserva",
                Descripcion = $"Se reservó el asiento {asiento.Id}",
            });

            return Ok(asiento);
        }

        [HttpPost("{id}/comprar")]
        public IActionResult ComprarAsiento(int eventId, int id)
        {
            var asiento = _context.Seats
            .FirstOrDefault(s => s.Id == id && s.EventId == eventId);

            if (asiento == null)
            return NotFound("Asiento no encontrado");

            if (asiento.Status != "Reservado")
            return Conflict("El asiento debe estar reservado antes de comprar");

            asiento.Status = "Vendido";
            _context.SaveChanges();
            
            AuditoriaController.logs.Add(new Auditoria
            {
            Accion = "Compra",
            Descripcion = $"Se compró el asiento {asiento.Id}",
            });
            return Ok(asiento);
        }
    }
}