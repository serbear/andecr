using System.Text;
using andecr.Models;

namespace andecr.Services.DeckExport;

public static class DeckExport
{
    private const char ExportFileFieldSeparator = '|';
    private static readonly string ExportFileHeader = $"#separator:{ExportFileFieldSeparator}\n#html:true";
    public static readonly string ExportFileTermFormatTemplate = "<span class=\"term\">{0}</span>";
    private static readonly string DefinitionFormatTemplate = "{0} ({1})";
    
    // Метод осбирает данные словарной карточки в строку записи файла экспорта колоды.
    // todo: Как формируется строка?
    //
    // fields - коллекция данных полей словарной карточки.
    public static string CreateDeckRecord(Dictionary<string, string> fields)
    {
        var result = new StringBuilder();
        const string tagFieldKey = nameof(EnglishDictionaryCard.Tag);
        string[] uppperCaseFields = [
            nameof(EnglishDictionaryCard.Tag),
            nameof(EnglishDictionaryCard.Marker)];

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

    // Возвращает строку состоящую из заголовка статьи и тега при наличии значения поля "Tag".
    // Если такого значения нет, то возвращает значение поля Definition (заголовок словарной статьи),
    // Возвращаемое значение используется в качестве названия словарной карточки в списке Anki.
    // Remark: Формат строки: "definition (TAG)". Значение тега в скобках, в верхнем регистре.
    //
    // definitionFieldValue - значение поля словарной карточки "Definition".
    // tagFieldValue - значение поля словарной карточки "Tag".
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