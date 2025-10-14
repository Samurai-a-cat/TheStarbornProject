# HighResolutionClock

`HighResolutionClock` — это вспомогательный внутренний статический класс, предназначенный для работы с высокоточным временем в .NET-приложениях, особенно в контексте игровых движков или систем, требующих точного измерения временных интервалов.

## 🎯 Цель и идея

Основная цель `HighResolutionClock` — обеспечить **максимально точное и производительное** измерение времени с использованием `System.Diagnostics.Stopwatch`, который в .NET использует высокоточные таймеры операционной системы (например, `QueryPerformanceCounter` на Windows).

Класс кэширует **частоту таймера** один раз при запуске приложения и предоставляет удобные коэффициенты преобразования «тиков» в секунды, миллисекунды и микросекунды. Это позволяет избежать повторных вычислений и повысить производительность в критичных к участкам кода (например, игровых циклах или симуляциях).

## 🔧 Принцип работы

- `Stopwatch.Frequency` — это количество «тиков» в секунду, предоставляемое системным высокоточным таймером.
- `Stopwatch.GetTimestamp()` возвращает текущее значение таймера в «тиках».
- `HighResolutionClock` предварительно вычисляет:
    - `TicksToSeconds = 1.0 / Frequency`
    - `TicksToMilliseconds = 1000.0 / Frequency`
    - `TicksToMicroseconds = 1_000_000.0 / Frequency`

Эти коэффициенты позволяют быстро конвертировать «сырые» тики в человекочитаемые единицы времени с помощью простого умножения.

Метод `GetTimestamp()` помечен атрибутом `[MethodImpl(MethodImplOptions.AggressiveInlining)]`, чтобы компилятор по возможности встраивал его вызов непосредственно в код, устраняя накладные расходы на вызов функции.

## 📌 Примеры использования

### Пример 1: Измерение времени выполнения участка кода

```csharp
using TheStarbornApp.GameCore.Services.TimeService.PreciseTimer;

long start = HighResolutionClock.GetTimestamp();

// ... какой-то код ...

long end = HighResolutionClock.GetTimestamp();
double elapsedMs = (end - start) * HighResolutionClock.TicksToMilliseconds;

Console.WriteLine($"Выполнено за {elapsedMs:F3} мс");
```
### Пример 2: Игровой цикл с фиксированным шагом
```cs
double lastTime = HighResolutionClock.GetTimestamp() * HighResolutionClock.TicksToSeconds;
const double fixedDeltaTime = 1.0 / 60.0; // 60 FPS

while (gameRunning)
{
double currentTime = HighResolutionClock.GetTimestamp() * HighResolutionClock.TicksToSeconds;
double deltaTime = currentTime - lastTime;

    if (deltaTime >= fixedDeltaTime)
    {
        UpdateGame(fixedDeltaTime);
        lastTime = currentTime;
    }

    Render();
}
```

⚠️ Важные замечания

Класс внутренний (internal) и предназначен только для использования внутри сборки TheStarbornApp.
Не предназначен для замены DateTime или DateTimeOffset — он работает с относительным временем, а не с абсолютными метками (например, «12:00 5 апреля 2025»).
Точность зависит от ОС и оборудования, но обычно составляет микросекунды или лучше на современных системах.
     