using HelloWorldMAUI.Services;

namespace HelloWorldMAUI.Views
{
    public partial class RegistroPage : ContentPage
    {
        private readonly ApiService _api;

        public RegistroPage(ApiService api)
        {
            InitializeComponent();
            _api = api;
        }

        private async void OnRegistrarClicked(object sender, EventArgs e)
        {
            var nombre = EntryNombre.Text?.Trim();
            var apellido = EntryApellido.Text?.Trim();
            var correo = EntryCorreo.Text?.Trim();
            var contrasena = EntryContrasena.Text;
            var rol = PickerRol.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) ||
                string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(contrasena) ||
                string.IsNullOrEmpty(rol))
            {
                LblError.Text = "Por favor complete todos los campos.";
                LblError.IsVisible = true;
                return;
            }

            if (contrasena.Length < 8)
            {
                LblError.Text = "La contraseña debe tener mínimo 8 caracteres.";
                LblError.IsVisible = true;
                return;
            }

            LblError.IsVisible = false;
            Loading.IsVisible = true;
            Loading.IsRunning = true;

            try
            {
                var exito = await _api.RegistrarAsync(nombre, apellido, correo, contrasena, rol);

                if (exito)
                {
                    await DisplayAlert("Cuenta creada",
                        $"Bienvenido {nombre}. Su cuenta ha sido creada exitosamente.", "OK");

                    Application.Current!.MainPage = new NavigationPage(
                        new LoginPage(_api));
                }
                else
                {
                    LblError.Text = "El correo ya está registrado o ocurrió un error.";
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

        private void OnVolverLoginTapped(object sender, TappedEventArgs e)
        {
            Application.Current!.MainPage = new NavigationPage(
                new LoginPage(_api));
        }
    }
}