using TheStarbornApp.GameCore.Services.TimeService.GameCalendar;

namespace TheStarbornApp.GameCore.Services.GameSimulationService;

public sealed class GameSimulationService : IGameSimulation
{
    private readonly GameCalendar _calendar = new();
    private readonly List<ITimeProcess> _activeProcesses = new();
    private DateTime? _lastRealTime;

    public GameCalendar Calendar => _calendar;

    public GameSimulationService()
    {
        // Устанавливаем якорь один раз при создании
        _calendar.SetRealTimeAnchor(DateTime.UtcNow);
        _lastRealTime = DateTime.UtcNow;
    }

    /// <summary>
    /// Основной метод симуляции. Вызывать при:
    /// входе в игру,
    /// возврате из фона,
    /// любом действии игрока.
    /// </summary>
    public void Update()
    {
        var now = DateTime.UtcNow;
        if (_lastRealTime == null)
        {
            _lastRealTime = now;
            return;
        }

        // Синхронизируем игровое время с реальным (включая офлайн-периоды)
        _calendar.SyncFromRealTime(now);

        // Вычисляем реальный дельта-тайм для процессов (опционально, но полезно)
        double realDeltaTime = (now - _lastRealTime.Value).TotalSeconds;
        _lastRealTime = now;

        // Обновляем все активные процессы
        for (int i = _activeProcesses.Count - 1; i >= 0; i--)
        {
            var process = _activeProcesses[i];
            if (!process.IsCompleted)
            {
                process.OnUpdate(realDeltaTime, _calendar.TotalSeconds);
            }

            if (process.IsCompleted)
            {
                _activeProcesses.RemoveAt(i);
            }
        }
    }

    public void RegisterProcess(ITimeProcess process)
    {
        if (!process.IsCompleted)
            _activeProcesses.Add(process);
    }
}