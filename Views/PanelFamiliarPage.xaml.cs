using HelloWorldMAUI.Services;

namespace HelloWorldMAUI.Views
{
    public partial class PanelFamiliarPage : ContentPage
    {
        private readonly ApiService _api;
        private readonly int _idUsuario;

        public PanelFamiliarPage(ApiService api, int idUsuario, string nombre)
        {
            InitializeComponent();
            _api = api;
            _idUsuario = idUsuario;
            LblTitulo.Text = $"Resumen de {nombre}";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarPanel();
        }

        private async Task CargarPanel()
        {
            var panel = await _api.GetPanelAsync(_idUsuario);
            if (panel != null)
            {
                LblPorcentaje.Text = $"{panel.Porcentaje}%";
                LblProgramados.Text = panel.TotalProgramados.ToString();
                LblConfirmados.Text = panel.TotalConfirmaciones.ToString();
                LblAlertas.Text = panel.TotalAlertas.ToString();

                LblPorcentaje.TextColor = panel.ColorIndicador switch
                {
                    "verde" => Color.FromArgb("#2E7D32"),
                    "ambar" => Color.FromArgb("#B8860B"),
                    _ => Color.FromArgb("#D32F2F")
                };
            }

            var medicamentos = await _api.GetMedicamentosAsync(_idUsuario);
            var items = medicamentos.Select(m => new
            {
                m.Nombre,
                DosisTexto = $"{m.Dosis} {m.Unidad} · {m.Frecuencia}",
                PrimeraHora = m.Horarios.FirstOrDefault()?.HoraAdministracion ?? ""
            }).ToList();

            ListaMedicamentos.ItemsSource = items;
        }

        private async void OnRefreshing(object sender, EventArgs e)
        {
            await CargarPanel();
            Refresh.IsRefreshing = false;
        }

        private void OnCerrarSesionClicked(object sender, EventArgs e)
        {
            Preferences.Remove("token");
            Preferences.Remove("idUsuario");
            Preferences.Remove("nombre");
            Preferences.Remove("rol");
            Application.Current!.MainPage = new NavigationPage(
                new LoginPage(_api));
        }
    }
}