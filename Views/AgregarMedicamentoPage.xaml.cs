using HelloWorldMAUI.Services;

namespace HelloWorldMAUI.Views
{
    public partial class AgregarMedicamentoPage : ContentPage
    {
        private readonly ApiService _api;
        private readonly int _idUsuario;

        public AgregarMedicamentoPage(ApiService api, int idUsuario)
        {
            InitializeComponent();
            _api = api;
            _idUsuario = idUsuario;
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            var nombre = EntryNombre.Text?.Trim();
            var dosisText = EntryDosis.Text?.Trim();
            var unidad = EntryUnidad.Text?.Trim();
            var frecuencia = EntryFrecuencia.Text?.Trim();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(dosisText)
                || string.IsNullOrEmpty(unidad) || string.IsNullOrEmpty(frecuencia))
            {
                LblError.Text = "Por favor complete todos los campos obligatorios.";
                LblError.IsVisible = true;
                return;
            }

            if (!decimal.TryParse(dosisText, out var dosis))
            {
                LblError.Text = "La dosis debe ser un número válido.";
                LblError.IsVisible = true;
                return;
            }

            LblError.IsVisible = false;
            Loading.IsVisible = true;
            Loading.IsRunning = true;

            try
            {
                var dto = new
                {
                    idUsuario = _idUsuario,
                    nombre,
                    dosis,
                    unidad,
                    frecuencia,
                    notas = EditorNotas.Text?.Trim(),
                    horaAdministracion = TimerHora.Time.ToString(@"hh\:mm")
                };

                var resultado = await _api.CrearMedicamentoAsync(dto);

                if (resultado)
                {
                    await DisplayAlert("Éxito",
                        $"Medicamento '{nombre}' registrado correctamente.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    LblError.Text = "Error al registrar el medicamento. Intente de nuevo.";
                    LblError.IsVisible = true;
                }
            }
            catch (Exception)
            {
                LblError.Text = "Error de conexión. Verifique que el servidor esté activo.";
                LblError.IsVisible = true;
            }
            finally
            {
                Loading.IsVisible = false;
                Loading.IsRunning = false;
            }
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}