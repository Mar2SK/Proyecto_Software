using EntradasApi.Data;
using EntradasApi.Models;
using EntradasApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=entradas.db"));

builder.Services.AddControllers();

builder.Services.AddScoped<SeatService>();
builder.Services.AddScoped<CompraService>();

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

using(var scope = app.Services.CreateScope())
{
    var context =
        scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    context.Database.EnsureCreated();

    if(!context.Usuarios.Any())
    {
        context.Usuarios.AddRange(

            new Usuario
            {
                Nombre = "Martin",
                Username = "martin",
                Email = "martin@mail.com",
                Password = "123",
                Rol = "User"
            },

            new Usuario
            {
                Nombre = "Joaquin",
                Username = "joaquin",
                Email = "joaquin@mail.com",
                Password = "123",
                Rol = "User"
            },

            new Usuario
            {
                Nombre = "Ivanna",
                Username = "ivanna",
                Email = "ivanna@mail.com",
                Password = "123",
                Rol = "User"
            },

            new Usuario
            {
                Nombre = "Admin",
                Username = "admin",
                Email = "admin@admin.com",
                Password = "1212",
                Rol = "Admin"
            }
        );

        context.SaveChanges();
    }

    if(!context.Events.Any())
    {
        context.Events.AddRange(

            new Event
            {
                Name = "Duki World Tour",

                Venue =
                    "Movistar Arena • Buenos Aires • 24 de Junio",

                ImageUrl =
                    "https://enagenda.com.ar/uploads/noticias/5/2025/06/20250602181554_duki-x-irishsuarez-036.jpg"
            },

            new Event
            {
                Name = "BOCA vs RIVER",

                Venue =
                    "River Plate • Buenos Aires • 26 de Junio",

                ImageUrl =
                    "https://www.lanacion.com.ar/resizer/v2/la-durisima-patada-de-marcos-rojo-que-derivo-en-GVH4JYH6PFBY7HL6YUXSDWR7OE.JPG?auth=452b2d8910b20b48db97c01a0bf265ee4afd8ade068e79f953376ad023b01b76&width=1200&height=800&quality=70&smart=true"
            }
        );

        context.SaveChanges();
    }

    var eventos = context.Events.ToList();

    foreach(var evento in eventos)
    {
        bool tieneAsientos =
            context.Seats.Any(s => s.EventId == evento.Id);

        if(tieneAsientos)
            continue;

        var platea = new Sector
        {
            Name = "Platea",
            EventId = evento.Id
        };

        context.Sectors.Add(platea);
        context.SaveChanges();

        for(int i = 1; i <= 50; i++)
        {   
            context.Seats.Add(
                new Seat
                {
                    EventId = evento.Id,
                    SectorId = platea.Id,
                    Number = i,
                    Status = "Disponible"
                });
        }

        var campo = new Sector
        {
            Name = "Campo",
            EventId = evento.Id
        };

        context.Sectors.Add(campo);
        context.SaveChanges();

        for(int i = 1; i <= 50; i++)
        {
            context.Seats.Add(
                new Seat
                {
                    EventId = evento.Id,
                    SectorId = campo.Id,
                    Number = i,
                    Status = "Disponible"
                });
        }

        context.SaveChanges();
    }
}

app.Run();