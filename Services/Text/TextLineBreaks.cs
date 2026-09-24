namespace andecr.Services.Text;

public static class TextLineBreaks
{
    private const string NewlineTagValue = "</br>";

    /// <summary>
    /// Replaces all line-break variants (\r\n, \r, \n) with <see cref="NewlineTagValue"/>, so the
    /// replacement is independent of the OS the text was typed/pasted on.
    /// </summary>
    public static string ReplaceNewlinesWithTag(string value)
    {
        return string.IsNullOrEmpty(value)
            ? value
            : value.Replace("\r\n", NewlineTagValue)
                .Replace("\r", NewlineTagValue)
                .Replace("\n", NewlineTagValue);
    }
}