using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TheStarbornApp.GameCore.Services.TimeService.PreciseTimer;

/// <summary>
/// Вспомогательный кэш частоты (один раз при старте)
/// </summary>
internal static class HighResolutionClock
{
    public static readonly long Frequency = Stopwatch.Frequency;
    public static readonly double TicksToSeconds = 1.0 / Frequency;
    public static readonly double TicksToMilliseconds = 1000.0 / Frequency;
    public static readonly double TicksToMicroseconds = 1_000_000.0 / Frequency;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetTimestamp() => Stopwatch.GetTimestamp();
}