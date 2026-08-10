namespace andecr.Services.Text;
/// <summary>
/// Defines a service that sanitizes text pasted from the clipboard before it is written into a text field,
/// stripping formatting artifacts that are irrelevant to the underlying data (e.g. a vocabulary card's front
/// or back text) but commonly survive a copy from word processors, PDFs, chat apps, or web pages.
/// </summary>
public interface IClipboardTextSanitizer
{
    /// <summary>
    /// Cleans the given text by removing hidden formatting characters, collapsing redundant spaces and
    /// non-breaking spaces, collapsing redundant line breaks, and stripping control characters.
    /// </summary>
    /// <param name="input">The raw text, typically taken directly from the clipboard.</param>
    /// <returns>
    /// The cleaned text, trimmed of leading/trailing whitespace. Never <see langword="null"/>; returns
    /// <see cref="string.Empty"/> for <see langword="null"/> or empty input.
    /// </returns>
    string Sanitize(string? input);
}

