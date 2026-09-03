using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HelloWorldMAUI.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;
        private static string _token = string.Empty;

        public ApiService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("MedicacionAPI");
        }

        public void SetToken(string token)
        {
            _token = token;
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public static string Token => _token;

        // ── Autenticación ─────────────────────────────────
        public async Task<LoginResponse?> LoginAsync(string correo, string contrasena)
        {
            var body = JsonSerializer.Serialize(new { correo, contrasena });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("api/Usuario/login", content);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LoginResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ── Medicamentos ──────────────────────────────────
        public async Task<List<MedicamentoResponse>> GetMedicamentosAsync(int idUsuario)
        {
            var response = await _http.GetAsync($"api/Medicamento/usuario/{idUsuario}");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MedicamentoResponse>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        public async Task<bool> CrearMedicamentoAsync(object dto)
        {
            var body = JsonSerializer.Serialize(dto);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("api/Medicamento", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> EliminarMedicamentoAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Medicamento/{id}");
            return response.IsSuccessStatusCode;
        }

        // ── Horarios ──────────────────────────────────────
        public async Task<List<HorarioResponse>> GetHorariosUsuarioAsync(int idUsuario)
        {
            var response = await _http.GetAsync($"api/Horario/usuario/{idUsuario}");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<HorarioResponse>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        // ── Confirmaciones ────────────────────────────────
        public async Task<bool> ConfirmarTomAsync(int idUsuario, int idMedicamento, int idHorario)
        {
            var body = JsonSerializer.Serialize(new { idUsuario, idMedicamento, idHorario });
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("api/Confirmacion", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<HistorialItem>> GetHistorialAsync(int idUsuario)
        {
            var response = await _http.GetAsync($"api/Confirmacion/historial/{idUsuario}");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<HistorialItem>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        public async Task<PanelResponse?> GetPanelAsync(int idUsuario)
        {
            var response = await _http.GetAsync($"api/Confirmacion/panel/{idUsuario}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PanelResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // ── Contacto familiar ─────────────────────────────
        public async Task<ContactoResponse?> GetContactoFamiliarAsync(int idUsuario)
        {
            var response = await _http.GetAsync($"api/ContactoFamiliar/{idUsuario}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ContactoResponse>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

    // ── Modelos de respuesta ──────────────────────────────
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    public class MedicamentoResponse
    {
        public int IdMedicamento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Dosis { get; set; }
        public string Unidad { get; set; } = string.Empty;
        public string Frecuencia { get; set; } = string.Empty;
        public string? Notas { get; set; }
        public List<HorarioResponse> Horarios { get; set; } = new();
    }

    public class HorarioResponse
    {
        public int IdHorario { get; set; }
        public int IdMedicamento { get; set; }
        public string NombreMedicamento { get; set; } = string.Empty;
        public string Dosis { get; set; } = string.Empty;
        public string HoraAdministracion { get; set; } = string.Empty;
    }

    public class HistorialItem
    {
        public int IdConfirmacion { get; set; }
        public string NombreMedicamento { get; set; } = string.Empty;
        public string Dosis { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string Hora { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
    }

    public class PanelResponse
    {
        public int TotalProgramados { get; set; }
        public int TotalConfirmaciones { get; set; }
        public int TotalAlertas { get; set; }
        public double Porcentaje { get; set; }
        public string ColorIndicador { get; set; } = string.Empty;
        public List<object> UltimasActividades { get; set; } = new();
    }

    public class ContactoResponse
    {
        public int IdContacto { get; set; }
        public string NombreFamiliar { get; set; } = string.Empty;
        public string CorreoFamiliar { get; set; } = string.Empty;
        public string? TelefonoFamiliar { get; set; }
    }
}