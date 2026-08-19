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
    public static string NormalizeDefinitionPasteTextFunc(string rawText)
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
}