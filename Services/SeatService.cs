using EntradasApi.Data;
using EntradasApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EntradasApi.Services
{
    public class SeatService
    {
        private readonly AppDbContext _context;

        public SeatService(AppDbContext context)
        {
            _context = context;
        }

        public Seat? ReservarAsiento(
            int eventId,
            int id,
            string usuario)
        {
            var asiento = _context.Seats
                .FirstOrDefault(s =>
                    s.Id == id &&
                    s.EventId == eventId);

            if (asiento == null)
                return null;

            if (asiento.Status != "Disponible")
                return null;

            asiento.Status = "Reservado";
            asiento.ReservationTime = DateTime.UtcNow;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }

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

            return asiento;
        }
    }
}