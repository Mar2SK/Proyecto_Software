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
        public DbSet<Usuario> Usuarios { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().HasData(

                new Usuario {
                    Id = 1,
                    Nombre = "Admin",
                    Username = "admin",
                    Email = "admin@admin.com",
                    Password = "1212",
                    Rol = "Admin"
                },

                new Usuario {
                    Id = 2,
                    Nombre = "Martin",
                    Username = "martin",
                    Email = "martin@mail.com",
                    Password = "123",
                    Rol = "User"
                },

                new Usuario {
                    Id = 3,
                    Nombre = "Joaquin",
                    Username = "joaquin",
                    Email = "joaquin@mail.com",
                    Password = "123",
                    Rol = "User"
                },

                new Usuario {
                    Id = 4,
                    Nombre = "Ivanna",
                    Username = "ivanna",
                    Email = "ivanna@mail.com",
                    Password = "123",
                    Rol = "User"
                }
            );
        }
    }
}