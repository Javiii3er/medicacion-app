using HelloWorldMAUI.Services;

namespace HelloWorldMAUI.Views
{
    public partial class PrincipalAdultoPage : ContentPage
    {
        private readonly ApiService _api;
        private readonly int _idUsuario;

        public PrincipalAdultoPage(ApiService api, int idUsuario, string nombre)
        {
            InitializeComponent();
            _api = api;
            _idUsuario = idUsuario;
            LblNombre.Text = nombre;
            LblSaludo.Text = DateTime.Now.Hour < 12 ? "Buenos días," :
                             DateTime.Now.Hour < 18 ? "Buenas tardes," : "Buenas noches,";
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarMedicamentos();
        }

        private async Task CargarMedicamentos()
        {
            var medicamentos = await _api.GetMedicamentosAsync(_idUsuario);
            var items = medicamentos.Select(m => new
            {
                m.Nombre,
                DosisTexto = $"{m.Dosis} {m.Unidad} · {m.Frecuencia}",
                PrimeraHora = m.Horarios.FirstOrDefault()?.HoraAdministracion ?? ""
            }).ToList();

            ListaMedicamentos.ItemsSource = items;

            if (items.Any())
            {
                LblProximoMed.Text = items.First().Nombre;
                LblProximaHora.Text = $"Hora: {items.First().PrimeraHora}";
            }
            else
            {
                LblProximoMed.Text = "Sin medicamentos registrados";
                LblProximaHora.Text = "";
            }
        }

        private async void OnRefreshing(object sender, EventArgs e)
        {
            await CargarMedicamentos();
            Refresh.IsRefreshing = false;
        }

        private async void OnVerHistorialClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Historial",
                "Función disponible próximamente.", "OK");
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