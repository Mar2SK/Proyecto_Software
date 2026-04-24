using EntradasApi.Data;
using EntradasApi.Models;
using EntradasApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=entradas.db"));

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (para frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Background Service
builder.Services.AddHostedService<SeatExpirationService>();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

// CORS
app.UseCors("AllowAll");

// 👇 ESTO ES LO QUE TE FALTABA
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();


// 🔥 SEED DE DATOS
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    db.Database.EnsureCreated();

    // Evento
    if (!db.Events.Any())
    {
        db.Events.AddRange(
            new Event { Name = "Recital", Venue = "Teatro" },
            new Event { Name = "Partido", Venue = "Estadio" }
        );
        db.SaveChanges();
    }

    // Sectores
    if (!db.Sectors.Any())
    {
        db.Sectors.AddRange(
            new Sector { Name = "Sector A", EventId = 1 },
            new Sector { Name = "Sector B", EventId = 1 }
        );
        db.SaveChanges();
    }

    // Asientos
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

        // 50 asientos Platea
        for (int i = 1; i <= 50; i++)
        {
            db.Seats.Add(new Seat
            {
                EventId = evento.Id,
                Number = i,
                SectorId = sectorA.Id
            });
        }

        // 50 asientos Campo
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

app.Run();