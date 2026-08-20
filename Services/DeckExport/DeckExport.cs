using System.Text;
using andecr.Models;

namespace andecr.Services.DeckExport;

/// <summary>
/// Builds record strings for the Anki deck export file from vocabulary card field values.
/// </summary>
/// <remarks>
/// The export file format is a pipe-delimited (<c>|</c>) plain-text file consumable by Anki's
/// text import ("Notes in Plain Text or CSV"), with an <c>#separator</c>/<c>#html</c> header
/// declared via <see cref="ExportFileHeader"/>.
/// </remarks>
public static class DeckExport
{
    private const char ExportFileFieldSeparator = '|';
    private const string DefinitionFormatTemplate = "{0} ({1})";
    private static readonly string ExportFileHeader = $"#separator:{ExportFileFieldSeparator}\n#html:true";
    public static readonly string ExportFileTermFormatTemplate = "<span class=\"term\">{0}</span>";

    /// <summary>
    /// Concatenates a vocabulary card's field values into a single export-file record string.
    /// </summary>
    /// <remarks>
    /// Fields are appended in <paramref name="fields"/>'s enumeration order and joined by
    /// <see cref="ExportFileFieldSeparator"/>, with no trailing separator after the last field.
    /// Two transformations are applied along the way:
    /// <list type="bullet">
    /// <item><description>
    /// The <c>DictionaryEntry</c> field's value is replaced by the combined
    /// "definition (TAG)" string produced by <see cref="DictionaryEntryWithTag"/>.
    /// </description></item>
    /// <item><description>
    /// The <c>Tag</c> and <c>Marker</c> field values are upper-cased.
    /// </description></item>
    /// </list>
    /// Callers are expected to pass a dictionary keyed by <see cref="EnglishDictionaryCard"/> field
    /// names (as produced by, e.g., <c>CardField.All.ToDictionary(...)</c>), in the field order
    /// intended for the export record, and to always include a <c>Tag</c> entry — it is read
    /// unconditionally when a <c>DictionaryEntry</c> field is present, even if empty.
    /// </remarks>
    /// <param name="fields">
    /// The vocabulary card's field values, keyed by field name.
    /// </param>
    /// <returns>The pipe-delimited export record string for one card.</returns>
    public static string CreateDeckRecord(Dictionary<string, string> fields)
    {
        var result = new StringBuilder();
        const string tagFieldKey = nameof(EnglishDictionaryCard.Tag);
        string[] uppperCaseFields =
        [
            nameof(EnglishDictionaryCard.Tag),
            nameof(EnglishDictionaryCard.Marker)
        ];

        // Добавить данные полей словарной карточки
        foreach (var field in fields)
        {
            if (field.Key == nameof(EnglishDictionaryCard.DictionaryEntry))
            {
                result.Append(
                    DictionaryEntryWithTag(
                        field.Value,
                        fields[tagFieldKey]));
            }
            else if (uppperCaseFields.Contains(field.Key))
            {
                // Значения полей словарной карточки "Tag" и "Marker" должны быть в верхнем регистре.
                result.Append(field.Value.ToUpper());
            }
            else
            {
                result.Append(field.Value);
            }

            // Добавить разделитель
            result.Append(ExportFileFieldSeparator);
        }

        // Последний разделитель полей слованой карточки не нужен.
        result = result.Remove(result.Length - 1, 1);

        return result.ToString();
    }

    /// <summary>
    /// Builds the display title used for a card's export record, combining the definition with its
    /// tag when present.
    /// </summary>
    /// <remarks>
    /// Returns <c>"{definition} ({TAG})"</c> when <paramref name="tagFieldValue"/> is non-empty (the
    /// tag is upper-cased), or just <paramref name="definitionFieldValue"/> otherwise. This value is
    /// used as the card's title in the Anki deck list.
    /// TODO: <paramref name="definitionFieldValue"/> is expected to be non-empty, but this is not
    /// currently validated — confirm whether an empty value should throw, or what the fallback
    /// behavior should be.
    /// </remarks>
    /// <param name="definitionFieldValue">The card's <c>Definition</c> field value.</param>
    /// <param name="tagFieldValue">The card's <c>Tag</c> field value, or empty if none.</param>
    /// <returns>The formatted dictionary entry title.</returns>
    private static string DictionaryEntryWithTag(string definitionFieldValue, string tagFieldValue)
    {
        // todo: definitionFieldValue не должен быть пустым.

        return string.IsNullOrEmpty(tagFieldValue)
            ? definitionFieldValue
            : string.Format(
                DefinitionFormatTemplate,
                definitionFieldValue,
                tagFieldValue.ToUpper());
    }
}