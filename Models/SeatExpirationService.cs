using EntradasApi.Data;
using EntradasApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EntradasApi.Services
{
    public class SeatExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeatExpirationService> _logger;

        // Constructor que inyecta IServiceProvider para poder crear scopes de DB
        public SeatExpirationService(IServiceProvider serviceProvider, ILogger<SeatExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de expiración de asientos iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                CheckExpiredSeats();

                // Espera 10 segundos antes de la próxima ejecución
                await Task.Delay(10000, stoppingToken);
            }
        }

        private void CheckExpiredSeats()
        {
            // Creamos un scope manual porque DbContext es un servicio "Scoped" 
            // y los BackgroundService son "Singleton"
            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var now = DateTime.Now;

                // Buscamos asientos reservados cuya reserva expiró (más de 30 segundos)
                var reservedSeats = db.Seats
                .Where(s => s.Status == "Reservado" && s.ReservationTime.HasValue)
                .ToList();

                var expiredSeats = reservedSeats
                .Where(s => (now - s.ReservationTime!.Value).TotalSeconds > 30)
                .ToList();

                if (expiredSeats.Any())
                {
                    foreach (var seat in expiredSeats)
                    {
                        seat.Status = "Disponible";
                        seat.ReservationTime = null;

                        // Agregamos el registro a la tabla de Auditoria de la DB
                        db.Auditorias.Add(new Auditoria
                        {
                            Accion = "Liberación",
                            Descripcion = $"Asiento {seat.Id} liberado automáticamente",
                            // Asegúrate de que tu modelo Auditoria tenga Fecha si es necesario
                        });

                        _logger.LogInformation($"Asiento {seat.Id} liberado automáticamente por expiración.");
                    }

                    // Guardamos todos los cambios en la base de datos de una sola vez
                    db.SaveChanges();
                }
            }
        }
    }
}