using EntradasApi.Data;
using EntradasApi.Models;

namespace EntradasApi.Services
{
    public class CompraService
    {
        private readonly AppDbContext _context;

        public CompraService(AppDbContext context)
        {
            _context = context;
        }

        public Seat? ComprarAsiento(
            int eventId,
            int id,
            CompraRequest compra,
            string usuario)
        {
            using var transaction =
                _context.Database.BeginTransaction();

            try
            {
                var asiento = _context.Seats
                    .FirstOrDefault(s =>
                        s.Id == id &&
                        s.EventId == eventId);

                if (asiento == null)
                    return null;

                if(asiento.Status != "Reservado")
                    return null;

                asiento.Status = "Vendido";
                asiento.ReservationTime = null;

                _context.SaveChanges();

                _context.Auditorias.Add(
                    new Auditoria
                    {
                        Accion = "Compra",

                        Usuario = usuario,

                        Descripcion =
                            $"Usuario: {usuario} | " +
                            $"Comprador: {compra.Nombre} {compra.Apellido} | " +
                            $"Email: {compra.Email} | " +
                            $"Evento: {eventId} | " +
                            $"Asiento: {asiento.Number}",

                        Fecha = DateTime.Now
                    });

                _context.SaveChanges();

                transaction.Commit();
                _context.Entry(asiento).Reload();
                return asiento;
            }
            catch
            {
                transaction.Rollback();
                return null;
            }
        }
    }
}