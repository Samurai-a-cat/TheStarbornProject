using System.Runtime.CompilerServices;

namespace TheStarbornApp.GameCore.Services.TimeService.PreciseTimer;

/// <summary>
/// Высокоточный, легковесный таймер для коротких интервалов.
/// Immutable по духу: все мутации возвращают новый экземпляр.
/// </summary>
public readonly struct PreciseTimer
{
    private readonly long _startTicks;
    private readonly double _timeScale;
    private readonly bool _isPaused;
    private readonly long _pauseTicks;

// --- Свойства ---

    /// <summary>
    /// Текущий коэффициент скорости времени (должен быть > 0).
    /// </summary>
    public double TimeScale => _timeScale;

    /// <summary>
    /// Находится ли таймер на паузе.
    /// </summary>
    public bool IsPaused => _isPaused;

// --- Конструкторы и фабрики ---

    /// <summary>
    /// Создаёт таймер, запущенный в текущий момент.
    /// </summary>
    /// <exception cref="ArgumentException">Если timeScale больше или равен 0</exception>
    public static PreciseTimer StartNew(double timeScale = 1.0)
    {
        if (timeScale <= 0)
            throw new ArgumentException("Time scale must be greater than zero.", nameof(timeScale));

        return new PreciseTimer(HighResolutionClock.GetTimestamp(), timeScale, false, 0);
    }

    private PreciseTimer(long startTicks, double timeScale, bool isPaused, long pauseTicks)
    {
        // Внутренний конструктор — доверяем себе
        _startTicks = startTicks;
        _timeScale = timeScale;
        _isPaused = isPaused;
        _pauseTicks = pauseTicks;
    }

// --- Управление состоянием ---

    /// <summary>
    /// Возобновляет таймер с текущего момента (если был на паузе).
    /// </summary>
    public PreciseTimer Resume()
    {
        if (!_isPaused) return this;
        long now = HighResolutionClock.GetTimestamp();
        long newStart = _startTicks + (now - _pauseTicks);
        return new PreciseTimer(newStart, _timeScale, false, 0);
    }

    /// <summary>
    /// Ставит таймер на паузу.
    /// </summary>
    public PreciseTimer Pause()
    {
        if (_isPaused) return this;
        long now = HighResolutionClock.GetTimestamp();
        return new PreciseTimer(_startTicks, _timeScale, true, now);
    }

    /// <summary>
    /// Сбрасывает таймер (перезапускает с текущего момента).
    /// </summary>
    public PreciseTimer Restart()
    {
        if (_timeScale <= 0)
            throw new InvalidOperationException("Cannot restart with invalid time scale.");

        return new PreciseTimer(HighResolutionClock.GetTimestamp(), _timeScale, false, 0);
    }

    /// <summary>
    /// Возвращает новый таймер с изменённым time scale.
    /// Если таймер на паузе — новый scale применится после Resume().
    /// </summary>
    /// <exception cref="ArgumentException">Если newTimeScale <= 0</exception>
    public PreciseTimer WithTimeScale(double newTimeScale)
    {
        if (newTimeScale <= 0)
            throw new ArgumentException("Time scale must be greater than zero.", nameof(newTimeScale));

        return new PreciseTimer(_startTicks, newTimeScale, _isPaused, _pauseTicks);
    }

// --- Чтение времени ---

    /// <summary>
    /// Возвращает прошедшее время в тиках (сырое значение).
    /// </summary>
    public long ElapsedTicks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            long reference = _isPaused ? _pauseTicks : HighResolutionClock.GetTimestamp();
            return reference - _startTicks;
        }
    }

    /// <summary>
    /// Прошедшее время в секундах.
    /// </summary>
    public double ElapsedSeconds
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ElapsedTicks * _timeScale * HighResolutionClock.TicksToSeconds;
    }

    /// <summary>
    /// Прошедшее время в миллисекундах.
    /// </summary>
    public double ElapsedMilliseconds
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ElapsedTicks * _timeScale * HighResolutionClock.TicksToMilliseconds;
    }

    /// <summary>
    /// Прошедшее время в микросекундах.
    /// </summary>
    public double ElapsedMicroseconds
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ElapsedTicks * _timeScale * HighResolutionClock.TicksToMicroseconds;
    }

// --- Удобные методы ---

    public bool HasElapsed(double seconds) => ElapsedSeconds >= seconds;
    public bool HasElapsedMs(double milliseconds) => ElapsedMilliseconds >= milliseconds;
}