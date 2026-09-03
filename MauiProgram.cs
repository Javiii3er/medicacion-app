using CommunityToolkit.Maui;
using HelloWorldMAUI.Services;
using HelloWorldMAUI.Views;
using Microsoft.Extensions.Logging;

namespace HelloWorldMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // HttpClient para el API
            builder.Services.AddHttpClient("MedicacionAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7239/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            // Registrar ApiService
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<PrincipalAdultoPage>();
            builder.Services.AddTransient<PanelFamiliarPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}