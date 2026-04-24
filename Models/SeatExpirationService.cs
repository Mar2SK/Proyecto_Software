using Microsoft.EntityFrameworkCore;
using EntradasApi.Data;
using EntradasApi.Controllers;
using EntradasApi.Models;

namespace EntradasApi.Services
{
    public class SeatExpirationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public SeatExpirationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope =
                    _scopeFactory.CreateScope();

                var context =
                    scope.ServiceProvider
                    .GetRequiredService<AppDbContext>();

                var reservedSeats =
                    context.Seats
                    .Where(s =>
                        s.Status == "Reservado" &&
                        s.ReservationTime != null)
                    .ToList();

                foreach (var seat in reservedSeats)
                {
                    var minutes =
                        (DateTime.UtcNow -
                        seat.ReservationTime.Value)
                        .TotalMinutes;

                    // SOLO LIBERAR DESPUÉS DE 5 MIN
                    if (minutes >= 5)
                    {
                        seat.Status = "Disponible";

                        seat.ReservationTime = null;

                        AuditoriaController.logs.Add(
                            new Auditoria
                            {
                                Accion = "Liberación",

                                Descripcion =
                                    $"Se liberó el asiento {seat.Number}"
                            });
                    }
                }

                context.SaveChanges();

                // REVISAR CADA 5 SEGUNDOS
                await Task.Delay(
                    5000,
                    stoppingToken);
            }
        }
    }
}