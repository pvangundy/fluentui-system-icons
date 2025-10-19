using CommunityToolkit.Maui;
using FieldLead.Mobile.Data;
using FieldLead.Mobile.Pages;
using FieldLead.Mobile.Services;
using FieldLead.Mobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Networking;
using Microsoft.Maui.Storage;

namespace FieldLead.Mobile;

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
            })
            .UseMauiMaps();

        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<MapPage>();
        builder.Services.AddSingleton<MapViewModel>();
        builder.Services.AddTransient<SiteDetailPage>();
        builder.Services.AddTransient<SiteDetailViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();

        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<PhotoService>();
        builder.Services.AddSingleton<SyncService>();
        builder.Services.AddSingleton<LeadRepository>();

        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
        builder.Services.AddSingleton<IFileSystem>(FileSystem.Current);

        builder.Services.AddHttpClient("api", client =>
        {
            client.BaseAddress = new Uri("https://api.example.com/");
        });

        builder.Services.AddHostedService<SyncBackgroundService>();

        return builder.Build();
    }
}
