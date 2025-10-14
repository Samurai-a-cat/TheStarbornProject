namespace TheStarbornApp.GameCore.Services.GameSimulationService;

public interface ITimeProcess
{
    bool IsCompleted { get; }
    void OnUpdate(double realDeltaTime, long currentGameTime);
}