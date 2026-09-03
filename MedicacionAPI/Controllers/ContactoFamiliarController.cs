using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicacionAPI.Data;
using MedicacionAPI.Models;

namespace MedicacionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ContactoFamiliarController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactoFamiliarController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/contactofamiliar/{idUsuario}
        [HttpGet("{idUsuario}")]
        public async Task<IActionResult> GetPorUsuario(int idUsuario)
        {
            var contacto = await _context.ContactosFamiliares
                .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && c.Activo);

            if (contacto == null)
                return NotFound(new { mensaje = "Contacto familiar no encontrado." });

            return Ok(new
            {
                contacto.IdContacto,
                contacto.IdUsuario,
                contacto.NombreFamiliar,
                contacto.CorreoFamiliar,
                contacto.TelefonoFamiliar,
                contacto.Activo
            });
        }

        // POST: api/contactofamiliar
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] ContactoFamiliarDto dto)
        {
            var existe = await _context.ContactosFamiliares
                .AnyAsync(c => c.IdUsuario == dto.IdUsuario && c.Activo);

            if (existe)
                return BadRequest(new { mensaje = "El usuario ya tiene un contacto familiar registrado. Use PUT para actualizar." });

            var contacto = new ContactoFamiliar
            {
                IdUsuario = dto.IdUsuario,
                NombreFamiliar = dto.NombreFamiliar,
                CorreoFamiliar = dto.CorreoFamiliar,
                TelefonoFamiliar = dto.TelefonoFamiliar,
                Activo = true
            };

            _context.ContactosFamiliares.Add(contacto);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Contacto familiar registrado exitosamente.",
                idContacto = contacto.IdContacto
            });
        }

        // PUT: api/contactofamiliar/{idUsuario}
        [HttpPut("{idUsuario}")]
        public async Task<IActionResult> Actualizar(int idUsuario,
            [FromBody] ContactoFamiliarDto dto)
        {
            var contacto = await _context.ContactosFamiliares
                .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && c.Activo);

            if (contacto == null)
                return NotFound(new { mensaje = "Contacto familiar no encontrado." });

            contacto.NombreFamiliar = dto.NombreFamiliar;
            contacto.CorreoFamiliar = dto.CorreoFamiliar;
            contacto.TelefonoFamiliar = dto.TelefonoFamiliar;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Contacto familiar actualizado exitosamente." });
        }

        // DELETE: api/contactofamiliar/{idUsuario}
        [HttpDelete("{idUsuario}")]
        public async Task<IActionResult> Eliminar(int idUsuario)
        {
            var contacto = await _context.ContactosFamiliares
                .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && c.Activo);

            if (contacto == null)
                return NotFound(new { mensaje = "Contacto familiar no encontrado." });

            contacto.Activo = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Contacto familiar eliminado exitosamente." });
        }
    }

    // ── DTO ───────────────────────────────────────────────
    public class ContactoFamiliarDto
    {
        public int IdUsuario { get; set; }
        public string NombreFamiliar { get; set; } = string.Empty;
        public string CorreoFamiliar { get; set; } = string.Empty;
        public string? TelefonoFamiliar { get; set; }
    }
}