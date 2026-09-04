using SendGrid;
using SendGrid.Helpers.Mail;
using MedicacionAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace MedicacionAPI.Services
{
    public class NotificacionService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<NotificacionService> _logger;

        public NotificacionService(IConfiguration config,
            ILogger<NotificacionService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> EnviarAlertaFamiliarAsync(
            int idUsuario, string nombreMedicamento,
            string dosis, string horaVencimiento,
            AppDbContext context)
        {
            try
            {
                // Obtener contacto familiar
                var contacto = await context.ContactosFamiliares
                    .FirstOrDefaultAsync(c => c.IdUsuario == idUsuario && c.Activo);

                if (contacto == null)
                {
                    _logger.LogWarning(
                        "No se encontró contacto familiar para usuario {Id}", idUsuario);
                    return false;
                }

                // Obtener nombre del adulto mayor
                var usuario = await context.Usuarios.FindAsync(idUsuario);
                var nombrePaciente = usuario != null
                    ? $"{usuario.Nombre} {usuario.Apellido}"
                    : "el adulto mayor";

                var apiKey = _config["SendGrid:ApiKey"];
                var fromEmail = _config["SendGrid:FromEmail"];
                var fromName = _config["SendGrid:FromName"];

                if (string.IsNullOrEmpty(apiKey) || apiKey == "TU_API_KEY_AQUI")
                {
                    _logger.LogWarning(
                        "SendGrid no configurado. Alerta pendiente para {Email}",
                        contacto.CorreoFamiliar);
                    return false;
                }

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(fromEmail, fromName);
                var to = new EmailAddress(contacto.CorreoFamiliar, contacto.NombreFamiliar);

                var subject = $"⚠️ Alerta de medicación — {nombrePaciente}";

                var htmlContent = $@"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <div style='background-color: #003A70; padding: 20px; text-align: center;'>
                            <h2 style='color: white; margin: 0;'>Sistema de Monitoreo de Medicación</h2>
                            <p style='color: #DCEFFD; margin: 5px 0 0;'>CAIMI Chiquimulilla</p>
                        </div>
                        <div style='background-color: #FDECEA; padding: 20px; border-left: 4px solid #D32F2F;'>
                            <h3 style='color: #D32F2F; margin-top: 0;'>⚠️ Toma no confirmada</h3>
                            <p><strong>{nombrePaciente}</strong> no confirmó la toma de su medicamento.</p>
                            <table style='width: 100%; border-collapse: collapse; margin-top: 16px;'>
                                <tr style='background-color: #fff;'>
                                    <td style='padding: 10px; border: 1px solid #E4E8ED; font-weight: bold;'>Medicamento</td>
                                    <td style='padding: 10px; border: 1px solid #E4E8ED;'>{nombreMedicamento}</td>
                                </tr>
                                <tr style='background-color: #F5F7FA;'>
                                    <td style='padding: 10px; border: 1px solid #E4E8ED; font-weight: bold;'>Dosis</td>
                                    <td style='padding: 10px; border: 1px solid #E4E8ED;'>{dosis}</td>
                                </tr>
                                <tr style='background-color: #fff;'>
                                    <td style='padding: 10px; border: 1px solid #E4E8ED; font-weight: bold;'>Hora de vencimiento</td>
                                    <td style='padding: 10px; border: 1px solid #E4E8ED;'>{horaVencimiento}</td>
                                </tr>
                            </table>
                            <p style='margin-top: 16px; color: #5B6570; font-size: 13px;'>
                                Por favor comuníquese con {nombrePaciente} para verificar su estado de salud.
                            </p>
                        </div>
                        <div style='background-color: #F5F7FA; padding: 16px; text-align: center;'>
                            <p style='color: #5B6570; font-size: 12px; margin: 0;'>
                                Este mensaje fue generado automáticamente por el Sistema de Monitoreo de Medicación — CAIMI Chiquimulilla.
                            </p>
                        </div>
                    </div>";

                var msg = MailHelper.CreateSingleEmail(
                    from, to, subject, "", htmlContent);

                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                    response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation(
                        "Alerta enviada a {Email} para medicamento {Med}",
                        contacto.CorreoFamiliar, nombreMedicamento);
                    return true;
                }
                else
                {
                    _logger.LogError(
                        "Error al enviar alerta. Status: {Status}",
                        response.StatusCode);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al enviar alerta por SendGrid.");
                return false;
            }
        }
    }
}