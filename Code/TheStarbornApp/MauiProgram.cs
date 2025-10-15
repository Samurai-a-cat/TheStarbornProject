using System.Reflection;
using Microsoft.Extensions.Logging;
using TheStarbornApp.GameCore.Services.GameSimulationService;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes.Sector;

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
		builder.Services.AddSingleton<PrototypeRegistry>(sp => 
			new PrototypeRegistry(Assembly.GetExecutingAssembly()));
		builder.Services.AddSingleton<ISectorContextGenerator, DefaultSectorContextGenerator>();
		builder.Services.AddSingleton<ISectorPopulator, SectorPopulator>();
		builder.Services.AddSingleton<IGameSimulation, GameSimulationService>();
		return builder.Build();
	}
}
