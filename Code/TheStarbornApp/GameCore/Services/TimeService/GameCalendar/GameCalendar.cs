namespace TheStarbornApp.GameCore.Services.TimeService.GameCalendar;

using System;

/// <summary>
/// Надёжная система крупного игрового времени с поддержкой миллионов лет,
/// отрицательных дат (до н.э.), привязки к реальному времени и офлайн-синхронизации.
/// Точность — до 1 секунды.
/// </summary>
public sealed class GameCalendar
{
    /// <summary>
    /// Общее количество секунд от эпохи (1 января 1 года н.э. по григорианскому календарю).
    /// Отрицательные значения соответствуют годам до н.э.
    /// </summary>
    private long _totalSeconds;

    /// <summary>
    /// Накопленное дробное время (в секундах), не достигшее целой секунды.
    /// Используется для предотвращения дрейфа при малых deltaTime или timeScale.
    /// Диапазон: [0, 1).
    /// </summary>
    private double _accumulatedTime;

    /// <summary>
    /// Коэффициент скорости игрового времени относительно реального.
    /// Должен быть строго больше нуля.
    /// </summary>
    private double _timeScale = 1.0;

    /// <summary>
    /// Указывает, находится ли игровое время на паузе.
    /// </summary>
    private bool _isPaused;

    /// <summary>
    /// Момент реального времени (UTC), к которому привязано игровое время.
    /// Используется для офлайн-синхронизации.
    /// </summary>
    private DateTime? _realTimeAnchor;

    /// <summary>
    /// Значение <see cref="_totalSeconds"/> на момент <see cref="_realTimeAnchor"/>.
    /// </summary>
    private long _gameTimeAnchor;

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GameCalendar"/> с начальным временем 0
    /// (1 января 1 года н.э., 00:00:00).
    /// </summary>
    public GameCalendar()
    {
    }

    /// <summary>
    /// Инициализирует новый экземпляр <see cref="GameCalendar"/> с заданным количеством секунд
    /// от эпохи (1 января 1 года н.э.).
    /// </summary>
    /// <param name="totalSeconds">Секунды от эпохи. Отрицательные значения — до н.э.</param>
    public GameCalendar(long totalSeconds)
    {
        _totalSeconds = totalSeconds;
    }

    /// <summary>
    /// Получает или задаёт общее количество секунд от эпохи.
    /// </summary>
    public long TotalSeconds => _totalSeconds;

    /// <summary>
    /// Получает или задаёт коэффициент скорости игрового времени.
    /// Значение должно быть больше нуля.
    /// </summary>
    /// <exception cref="ArgumentException">Если значение меньше или равно нулю.</exception>
    public double TimeScale
    {
        get => _timeScale;
        set => _timeScale = value > 0 
            ? value 
            : throw new ArgumentException("Time scale must be greater than zero.", nameof(value));
    }

    /// <summary>
    /// Получает или задаёт флаг паузы игрового времени.
    /// </summary>
    public bool IsPaused
    {
        get => _isPaused;
        set => _isPaused = value;
    }

    /// <summary>
    /// Обновляет игровое время на основе прошедшего реального времени.
    /// Вызывается каждый кадр/тик игрового цикла.
    /// </summary>
    /// <param name="deltaTime">Прошедшее реальное время в секундах.</param>
    public void Update(double deltaTime)
    {
        if (_isPaused) return;

        _accumulatedTime += deltaTime * _timeScale;

        // Извлекаем целые секунды
        long wholeSeconds = (long)_accumulatedTime;
        if (wholeSeconds != 0)
        {
            _totalSeconds += wholeSeconds;
            _accumulatedTime -= wholeSeconds;
        }

        // Нормализуем накопленное время в [0, 1)
        // (защита от ошибок округления при очень больших значениях)
        if (_accumulatedTime >= 1.0)
        {
            _totalSeconds += 1;
            _accumulatedTime -= 1.0;
        }
        else if (_accumulatedTime < 0.0)
        {
            // Теоретически не должно происходить, но на всякий случай
            _totalSeconds -= 1;
            _accumulatedTime += 1.0;
        }
    }

