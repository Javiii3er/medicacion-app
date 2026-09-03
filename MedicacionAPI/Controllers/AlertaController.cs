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
    public class AlertaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlertaController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/alerta
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] AlertaDto dto)
        {
            var alerta = new Alerta
            {
                IdUsuario = dto.IdUsuario,
                IdMedicamento = dto.IdMedicamento,
                IdHorario = dto.IdHorario,
                HoraProgramada = TimeOnly.Parse(dto.HoraProgramada),
                HoraVencimiento = dto.HoraVencimiento,
                Estado = "pendiente",
                FechaCreacion = DateTime.Now
            };

            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Alerta registrada exitosamente.",
                idAlerta = alerta.IdAlerta
            });
        }

        // PUT: api/alerta/{id}/enviada
        [HttpPut("{id}/enviada")]
        public async Task<IActionResult> MarcarEnviada(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);

            if (alerta == null)
                return NotFound(new { mensaje = "Alerta no encontrada." });

            alerta.Estado = "enviada";
            alerta.HoraEnvio = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Alerta marcada como enviada.", horaEnvio = alerta.HoraEnvio });
        }

        // PUT: api/alerta/{id}/error
        [HttpPut("{id}/error")]
        public async Task<IActionResult> MarcarError(int id)
        {
            var alerta = await _context.Alertas.FindAsync(id);

            if (alerta == null)
                return NotFound(new { mensaje = "Alerta no encontrada." });

            alerta.Estado = "error";
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Alerta marcada con error." });
        }

        // GET: api/alerta/usuario/{idUsuario}
        [HttpGet("usuario/{idUsuario}")]
        public async Task<IActionResult> GetPorUsuario(int idUsuario,
            [FromQuery] DateTime? inicio, [FromQuery] DateTime? fin)
        {
            var fechaInicio = inicio ?? DateTime.Now.AddDays(-7);
            var fechaFin = fin ?? DateTime.Now;

            var alertas = await _context.Alertas
                .Include(a => a.Medicamento)
                .Where(a => a.IdUsuario == idUsuario
                         && a.FechaCreacion >= fechaInicio
                         && a.FechaCreacion <= fechaFin)
                .OrderByDescending(a => a.FechaCreacion)
                .Select(a => new
                {
                    a.IdAlerta,
                    a.IdMedicamento,
                    NombreMedicamento = a.Medicamento!.Nombre,
                    Dosis = $"{a.Medicamento.Dosis} {a.Medicamento.Unidad}",
                    HoraProgramada = a.HoraProgramada.ToString("HH:mm"),
                    a.HoraVencimiento,
                    a.Estado,
                    a.HoraEnvio,
                    a.FechaCreacion,
                    Tipo = "alerta"
                })
                .ToListAsync();

            return Ok(alertas);
        }

        // GET: api/alerta/pendientes/{idUsuario}
        [HttpGet("pendientes/{idUsuario}")]
        public async Task<IActionResult> GetPendientes(int idUsuario)
        {
            var alertas = await _context.Alertas
                .Include(a => a.Medicamento)
                .Where(a => a.IdUsuario == idUsuario && a.Estado == "pendiente")
                .OrderByDescending(a => a.FechaCreacion)
                .Select(a => new
                {
                    a.IdAlerta,
                    NombreMedicamento = a.Medicamento!.Nombre,
                    HoraProgramada = a.HoraProgramada.ToString("HH:mm"),
                    a.HoraVencimiento,
                    a.Estado
                })
                .ToListAsync();

            return Ok(alertas);
        }
    }

    // ── DTO ───────────────────────────────────────────────
    public class AlertaDto
    {
        public int IdUsuario { get; set; }
        public int IdMedicamento { get; set; }
        public int IdHorario { get; set; }
        public string HoraProgramada { get; set; } = string.Empty;
        public DateTime HoraVencimiento { get; set; }
    }
}