using Microsoft.AspNetCore.Mvc;
using EntradasApi.Data;
using EntradasApi.Models;

namespace EntradasApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario login)
        {
            var user = _context.Usuarios.FirstOrDefault(u =>
                u.Username == login.Username ||
                u.Email == login.Email);

            if (user == null || user.Password != login.Password)
                return Unauthorized("Credenciales incorrectas");

            return Ok(user);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Usuario nuevo)
        {
            nuevo.Rol = "User";

            _context.Usuarios.Add(nuevo);
            _context.SaveChanges();

            return Ok(nuevo);
        }
    }
}