    /// <summary>
    /// Устанавливает привязку к реальному времени для последующей офлайн-синхронизации.
    /// </summary>
    /// <param name="utcNow">Текущее реальное время в UTC.</param>
    public void SetRealTimeAnchor(DateTime utcNow)
    {
        _realTimeAnchor = utcNow;
        _gameTimeAnchor = _totalSeconds;
    }

    /// <summary>
    /// Синхронизирует игровое время с реальным, учитывая прошедшее время в офлайне.
    /// </summary>
    /// <param name="utcNow">Текущее реальное время в UTC.</param>
    public void SyncFromRealTime(DateTime utcNow)
    {
        if (_realTimeAnchor.HasValue)
        {
            var deltaSeconds = (utcNow - _realTimeAnchor.Value).TotalSeconds;
            var gameDelta = deltaSeconds * _timeScale;

            _accumulatedTime += gameDelta;
            long wholeSeconds = (long)_accumulatedTime;
            _totalSeconds += wholeSeconds;
            _accumulatedTime -= wholeSeconds;

            // Обновляем якоря
            _realTimeAnchor = utcNow;
            _gameTimeAnchor = _totalSeconds;
        }
    }

    /// <summary>
    /// Преобразует текущее игровое время в структуру календарной даты.
    /// </summary>
    /// <returns>Структура <see cref="GameDateTime"/>, представляющая текущую дату и время.</returns>
    public GameDateTime ToDateTime()
    {
        return GameDateTime.FromTotalSeconds(_totalSeconds, _accumulatedTime);
    }

    /// <summary>
    /// Деструктурирует объект для сериализации.
    /// </summary>
    /// <param name="totalSeconds">Общее количество секунд от эпохи.</param>
    /// <param name="accumulatedTime">Накопленное дробное время.</param>
    /// <param name="realAnchor">Якорь реального времени.</param>
    /// <param name="gameAnchor">Якорь игрового времени.</param>
    public void Deconstruct(
        out long totalSeconds,
        out double accumulatedTime,
        out DateTime? realAnchor,
        out long gameAnchor)
    {
        totalSeconds = _totalSeconds;
        accumulatedTime = _accumulatedTime;
        realAnchor = _realTimeAnchor;
        gameAnchor = _gameTimeAnchor;
    }

    /// <summary>
    /// Восстанавливает <see cref="GameCalendar"/> из сериализованных данных.
    /// </summary>
    /// <param name="totalSeconds">Общее количество секунд от эпохи.</param>
    /// <param name="accumulatedTime">Накопленное дробное время.</param>
    /// <param name="realAnchor">Якорь реального времени.</param>
    /// <param name="gameAnchor">Якорь игрового времени.</param>
    /// <param name="timeScale">Коэффициент скорости времени.</param>
    /// <param name="isPaused">Флаг паузы.</param>
    /// <returns>Восстановленный экземпляр <see cref="GameCalendar"/>.</returns>
    /// <exception cref="ArgumentException">Если timeScale &lt;= 0.</exception>
    public static GameCalendar Reconstruct(
        long totalSeconds,
        double accumulatedTime,
        DateTime? realAnchor,
        long gameAnchor,
        double timeScale = 1.0,
        bool isPaused = false)
    {
        if (timeScale <= 0)
            throw new ArgumentException("Time scale must be greater than zero.", nameof(timeScale));

        // Нормализуем accumulatedTime в [0, 1)
        long whole = (long)accumulatedTime;
        totalSeconds += whole;
        accumulatedTime -= whole;
        if (accumulatedTime < 0)
        {
            totalSeconds--;
            accumulatedTime += 1.0;
        }

        var cal = new GameCalendar(totalSeconds)
        {
            _accumulatedTime = accumulatedTime,
            _timeScale = timeScale,
            _isPaused = isPaused,
            _realTimeAnchor = realAnchor,
            _gameTimeAnchor = gameAnchor
        };
        return cal;
    }
}