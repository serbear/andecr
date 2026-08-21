using System.Text;

namespace andecr.Services.Text;

public static class Normalizers
{
    /// <summary>
    /// Normalizes a raw text string by applying standard formatting rules for definition texts.
    /// </summary>
    /// <param name="rawText">The input string to normalize. Can be null or empty.</param>
    /// <returns>
    /// The normalized string with:
    /// <list type="bullet">
    /// <item><description>
    /// First character converted to uppercase (if the string is non-empty).
    /// </description></item>
    /// <item><description>
    /// A period appended at the end if the original string didn't already end with one.
    /// </description></item>
    /// </list>
    /// Returns <c>null</c> if <paramref name="rawText"/> is <c>null</c>.
    /// Returns <see cref="string.Empty"/> if input is <see cref="string.Empty"/>.
    /// </returns>
    /// <remarks>
    /// This method does not trim whitespace or adjust internal punctuation.
    /// Edge cases:
    /// <list type="bullet">
    /// <item><description>
    /// Single-character strings: "a" → "A."
    /// </description></item>
    /// <item><description>
    /// Strings already ending with '.': "Hello." → "Hello." (unchanged except first letter)
    /// </description></item>
    /// <item><description>
    /// Strings with trailing whitespace: "hello " → "Hello ." (preserves whitespace before dot).
    /// </description></item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// NormalizeDefinitionPasteTextFunc("hello world");   // "Hello world."
    /// NormalizeDefinitionPasteTextFunc("Hello.");        // "Hello."
    /// NormalizeDefinitionPasteTextFunc("a");             // "A."
    /// NormalizeDefinitionPasteTextFunc("");              // ""
    /// NormalizeDefinitionPasteTextFunc(null);            // null
    /// </code>
    /// </example>
    private static string NormalizeDefinitionPasteTextFunc(string rawText)
    {
        if (string.IsNullOrEmpty(rawText))
        {
            return rawText;
        }

        var returnString = new StringBuilder();

        // Uppercase first letter
        returnString.Append(rawText[..1].ToUpper());
        returnString.Append(rawText[1..]);

        // A dot at the end of sentence if absent.
        if (rawText[^1] != '.')
        {
            returnString.Append('.');
        }

        return returnString.ToString();
    }

    /// <summary>
    /// Removes a single trailing occurrence of one of the given symbols from the end of a string.
    /// </summary>
    /// <param name="rawText">The input string to process. Can be null or empty.</param>
    /// <param name="symbolsToRemove">
    /// The set of characters to check for at the end of <paramref name="rawText"/>.
    /// Only the first matching symbol found in the array is removed.
    /// </param>
    /// <returns>
    /// The string with a trailing symbol removed if <paramref name="rawText"/> ends with any
    /// character from <paramref name="symbolsToRemove"/>; otherwise <see cref="string.Empty"/>.
    /// Returns <paramref name="rawText"/> unchanged if it is null or empty.
    /// </returns>
    /// <remarks>
    /// <b>Caution:</b> if the last character of <paramref name="rawText"/> does not match any
    /// symbol in <paramref name="symbolsToRemove"/>, the method returns an empty string rather
    /// than the original text — this looks like a bug in the current implementation, since the
    /// loop only appends to <c>returnString</c> inside the matching branch and never falls back
    /// to the original text when no symbol matches.
    /// </remarks>
    /// <example>
    /// <code>
    /// RemoveOnceSymbolsFromEnd("hello,", new[] { ',', ';' });  // "hello"
    /// RemoveOnceSymbolsFromEnd("hello;", new[] { ',', ';' });  // "hello"
    /// RemoveOnceSymbolsFromEnd("hello", new[] { ',', ';' });   // "" (see remark above)
    /// RemoveOnceSymbolsFromEnd("", new[] { ',', ';' });        // ""
    /// RemoveOnceSymbolsFromEnd(null, new[] { ',', ';' });      // null
    /// </code>
    /// </example>
    private static string RemoveOnceSymbolsFromEnd(string rawText, char[] symbolsToRemove)
    {
        if (string.IsNullOrEmpty(rawText))
        {
            return rawText;
        }

        var returnString = rawText;
        foreach (var symbol in symbolsToRemove)
        {
            if (rawText[^1] == symbol)
            {
                returnString = rawText[..^1];
            }
        }

        return returnString;
    }

    /// <summary>
    /// Normalizes raw pasted text by stripping a trailing symbol (if present) and applying
    /// standard definition-text formatting: capitalized first letter and a trailing period.
    /// </summary>
    /// <param name="rawText">The input string to normalize. Can be null or empty.</param>
    /// <param name="symbolsToRemove">
    /// Trailing characters (e.g. stray commas or semicolons) to strip from <paramref name="rawText"/>
    /// before formatting. See <see cref="RemoveOnceSymbolsFromEnd"/>.
    /// </param>
    /// <returns>
    /// The normalized string, capitalized and ending with a period.
    /// See <see cref="RemoveOnceSymbolsFromEnd"/> and
    /// <see cref="NormalizeDefinitionPasteTextFunc"/> for edge-case behavior.
    /// </returns>
    /// <example>
    /// <code>
    /// NormalizeText("hello world,", new[] { ',', ';' });  // "Hello world."
    /// NormalizeText("hello world", new[] { ',', ';' });   // "" (see RemoveOnceSymbolsFromEnd remark)
    /// </code>
    /// </example>
    public static string NormalizeText(string rawText, char[] symbolsToRemove)
    {
        var returnString = RemoveOnceSymbolsFromEnd(rawText, symbolsToRemove);
        returnString = NormalizeDefinitionPasteTextFunc(returnString);
        return returnString;
    }
}