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
            var asiento = _context.Seats
                .FirstOrDefault(s =>
                    s.Id == id &&
                    s.EventId == eventId);
        
            if (asiento == null)
            {
                return NotFound("Asiento no encontrado");
            }

            // SOLO SE PUEDE RESERVAR SI ESTA DISPONIBLE
            if (asiento.Status != "Disponible")
            {
                return Conflict("El asiento no está disponible");
            }

            // USUARIO
            var usuario = Request.Headers["usuario"].ToString();
            if (string.IsNullOrEmpty(usuario))
                usuario = "Desconocido";

            // RESERVAR
            asiento.Status = "Reservado";
            asiento.ReservationTime = DateTime.UtcNow;

            _context.SaveChanges();

            _context.Auditorias.Add(
                new Auditoria
                {
                    Accion = "Reserva",
                    Usuario = usuario,
                    Descripcion =
                        $"{usuario} reservó el asiento {asiento.Number} | Evento {eventId}",
                    Fecha = DateTime.Now
                });

            _context.SaveChanges();

            return Ok(asiento);
        }

        [HttpPost("{id}/refresh")]
        public IActionResult RefreshReserva(int eventId, int id)
        {
            var asiento = _context.Seats
                .FirstOrDefault(s =>
                    s.Id == id &&
                    s.EventId == eventId);
        
            if (asiento == null)
                return NotFound();

            if (asiento.Status != "Reservado")
                return Conflict();
        
            asiento.ReservationTime = DateTime.UtcNow;
        
            _context.SaveChanges();
        
            return Ok();
        }

        [HttpPost("{id}/cancelar")]
        public IActionResult CancelarReserva(int eventId, int id)
        {
            var asiento = _context.Seats
                .FirstOrDefault(s =>
                    s.Id == id &&
                    s.EventId == eventId);

            if (asiento == null)
                return NotFound();

            if (asiento.Status == "Reservado")
            {
                // USUARIO
                var usuario = Request.Headers["usuario"].ToString();
                if (string.IsNullOrEmpty(usuario))
                    usuario = "Desconocido";

                asiento.Status = "Disponible";
                asiento.ReservationTime = null;

                _context.SaveChanges();

                _context.Auditorias.Add(
                    new Auditoria
                    {        
                        Accion = "Cancelación",
                        Usuario = usuario,
                        Descripcion =
                            $"{usuario} canceló el asiento {asiento.Number} | Evento {eventId}",
                        Fecha = DateTime.Now
                    });

                _context.SaveChanges();
            }
            return Ok();
        }

        [HttpPost("{id}/comprar")]
        public IActionResult ComprarAsiento(
            int eventId,
            int id,
            [FromBody] CompraRequest compra)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var asiento = _context.Seats
                    .FirstOrDefault(s =>
                        s.Id == id &&
                        s.EventId == eventId);
        
                if (asiento == null)
                    return NotFound("Asiento no encontrado");
        
                if (asiento.Status != "Reservado")
                    return Conflict("El asiento no está reservado");

                asiento.Status = "Vendido";
                asiento.ReservationTime = null;

                _context.SaveChanges();

                _context.Auditorias.Add(
                    new Auditoria
                    {
                        Accion = "Compra",
                        Usuario = compra.Nombre,
                        Descripcion =
                            $"{compra.Nombre} compró el asiento {asiento.Number} | Evento {eventId}",
                        Fecha = DateTime.Now
                    });

                _context.SaveChanges();

                transaction.Commit();

                return Ok(asiento);
            }
            catch
            {
                transaction.Rollback();
                return StatusCode(500, "Error en la compra");
            }
        }

    }
}