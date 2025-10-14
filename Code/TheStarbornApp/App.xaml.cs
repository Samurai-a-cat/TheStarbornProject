using TheStarbornApp.GameCore.Services.GameSimulationService;

namespace TheStarbornApp;

public partial class App : Application
{
	private readonly IGameSimulation _game;
	public App(IGameSimulation game)
	{
		_game = game;
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage()) { Title = "TheStarbornApp" };
	}
	
	protected override void OnResume()
	{
		base.OnResume();
		_game.Update();
	}
}
