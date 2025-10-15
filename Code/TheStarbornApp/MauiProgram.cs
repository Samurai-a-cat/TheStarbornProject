using System.Reflection;
using Microsoft.Extensions.Logging;
using TheStarbornApp.GameCore.Services.GameSimulationService;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.Asteroids;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Objects.StellarObjects.DustClouds;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;
using TheStarbornApp.GameCore.Services;

namespace TheStarbornApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Регистрируем сервисы для загрузки и работы с шаблонами
        builder.Services.AddTemplateData();

        // Обновлённая регистрация TemplateRegistry - теперь принимает IEnumerable<ITemplateData>
        builder.Services.AddSingleton<ITemplateRegistry>(sp =>
        {
            var templateData = sp.GetRequiredService<IEnumerable<AsteroidTemplateData>>();
            var logger = sp.GetRequiredService<ILogger<TemplateRegistry>>();
            return new TemplateRegistry(templateData, logger);
        });

        builder.Services.AddSingleton<IAsteroidFactory, AsteroidFactory>();
        builder.Services.AddSingleton<IDustCloudFactory, DustCloudFactory>();

        builder.Services.AddSingleton<ISectorContextGenerator, DefaultSectorContextGenerator>();
        builder.Services.AddSingleton<ISectorPopulator, SectorPopulator>();

        builder.Services.AddSingleton<IGameSimulation, GameSimulationService>();
        return builder.Build();
    }
}