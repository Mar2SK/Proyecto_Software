using EntradasApi.Data;
using EntradasApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntradasApi.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var events = _context.Events
            .Select(e => new
            {
                e.Id,
                e.Name,
                e.Venue,
                e.ImageUrl,

                AvailableSeats =
                    _context.Seats.Count(s =>
                    s.EventId == e.Id &&
                    s.Status == "Disponible")
            })
            .ToList();

            return Ok(events);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if(file == null || file.Length == 0)
            {
                return BadRequest("No file");
            }

            var fileName =
                Guid.NewGuid().ToString() +
                Path.GetExtension(file.FileName);

            var path =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads",
                    fileName
                );
        
            using(var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var url =
                $"http://localhost:5123/uploads/{fileName}";

            return Ok(new { imageUrl = url });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromForm] string name,
            [FromForm] string venue,
            [FromForm] int? plateaSeats,
            [FromForm] int? campoSeats,
            IFormFile image)
        {
            if(string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(
                    "El nombre del evento es obligatorio");
            }

            if(image == null)
            {
                return BadRequest(
                    "La imagen del evento es obligatoria");
            }

            if(plateaSeats < 0 || campoSeats < 0)
            {
                return BadRequest(
                    "La cantidad de butacas no puede ser negativa");
            }

            string imagePath = "";

            // CREAR CARPETA UPLOADS
            var uploadsFolder =
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // GUARDAR IMAGEN
            if (image != null)
            {
                var fileName =
                    Guid.NewGuid() +
                    Path.GetExtension(image.FileName);

                var fullPath =
                    Path.Combine(
                        uploadsFolder,
                        fileName);

                using(var stream =
                    new FileStream(fullPath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imagePath = "/uploads/" + fileName;
            }

            // CREAR EVENTO
            var evento = new Event
            {
                Name = name,
                Venue = venue,
                ImageUrl = imagePath
            };

            _context.Events.Add(evento);

            await _context.SaveChangesAsync();

            // SECTOR PLATEA

            if(plateaSeats > 0)
            {
                var platea = new Sector
                {
                    Name = "Platea",
                    EventId = evento.Id
                };

                _context.Sectors.Add(platea);

                await _context.SaveChangesAsync();

                for(int i = 1; i <= plateaSeats; i++)
                {
                    _context.Seats.Add(
                        new Seat
                        {
                            EventId = evento.Id,
                            SectorId = platea.Id,
                            Number = i,
                            Status = "Disponible"
                        });
                }
            }

            // SECTOR CAMPO
        
            if(campoSeats > 0)
            {
                var campo = new Sector
                {
                    Name = "Campo",
                    EventId = evento.Id
                };

                _context.Sectors.Add(campo);

                await _context.SaveChangesAsync();

                for(int i = 1; i <= campoSeats; i++)
                {
                    _context.Seats.Add(
                        new Seat
                        {
                            EventId = evento.Id,
                            SectorId = campo.Id,
                            Number = i,
                            Status = "Disponible"
                        });
                }
            }

            await _context.SaveChangesAsync();

            return Created(
                $"/api/events/{evento.Id}",
                evento
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditEvent(
        int id,
        [FromForm] string name,
        [FromForm] string venue,
        [FromForm] int? plateaSeats,
        [FromForm] int? campoSeats,
        IFormFile? image)
            {
                var evento =
                    await _context.Events
                    .FirstOrDefaultAsync(e => e.Id == id);

                if(evento == null)
                {
                    return NotFound(
                        "El evento no existe");
                }

                if(string.IsNullOrWhiteSpace(name))
                {
                    return BadRequest(
                        "El nombre es obligatorio");
                }

                if(plateaSeats < 0 || campoSeats < 0)
                {
                    return BadRequest(
                        "La cantidad de butacas no puede ser negativa");
                }

                // UPDATE INFO
                evento.Name = name;
                evento.Venue = venue;

                // UPDATE IMAGEN
                if(image != null)
                {
                    var uploadsFolder =
                        Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot/uploads"
                        );
            
                    if(!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var fileName =
                        Guid.NewGuid().ToString() +
                        Path.GetExtension(image.FileName);

                    var path =
                        Path.Combine(
                            uploadsFolder,
                            fileName
                        );

                    using(var stream =
                        new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
            
                    evento.ImageUrl =
                        "/uploads/" + fileName;
                }            
            
                // PLATEA

                var platea =
                    await _context.Sectors
                    .FirstOrDefaultAsync(s =>
                        s.EventId == id &&
                        s.Name == "Platea");

                if(platea != null)
                {
                    var plateaSeatsActuales =
                        await _context.Seats
                        .Where(s => s.SectorId == platea.Id)
                        .ToListAsync();

                    int actuales =
                        plateaSeatsActuales.Count;

                    // AGREGAR
                    if(plateaSeats.HasValue &&
                        plateaSeats.Value > actuales)
                    {
                        for(int i = actuales + 1;
                            i <= plateaSeats.Value;
                            i++)
                        {
                            _context.Seats.Add(
                                new Seat
                                {
                                    EventId = id,
                                    SectorId = platea.Id,
                                    Number = i,
                                    Status = "Disponible"
                            }); 
                        }   
                    }
            
                    // ELIMINAR
                    else if(plateaSeats.HasValue &&
                        plateaSeats.Value < actuales)
                    {
                        var eliminar =
                            plateaSeatsActuales
                            .OrderByDescending(s => s.Number)
                            .Take(actuales - plateaSeats.Value);

                        _context.Seats.RemoveRange(eliminar);
                    }
                }

                    //CAMPO

                var campo =
                    await _context.Sectors
                    .FirstOrDefaultAsync(s =>
                        s.EventId == id &&
                        s.Name == "Campo");

                if(campo != null)
                {
                    var campoSeatsActuales =
                        await _context.Seats
                        .Where(s => s.SectorId == campo.Id)
                        .ToListAsync();

                    int actuales =
                        campoSeatsActuales.Count;

                    // AGREGAR
                    if(campoSeats.HasValue &&
                        campoSeats.Value > actuales)
                    {
                        for(int i = actuales + 1;
                            i <= campoSeats.Value;
                            i++)
                        {
                            _context.Seats.Add(
                                new Seat
                                {
                                    EventId = id,
                                    SectorId = campo.Id,
                                    Number = i,
                                    Status = "Disponible"
                                });
                        }
                    }

                    // ELIMINAR
                    else if(campoSeats.HasValue &&
                        campoSeats.Value < actuales)
                    {
                        var eliminar =
                            campoSeatsActuales
                            .OrderByDescending(s => s.Number)
                            .Take(actuales - campoSeats.Value);

                        _context.Seats.RemoveRange(eliminar);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(evento);
            }

        [HttpDelete("{id}")]
            public IActionResult DeleteEvent(int id)
            {
            var evento =
                _context.Events
                .FirstOrDefault(e => e.Id == id);

            if(evento == null)
                return NotFound();

            // ASIENTOS
            var seats =
                _context.Seats
                .Where(s => s.EventId == id);

                _context.Seats.RemoveRange(seats);

            // SECTORES
            var sectors =
                _context.Sectors
                .Where(s => s.EventId == id);

            _context.Sectors.RemoveRange(sectors);

            // EVENTO
            _context.Events.Remove(evento);

            _context.SaveChanges();

            return NoContent();
        }
    }
}