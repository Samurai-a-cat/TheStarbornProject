namespace TheStarbornApp.GameCore.Services.TimeService.GameCalendar;

using System;

/// <summary>
/// Представляет календарную дату и время в игровой вселенной.
/// Поддерживает годы до н.э. (отрицательные значения).
/// </summary>
public readonly struct GameDateTime : IEquatable<GameDateTime>
{
    /// <summary>
    /// Год. Отрицательные значения означают годы до н.э.
    /// Например: -500 = 500 год до н.э.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// Месяц (1–12).
    /// </summary>
    public int Month { get; }

    /// <summary>
    /// День месяца (1–31).
    /// </summary>
    public int Day { get; }

    /// <summary>
    /// Час (0–23).
    /// </summary>
    public int Hour { get; }

    /// <summary>
    /// Минута (0–59).
    /// </summary>
    public int Minute { get; }

    /// <summary>
    /// Секунда (0–59).
    /// </summary>
    public int Second { get; }

    private GameDateTime(int year, int month, int day, int hour, int minute, int second)
    {
        Year = year;
        Month = month;
        Day = day;
        Hour = hour;
        Minute = minute;
        Second = second;
    }

    /// <summary>
    /// Преобразует общее количество секунд от эпохи в календарную дату.
    /// </summary>
    /// <param name="totalSeconds">Целые секунды от эпохи.</param>
    /// <param name="fractionalSeconds">Дробная часть секунд (0 ≤ fractionalSeconds меньше 1).</param>
    /// <returns>Структура <see cref="GameDateTime"/>.</returns>
    public static GameDateTime FromTotalSeconds(long totalSeconds, double fractionalSeconds = 0.0)
    {
        const long epochJulianDay = 1721426; // 1 янв 1 г. н.э.
        const long secondsPerDay = 86400;

        long secondsInDayRaw = totalSeconds % secondsPerDay;
        double totalSecondsInDay = secondsInDayRaw + fractionalSeconds;

        // Обработка отрицательных секунд
        if (totalSecondsInDay < 0)
        {
            totalSecondsInDay += secondsPerDay;
            totalSeconds -= secondsPerDay;
        }

        long julianDay = epochJulianDay + totalSeconds / secondsPerDay;
        long secondsInDay = (long)Math.Floor(totalSecondsInDay);

        (int year, int month, int day) = JulianDayToGregorian(julianDay);

        int hour = (int)(secondsInDay / 3600);
        int minute = (int)((secondsInDay % 3600) / 60);
        int second = (int)(secondsInDay % 60);

        return new GameDateTime(year, month, day, hour, minute, second);
    }

    private static (int year, int month, int day) JulianDayToGregorian(long jd)
    {
        long j = jd + 32044;
        long g = j / 146097;
        long dg = j % 146097;
        long c = (dg / 36524 + 1) * 3 / 4;
        long dc = dg - c * 36524;
        long b = dc / 1461;
        long db = dc % 1461;
        long a = (db / 365 + 1) * 3 / 4;
        long da = db - a * 365;
        long y = g * 400 + c * 100 + b * 4 + a;
        long m = (da * 5 + 308) / 153 - 2;
        long d = da - (m + 4) * 153 / 5 + 122;
        long year = y - 4800 + (m + 2) / 12;
        long month = (m + 2) % 12 + 1;
        long day = d + 1;

        return ((int)year, (int)month, (int)day);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        if (Year <= 0)
            return $"{-Year + 1} BC-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}:{Second:D2}";
        return $"{Year:D4}-{Month:D2}-{Day:D2} {Hour:D2}:{Minute:D2}:{Second:D2}";
    }

    /// <inheritdoc/>
    public bool Equals(GameDateTime other) =>
        Year == other.Year && Month == other.Month && Day == other.Day &&
        Hour == other.Hour && Minute == other.Minute && Second == other.Second;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is GameDateTime other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Year, Month, Day, Hour, Minute, Second);
}