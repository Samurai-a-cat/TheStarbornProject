// File: TheStarbornApp/GameCore/Services/Validators/IValidatableTemplateData.cs
using TheStarbornApp.GameCore.TemporalWorldFabric.Entityes;

namespace TheStarbornApp.GameCore.Services.Validators;

/// <summary>
/// Интерфейс для шаблонов данных, которые могут быть валидированы.
/// </summary>
/// <typeparam name="T">Тип самого шаблона данных.</typeparam>
public interface IValidatableTemplateData<in T> : ITemplateData where T : ITemplateData
{
    /// <summary>
    /// Валидирует текущий экземпляр. Выбрасывает исключение при ошибке.
    /// Вызывает общую валидацию и затем специфичную.
    /// </summary>
    void Validate()
    {
        // --- Общая валидация ---
        if (string.IsNullOrWhiteSpace(TemplateName))
            throw new InvalidOperationException($"TemplateName for {typeof(T).Name} cannot be null or empty");

        // --- Специфичная валидация ---
        ValidateSpecific();
    }

    /// <summary>
    /// Метод для специфичной валидации. Должен быть реализован наследниками.
    /// </summary>
    void ValidateSpecific();
}