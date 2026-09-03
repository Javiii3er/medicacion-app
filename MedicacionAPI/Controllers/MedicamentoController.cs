using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicacionAPI.Data;
using MedicacionAPI.Models;
using System.Security.Claims;

namespace MedicacionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicamentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedicamentoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/medicamento/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> GetPorUsuario(int idUsuario)
        {
            var medicamentos = await _context.Medicamentos
                .Include(m => m.Horarios)
                .Where(m => m.IdUsuario == idUsuario && m.Activo)
                .Select(m => new
                {
                    m.IdMedicamento,
                    m.Nombre,
                    m.Dosis,
                    m.Unidad,
                    m.Frecuencia,
                    m.Notas,
                    Horarios = m.Horarios
                        .Where(h => h.Activo)
                        .Select(h => new
                        {
                            h.IdHorario,
                            HoraAdministracion = h.HoraAdministracion.ToString("HH:mm")
                        }).ToList()
                })
                .ToListAsync();

            return Ok(medicamentos);
        }

        // GET: api/medicamento/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPorId(int id)
        {
            var medicamento = await _context.Medicamentos
                .Include(m => m.Horarios)
                .Where(m => m.IdMedicamento == id && m.Activo)
                .Select(m => new
                {
                    m.IdMedicamento,
                    m.IdUsuario,
                    m.Nombre,
                    m.Dosis,
                    m.Unidad,
                    m.Frecuencia,
                    m.Notas,
                    Horarios = m.Horarios
                        .Where(h => h.Activo)
                        .Select(h => new
                        {
                            h.IdHorario,
                            HoraAdministracion = h.HoraAdministracion.ToString("HH:mm")
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (medicamento == null)
                return NotFound(new { mensaje = "Medicamento no encontrado." });

            return Ok(medicamento);
        }

        // POST: api/medicamento
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] MedicamentoDto dto)
        {
            // Verificar duplicado
            var existe = await _context.Medicamentos
                .AnyAsync(m => m.IdUsuario == dto.IdUsuario
                            && m.Nombre == dto.Nombre
                            && m.Activo);

            if (existe)
                return BadRequest(new { mensaje = "Ya existe un medicamento con ese nombre para este usuario." });

            var medicamento = new Medicamento
            {
                IdUsuario = dto.IdUsuario,
                Nombre = dto.Nombre,
                Dosis = dto.Dosis,
                Unidad = dto.Unidad,
                Frecuencia = dto.Frecuencia,
                Notas = dto.Notas,
                Activo = true,
                FechaCreacion = DateTime.Now
            };

            _context.Medicamentos.Add(medicamento);
            await _context.SaveChangesAsync();

            // Registrar horario si viene en el DTO
            if (!string.IsNullOrEmpty(dto.HoraAdministracion))
            {
                if (TimeOnly.TryParse(dto.HoraAdministracion, out var hora))
                {
                    var horario = new Horario
                    {
                        IdMedicamento = medicamento.IdMedicamento,
                        HoraAdministracion = hora,
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    };
                    _context.Horarios.Add(horario);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new
            {
                mensaje = "Medicamento registrado exitosamente.",
                idMedicamento = medicamento.IdMedicamento
            });
        }

        // PUT: api/medicamento/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] MedicamentoDto dto)
        {
            var medicamento = await _context.Medicamentos
                .Include(m => m.Horarios)
                .FirstOrDefaultAsync(m => m.IdMedicamento == id && m.Activo);

            if (medicamento == null)
                return NotFound(new { mensaje = "Medicamento no encontrado." });

            medicamento.Nombre = dto.Nombre;
            medicamento.Dosis = dto.Dosis;
            medicamento.Unidad = dto.Unidad;
            medicamento.Frecuencia = dto.Frecuencia;
            medicamento.Notas = dto.Notas;

            // Actualizar horario si cambió
            if (!string.IsNullOrEmpty(dto.HoraAdministracion))
            {
                if (TimeOnly.TryParse(dto.HoraAdministracion, out var nuevaHora))
                {
                    var horarioExistente = medicamento.Horarios
                        .FirstOrDefault(h => h.Activo);

                    if (horarioExistente != null)
                    {
                        horarioExistente.HoraAdministracion = nuevaHora;
                    }
                    else
                    {
                        _context.Horarios.Add(new Horario
                        {
                            IdMedicamento = id,
                            HoraAdministracion = nuevaHora,
                            Activo = true,
                            FechaCreacion = DateTime.Now
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Medicamento actualizado exitosamente." });
        }

        // DELETE: api/medicamento/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var medicamento = await _context.Medicamentos
                .Include(m => m.Horarios)
                .FirstOrDefaultAsync(m => m.IdMedicamento == id && m.Activo);

            if (medicamento == null)
                return NotFound(new { mensaje = "Medicamento no encontrado." });

            // Soft delete — no elimina físicamente
            medicamento.Activo = false;
            foreach (var horario in medicamento.Horarios)
                horario.Activo = false;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Medicamento eliminado exitosamente." });
        }
    }

    // ── DTO ───────────────────────────────────────────────
    public class MedicamentoDto
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Dosis { get; set; }
        public string Unidad { get; set; } = string.Empty;
        public string Frecuencia { get; set; } = string.Empty;
        public string? Notas { get; set; }
        public string? HoraAdministracion { get; set; }
    }
}