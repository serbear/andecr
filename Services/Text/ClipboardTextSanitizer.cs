using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace andecr.Services.Text;

/// <summary>
/// Default implementation of <see cref="IClipboardTextSanitizer"/>.
/// </summary>
/// <remarks>
/// Cleaning happens in two passes:
/// <list type="number">
/// <item>
/// A single character-by-character pass that normalizes line endings to <c>'\n'</c> and drops/replaces
/// characters based on their <see cref="UnicodeCategory"/> (control characters, hidden formatting
/// characters, exotic space variants, Unicode line/paragraph separators). Classifying by Unicode category
/// rather than a hardcoded blacklist means uncommon zero-width or bidi-control characters introduced by a
/// particular source application are still caught, not just the usual handful (ZWSP, BOM, NBSP, etc.).
/// </item>
/// <item>
/// A small set of regex passes that collapse the now-uniform ASCII space/newline characters: multiple
/// spaces into one, spaces hugging a line break removed, and multiple consecutive line breaks into one.
/// </item>
/// </list>
/// </remarks>
public sealed class ClipboardTextSanitizer : IClipboardTextSanitizer
{
    /// <summary>
    /// Matches runs of two or more consecutive regular spaces or tabs, used to collapse "extra" spaces
    /// down to a single space.
    /// </summary>
    private static readonly Regex MultipleSpacesRegex = new("[ \t]{2,}", RegexOptions.Compiled);

    /// <summary>
    /// Matches horizontal whitespace immediately touching a line break, used to trim trailing/leading
    /// spaces on each line without a separate per-line loop.
    /// </summary>
    private static readonly Regex SpaceAroundLineBreakRegex = new("[ \t]*\n[ \t]*", RegexOptions.Compiled);

    /// <summary>
    /// Matches runs of two or more consecutive line breaks (after normalization to <c>'\n'</c>), used to
    /// collapse "extra" line breaks down to a single one.
    /// </summary>
    private static readonly Regex MultipleLineBreaksRegex = new("\n{2,}", RegexOptions.Compiled);

    /// <inheritdoc />
    public string Sanitize(string? input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        var normalized = NormalizeCharacters(input);

        // Collapse extra spaces, then drop spaces hugging a line break, then collapse extra line breaks.
        // Order matters: spaces must be collapsed first so "word   \n   word" doesn't leave stray
        // single spaces behind once the surrounding runs are trimmed.
        normalized = MultipleSpacesRegex.Replace(normalized, " ");
        normalized = SpaceAroundLineBreakRegex.Replace(normalized, "\n");
        normalized = MultipleLineBreaksRegex.Replace(normalized, "\n");

        return normalized.Trim();
    }

    /// <summary>
    /// Performs the character-level normalization pass: unifies line endings and removes or replaces
    /// characters based on their Unicode category.
    /// </summary>
    /// <param name="input">The raw input text.</param>
    /// <returns>Text containing only regular spaces, <c>'\n'</c> line breaks, and "visible" characters.</returns>
    private static string NormalizeCharacters(string input)
    {
        var builder = new StringBuilder(input.Length);
        var previousWasCarriageReturn = false;

        foreach (var c in input)
        {
            if (c == '\r')
            {
                // Defer emitting the line break until we know whether a following '\n' belongs to the
                // same CRLF pair, so "\r\n" collapses to a single '\n' rather than two.
                builder.Append('\n');
                previousWasCarriageReturn = true;
                continue;
            }

            if (c == '\n')
            {
                if (!previousWasCarriageReturn)
                {
                    builder.Append('\n');
                }

                previousWasCarriageReturn = false;
                continue;
            }

            previousWasCarriageReturn = false;

            switch (CharUnicodeInfo.GetUnicodeCategory(c))
            {
                case UnicodeCategory.Control:
                    // Drop C0/C1 control characters (NUL, BEL, VT, FF, ESC, etc.). A literal tab is kept
                    // as horizontal whitespace so it still separates words; it gets collapsed like any
                    // other space in the regex pass below.
                    if (c == '\t')
                    {
                        builder.Append(' ');
                    }

                    break;

                case UnicodeCategory.Format:
                    // Hidden formatting characters: zero-width space/joiner/non-joiner, BOM, left-to-right
                    // and right-to-left marks, bidi isolates/embeddings, soft hyphen, etc.
                    break;

                case UnicodeCategory.SpaceSeparator:
                    // Non-breaking space and other Unicode space variants (thin space, em space, ideographic
                    // space, ...) collapse down to a regular space.
                    builder.Append(' ');
                    break;

                case UnicodeCategory.LineSeparator:
                case UnicodeCategory.ParagraphSeparator:
                    // U+2028 / U+2029, sometimes produced by rich text sources.
                    builder.Append('\n');
                    break;

                default:
                    builder.Append(c);
                    break;
            }
        }

        return builder.ToString();
    }
}