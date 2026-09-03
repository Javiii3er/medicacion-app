using MedicacionAPI.Data;
using MedicacionAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MedicacionAPI.Services
{
    public class VerificacionService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<VerificacionService> _logger;
        private readonly TimeSpan _intervalo = TimeSpan.FromMinutes(1);

        public VerificacionService(IServiceProvider services,
            ILogger<VerificacionService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("VerificacionService iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                await VerificarConfirmacionesPendientes();
                await Task.Delay(_intervalo, stoppingToken);
            }
        }

        private async Task VerificarConfirmacionesPendientes()
        {
            using var scope = _services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            try
            {
                var ahora = DateTime.Now;
                var hoy = DateOnly.FromDateTime(ahora);

                // Obtener todos los horarios activos
                var horarios = await context.Horarios
                    .Include(h => h.Medicamento)
                    .Where(h => h.Activo && h.Medicamento!.Activo)
                    .ToListAsync();

                foreach (var horario in horarios)
                {
                    // Calcular la hora de vencimiento (hora programada + 30 min)
                    var horaVencimiento = ahora.Date
                        .Add(horario.HoraAdministracion.ToTimeSpan())
                        .AddMinutes(30);

                    // Solo verificar si ya venció el período de tolerancia
                    if (ahora < horaVencimiento) continue;

                    // Verificar si ya existe una alerta para este horario hoy
                    var alertaExiste = await context.Alertas
                        .AnyAsync(a => a.IdHorario == horario.IdHorario
                                    && DateOnly.FromDateTime(a.FechaCreacion) == hoy);

                    if (alertaExiste) continue;

                    // Verificar si existe confirmación para este horario hoy
                    var confirmacionExiste = await context.Confirmaciones
                        .AnyAsync(c => c.IdHorario == horario.IdHorario
                                    && c.FechaConfirmacion == hoy);

                    if (confirmacionExiste) continue;

                    // Crear registro de alerta
                    var alerta = new Alerta
                    {
                        IdUsuario = horario.Medicamento!.IdUsuario,
                        IdMedicamento = horario.IdMedicamento,
                        IdHorario = horario.IdHorario,
                        HoraProgramada = horario.HoraAdministracion,
                        HoraVencimiento = horaVencimiento,
                        Estado = "pendiente",
                        FechaCreacion = ahora
                    };

                    context.Alertas.Add(alerta);
                    await context.SaveChangesAsync();

                    _logger.LogWarning(
                        "Alerta generada: Medicamento {Med}, Horario {Hora}, Usuario {User}",
                        horario.Medicamento.Nombre,
                        horario.HoraAdministracion,
                        horario.Medicamento.IdUsuario);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en VerificacionService.");
            }
        }
    }
}