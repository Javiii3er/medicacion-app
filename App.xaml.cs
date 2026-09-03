using HelloWorldMAUI.Services;
using HelloWorldMAUI.Views;

namespace HelloWorldMAUI
{
    public partial class App : Application
    {
        public App(ApiService api)
        {
            InitializeComponent();
            MainPage = new NavigationPage(new LoginPage(api));
        }
    }
}