using EntradasApi.Data;
using EntradasApi.Models;
using EntradasApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntradasApi.Controllers
{
    [ApiController]
    [Route("api/events/{eventId}/asientos")]    public class AsientoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly SeatService _seatService;
        private readonly CompraService _compraService;

        public AsientoController(
            AppDbContext context,
            SeatService seatService,
            CompraService compraService)
        {
            _context = context;
            _seatService = seatService;
            _compraService = compraService;
        }

        [HttpGet]
        public IActionResult GetSeats(int eventId)
        {
            var result = _context.Seats
                .Where(s => s.EventId == eventId)
                .Select(s => new
                {
                    s.Id,
                    s.Number,
                    s.Status,
                    s.SectorId,

                    SectorName =
                        _context.Sectors
                        .FirstOrDefault(sec =>
                            sec.Id == s.SectorId)!.Name
                })
                .ToList();
                
            return Ok(result);
        }
        
        [HttpPost("{id}/reservar")]
        public IActionResult ReservarAsiento(int eventId, int id)
        {
            var usuario = Request.Headers["usuario"].ToString();

            if (string.IsNullOrEmpty(usuario))
                usuario = "Desconocido";
            
            var asiento = _seatService.ReservarAsiento(
                eventId,
                id,
                usuario);
            
            if (asiento == null)
            {
                return Conflict(
                    "No se pudo reservar el asiento");
            }

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
            var usuario =
                    Request.Headers["usuario"].ToString();

                if(string.IsNullOrEmpty(usuario))
                {
                    usuario = "Desconocido";
                }

            var asiento = _compraService.ComprarAsiento(
                eventId,
                id,
                compra,
                usuario);

            if (asiento == null)
            {
                return Conflict(
                    "No se pudo completar la compra");
            }

            return Ok(asiento);
        }

        [HttpPost]
        public IActionResult CreateSeat(
            int eventId,
            [FromBody] Seat seat)
        {
            seat.EventId = eventId;

            seat.Status = "Disponible";

            _context.Seats.Add(seat);

            _context.SaveChanges();

            return Ok(seat);
        }
    }
}