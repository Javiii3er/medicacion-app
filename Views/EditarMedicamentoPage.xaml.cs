using HelloWorldMAUI.Services;

namespace HelloWorldMAUI.Views
{
    public partial class EditarMedicamentoPage : ContentPage
    {
        private readonly ApiService _api;
        private readonly int _idUsuario;
        private readonly MedicamentoResponse _medicamento;

        public EditarMedicamentoPage(ApiService api, int idUsuario,
            MedicamentoResponse medicamento)
        {
            InitializeComponent();
            _api = api;
            _idUsuario = idUsuario;
            _medicamento = medicamento;
            CargarDatos();
        }

        private void CargarDatos()
        {
            EntryNombre.Text = _medicamento.Nombre;
            EntryDosis.Text = _medicamento.Dosis.ToString();
            EntryUnidad.Text = _medicamento.Unidad;
            EntryFrecuencia.Text = _medicamento.Frecuencia;
            EditorNotas.Text = _medicamento.Notas;

            if (_medicamento.Horarios.Any() &&
                TimeOnly.TryParse(_medicamento.Horarios.First().HoraAdministracion, out var hora))
            {
                TimerHora.Time = hora.ToTimeSpan();
            }
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

                var resultado = await _api.ActualizarMedicamentoAsync(
                    _medicamento.IdMedicamento, dto);

                if (resultado)
                {
                    await DisplayAlert("Éxito",
                        "Medicamento actualizado correctamente.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    LblError.Text = "Error al actualizar. Intente de nuevo.";
                    LblError.IsVisible = true;
                }
            }
            catch
            {
                LblError.Text = "Error de conexión.";
                LblError.IsVisible = true;
            }
            finally
            {
                Loading.IsVisible = false;
                Loading.IsRunning = false;
            }
        }

        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            bool confirmar = await DisplayAlert(
                "Eliminar medicamento",
                $"¿Está seguro que desea eliminar {_medicamento.Nombre}?",
                "Sí, eliminar", "Cancelar");

            if (!confirmar) return;

            Loading.IsVisible = true;
            Loading.IsRunning = true;

            try
            {
                var resultado = await _api.EliminarMedicamentoAsync(
                    _medicamento.IdMedicamento);

                if (resultado)
                {
                    await DisplayAlert("Eliminado",
                        "Medicamento eliminado correctamente.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    LblError.Text = "Error al eliminar. Intente de nuevo.";
                    LblError.IsVisible = true;
                }
            }
            catch
            {
                LblError.Text = "Error de conexión.";
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