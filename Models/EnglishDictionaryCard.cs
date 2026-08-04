namespace andecr.Models;

/// <summary>
/// Словарная карточка Anki для изучаемого английского слова или выражения.
/// </summary>
public class EnglishDictionaryCard
{
    /// <summary>
    /// Определение — описание изучаемого слова или выражения из словаря.
    /// </summary>
    public string Definition { get; set; } = string.Empty;

    /// <summary>
    /// Тег — смысловая область слова (например, "медицина" или "юриспруденция"),
    /// используется для разделения одинаковых слов. Значение выбирается из
    /// фиксированного списка, загружаемого из отдельного текстового файла.
    /// </summary>
    public string Tag { get; set; } = string.Empty;

    /// <summary>
    /// Словарная статья — название словарной карточки, отображаемое в списке
    /// карточек Anki.
    /// </summary>
    public string DictionaryEntry { get; set; } = string.Empty;

    /// <summary>
    /// Дословный перевод контекста использования изучаемого слова или выражения.
    /// </summary>
    public string LiteralTranslation { get; set; } = string.Empty;

    /// <summary>
    /// Оригинал — контекст использования изучаемого слова или выражения на
    /// изучаемом языке.
    /// </summary>
    public string Original { get; set; } = string.Empty;

    /// <summary>
    /// Литературный перевод — смысловой перевод оригинала.
    /// </summary>
    public string LiteraryTranslation { get; set; } = string.Empty;

    /// <summary>
    /// Часть речи — маркер части речи. Значение выбирается из фиксированного
    /// списка, загружаемого из отдельного текстового файла.
    /// </summary>
    public string PartOfSpeech { get; set; } = string.Empty;

    /// <summary>
    /// Транскрипция британская.
    /// </summary>
    public string BritishTranscription { get; set; } = string.Empty;

    /// <summary>
    /// Транскрипция американская.
    /// </summary>
    public string AmericanTranscription { get; set; } = string.Empty;

    /// <summary>
    /// Маркер — стилистическая пометка (например, "сленг", "устаревшее" или
    /// "формальное"). Значение выбирается из фиксированного списка,
    /// загружаемого из отдельного текстового файла.
    /// </summary>
    public string Marker { get; set; } = string.Empty;

    /// <summary>
    /// Уровень CEFR — уровень владения языком (A1, A2, B1, B2, C1, C2).
    /// </summary>
    public string CefrLevel { get; set; } = string.Empty;

    /// <summary>
    /// Звук — ссылка на звуковой файл озвучки изучаемого слова.
    /// </summary>
    public string Sound { get; set; } = string.Empty;

    /// <summary>
    /// Заметки — различные примечания относительно изучаемого слова или
    /// выражения.
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}
