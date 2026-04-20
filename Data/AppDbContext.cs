using Microsoft.EntityFrameworkCore;
using EntradasApi.Models;

namespace EntradasApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Seat> Seats { get; set; }
        
        public DbSet<Event> Events { get; set; }
        
        public DbSet<Sector> Sectors { get; set; }

        public DbSet<Auditoria> Auditorias { get; set; }
    }
}