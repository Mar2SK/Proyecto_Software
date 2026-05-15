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
        public IActionResult Register(
            [FromBody] Usuario nuevo)
        {
            // VALIDAR USUARIO
            var usuarioExiste =
                _context.Usuarios.Any(u =>
                    u.Username == nuevo.Username);

            if(usuarioExiste)
            {
                return BadRequest(
                    "El usuario ya existe"
                );
            }

            // VALIDAR EMAIL
            var emailExiste =
                _context.Usuarios.Any(u =>
                    u.Email == nuevo.Email);

            if(emailExiste)
            {
                return BadRequest(
                    "El email ya está registrado"
                );
            }

            // ROL DEFAULT
            nuevo.Rol = "User";

            // GUARDAR USUARIO
            _context.Usuarios.Add(nuevo);

            _context.SaveChanges();

            return Ok(nuevo);
        }
    }
}