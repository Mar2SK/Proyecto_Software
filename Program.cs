using EntradasApi.Data;
using EntradasApi.Models;
using EntradasApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=entradas.db"));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddHostedService<SeatExpirationService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.EnsureCreated();

    if (!db.Events.Any())
    {
        db.Events.AddRange(
            new Event { Name = "Recital", Venue = "Teatro" },
            new Event { Name = "Partido", Venue = "Estadio" }
        );
        db.SaveChanges();
    }

    if (!db.Sectors.Any())
    {
        db.Sectors.AddRange(
            new Sector { Name = "Sector A", EventId = 1 },
            new Sector { Name = "Sector B", EventId = 1 }
        );
        db.SaveChanges();
    }

    if (!db.Seats.Any())
{
    var eventos = db.Events.ToList();

    foreach (var evento in eventos)
    {
        var sectorA = new Sector
        {
            Name = "Platea",
            EventId = evento.Id
        };

        var sectorB = new Sector
        {
            Name = "Campo",
            EventId = evento.Id
        };

        db.Sectors.AddRange(sectorA, sectorB);

        db.SaveChanges();

        for (int i = 1; i <= 50; i++)
        {
            db.Seats.Add(new Seat
            {
                EventId = evento.Id,
                Number = i,
                SectorId = sectorA.Id
            });
        }

        for (int i = 51; i <= 100; i++)
        {
            db.Seats.Add(new Seat
            {
                EventId = evento.Id,
                Number = i,
                SectorId = sectorB.Id
            });
        }
    }

    db.SaveChanges();
}
}

using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if(!context.Usuarios.Any())
    {
        context.Usuarios.AddRange(
            new Usuario { Username="Martin", Password="123", Rol="User" },
            new Usuario { Username="Joaquin", Password="123", Rol="User" },
            new Usuario { Username="Ivanna", Password="123", Rol="User" },
            new Usuario { Username="admin", Password="1212", Rol="Admin" }
        );

        context.SaveChanges();
    }
}

app.Run();