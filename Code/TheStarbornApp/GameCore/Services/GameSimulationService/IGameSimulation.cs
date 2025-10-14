using TheStarbornApp.GameCore.Services.TimeService.GameCalendar;

namespace TheStarbornApp.GameCore.Services.GameSimulationService;

public interface IGameSimulation
{
    GameCalendar Calendar { get; }
    void Update();
    void RegisterProcess(ITimeProcess process);
}