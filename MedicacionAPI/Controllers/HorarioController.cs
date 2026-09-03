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
    public class HorarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public HorarioController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/horario/medicamento/{idMedicamento}
        [HttpGet("medicamento/{idMedicamento}")]
        public async Task<IActionResult> GetPorMedicamento(int idMedicamento)
        {
            var horarios = await _context.Horarios
                .Where(h => h.IdMedicamento == idMedicamento && h.Activo)
                .Select(h => new
                {
                    h.IdHorario,
                    h.IdMedicamento,
                    HoraAdministracion = h.HoraAdministracion.ToString("HH:mm"),
                    h.Activo
                })
                .ToListAsync();

            return Ok(horarios);
        }

        // GET: api/horario/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> GetPorUsuario(int idUsuario)
        {
            var horarios = await _context.Horarios
                .Include(h => h.Medicamento)
                .Where(h => h.Medicamento!.IdUsuario == idUsuario
                         && h.Activo
                         && h.Medicamento.Activo)
                .OrderBy(h => h.HoraAdministracion)
                .Select(h => new
                {
                    h.IdHorario,
                    h.IdMedicamento,
                    NombreMedicamento = h.Medicamento!.Nombre,
                    Dosis = $"{h.Medicamento.Dosis} {h.Medicamento.Unidad}",
                    HoraAdministracion = h.HoraAdministracion.ToString("HH:mm"),
                    h.Activo
                })
                .ToListAsync();

            return Ok(horarios);
        }

        // POST: api/horario
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] HorarioDto dto)
        {
            var medicamento = await _context.Medicamentos
                .FirstOrDefaultAsync(m => m.IdMedicamento == dto.IdMedicamento && m.Activo);

            if (medicamento == null)
                return NotFound(new { mensaje = "Medicamento no encontrado." });

            if (!TimeOnly.TryParse(dto.HoraAdministracion, out var hora))
                return BadRequest(new { mensaje = "Formato de hora inválido. Use HH:mm" });

            var horario = new Horario
            {
                IdMedicamento = dto.IdMedicamento,
                HoraAdministracion = hora,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _context.Horarios.Add(horario);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Horario registrado exitosamente.",
                idHorario = horario.IdHorario,
                horaAdministracion = horario.HoraAdministracion.ToString("HH:mm")
            });
        }

        // PUT: api/horario/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] HorarioDto dto)
        {
            var horario = await _context.Horarios
                .FirstOrDefaultAsync(h => h.IdHorario == id && h.Activo);

            if (horario == null)
                return NotFound(new { mensaje = "Horario no encontrado." });

            if (!TimeOnly.TryParse(dto.HoraAdministracion, out var hora))
                return BadRequest(new { mensaje = "Formato de hora inválido. Use HH:mm" });

            horario.HoraAdministracion = hora;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Horario actualizado exitosamente.",
                horaAdministracion = horario.HoraAdministracion.ToString("HH:mm")
            });
        }

        // DELETE: api/horario/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var horario = await _context.Horarios
                .FirstOrDefaultAsync(h => h.IdHorario == id && h.Activo);

            if (horario == null)
                return NotFound(new { mensaje = "Horario no encontrado." });

            horario.Activo = false;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Horario eliminado exitosamente." });
        }
    }

    // ── DTO ───────────────────────────────────────────────
    public class HorarioDto
    {
        public int IdMedicamento { get; set; }
        public string HoraAdministracion { get; set; } = string.Empty;
    }
}