using HelloWorldMAUI.Services;

namespace HelloWorldMAUI.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly ApiService _api;

        public LoginPage(ApiService api)
        {
            InitializeComponent();
            _api = api;
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var correo = EntryCorreo.Text?.Trim();
            var contrasena = EntryContrasena.Text;

            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena))
            {
                LblError.Text = "Por favor ingrese su correo y contraseña.";
                LblError.IsVisible = true;
                return;
            }

            LblError.IsVisible = false;
            Loading.IsVisible = true;
            Loading.IsRunning = true;

            try
            {
                var resultado = await _api.LoginAsync(correo, contrasena);

                if (resultado == null)
                {
                    LblError.Text = "Credenciales incorrectas. Intente de nuevo.";
                    LblError.IsVisible = true;
                    return;
                }

                _api.SetToken(resultado.Token);

                // Guardar sesión
                Preferences.Set("token", resultado.Token);
                Preferences.Set("idUsuario", resultado.IdUsuario);
                Preferences.Set("nombre", resultado.Nombre);
                Preferences.Set("rol", resultado.Rol);

                // Navegar según rol
                if (resultado.Rol == "AdultoMayor")
                    Application.Current!.MainPage = new NavigationPage(
                        new PrincipalAdultoPage(_api, resultado.IdUsuario, resultado.Nombre));
                else
                    Application.Current!.MainPage = new NavigationPage(
                        new PanelFamiliarPage(_api, resultado.IdUsuario, resultado.Nombre));
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
    }
}