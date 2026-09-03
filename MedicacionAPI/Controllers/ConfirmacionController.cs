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
    public class ConfirmacionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConfirmacionController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/confirmacion
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] ConfirmacionDto dto)
        {
            var horario = await _context.Horarios
                .FirstOrDefaultAsync(h => h.IdHorario == dto.IdHorario && h.Activo);

            if (horario == null)
                return NotFound(new { mensaje = "Horario no encontrado." });

            var ahora = DateTime.Now;

            var confirmacion = new Confirmacion
            {
                IdUsuario = dto.IdUsuario,
                IdMedicamento = dto.IdMedicamento,
                IdHorario = dto.IdHorario,
                FechaConfirmacion = DateOnly.FromDateTime(ahora),
                HoraConfirmacion = TimeOnly.FromDateTime(ahora),
                TimestampExacto = ahora
            };

            _context.Confirmaciones.Add(confirmacion);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Confirmación registrada exitosamente.",
                idConfirmacion = confirmacion.IdConfirmacion,
                timestamp = confirmacion.TimestampExacto
            });
        }

        // GET: api/confirmacion/historial/{idUsuario}
        [HttpGet("historial/{idUsuario}")]
        public async Task<IActionResult> Historial(int idUsuario,
            [FromQuery] DateTime? inicio, [FromQuery] DateTime? fin)
        {
            var fechaInicio = inicio ?? DateTime.Now.AddDays(-7);
            var fechaFin = fin ?? DateTime.Now;

            var fechaInicioOnly = DateOnly.FromDateTime(fechaInicio);
            var fechaFinOnly = DateOnly.FromDateTime(fechaFin);

            var confirmaciones = await _context.Confirmaciones
                .Include(c => c.Medicamento)
                .Where(c => c.IdUsuario == idUsuario
                         && c.FechaConfirmacion >= fechaInicioOnly
                         && c.FechaConfirmacion <= fechaFinOnly)
                .OrderByDescending(c => c.TimestampExacto)
                .Select(c => new
                {
                    c.IdConfirmacion,
                    c.IdMedicamento,
                    NombreMedicamento = c.Medicamento!.Nombre,
                    Dosis = $"{c.Medicamento.Dosis} {c.Medicamento.Unidad}",
                    Fecha = c.FechaConfirmacion.ToString("yyyy-MM-dd"),
                    Hora = c.HoraConfirmacion.ToString("HH:mm"),
                    Timestamp = c.TimestampExacto,
                    Estado = "confirmado"
                })
                .ToListAsync();

            return Ok(confirmaciones);
        }

        // GET: api/confirmacion/verificar/{idHorario}
        [HttpGet("verificar/{idHorario}")]
        public async Task<IActionResult> Verificar(int idHorario)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            var existe = await _context.Confirmaciones
                .AnyAsync(c => c.IdHorario == idHorario
                            && c.FechaConfirmacion == hoy);

            return Ok(new
            {
                confirmado = existe,
                fecha = hoy.ToString("yyyy-MM-dd")
            });
        }

        // GET: api/confirmacion/panel/{idUsuario}
        [HttpGet("panel/{idUsuario}")]
        public async Task<IActionResult> Panel(int idUsuario,
            [FromQuery] int mes = 0, [FromQuery] int anio = 0)
        {
            if (mes == 0) mes = DateTime.Now.Month;
            if (anio == 0) anio = DateTime.Now.Year;

            var fechaInicio = DateOnly.FromDateTime(new DateTime(anio, mes, 1));
            var fechaFin = DateOnly.FromDateTime(new DateTime(anio, mes,
                DateTime.DaysInMonth(anio, mes)));

            var totalProgramados = await _context.Horarios
                .CountAsync(h => h.Medicamento!.IdUsuario == idUsuario && h.Activo);

            var totalConfirmaciones = await _context.Confirmaciones
                .CountAsync(c => c.IdUsuario == idUsuario
                              && c.FechaConfirmacion >= fechaInicio
                              && c.FechaConfirmacion <= fechaFin);

            var totalAlertas = await _context.Alertas
                .CountAsync(a => a.IdUsuario == idUsuario
                              && DateOnly.FromDateTime(a.FechaCreacion) >= fechaInicio
                              && DateOnly.FromDateTime(a.FechaCreacion) <= fechaFin);

            var porcentaje = totalProgramados > 0
                ? Math.Round((double)totalConfirmaciones / (totalProgramados * 30) * 100, 1)
                : 0;

            var colorIndicador = porcentaje >= 80 ? "verde"
                : porcentaje >= 50 ? "ambar"
                : "rojo";

            var ultimasActividades = await _context.Confirmaciones
                .Include(c => c.Medicamento)
                .Where(c => c.IdUsuario == idUsuario)
                .OrderByDescending(c => c.TimestampExacto)
                .Take(5)
                .Select(c => new
                {
                    tipo = "confirmacion",
                    NombreMedicamento = c.Medicamento!.Nombre,
                    Fecha = c.FechaConfirmacion.ToString("yyyy-MM-dd"),
                    Hora = c.HoraConfirmacion.ToString("HH:mm"),
                    estado = "confirmado"
                })
                .ToListAsync();

            return Ok(new
            {
                mes,
                anio,
                totalProgramados,
                totalConfirmaciones,
                totalAlertas,
                porcentaje,
                colorIndicador,
                ultimasActividades
            });
        }
    }

    // ── DTO ───────────────────────────────────────────────
    public class ConfirmacionDto
    {
        public int IdUsuario { get; set; }
        public int IdMedicamento { get; set; }
        public int IdHorario { get; set; }
    }
}