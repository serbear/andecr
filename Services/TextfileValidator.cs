namespace andecr.Services;

/// <summary>
/// Pre-flight checks for text files that are about to be read (option lists, etc.).
/// Kept separate from the view model so the rules can be reused and unit-tested without UI.
/// </summary>
public static class TextFileValidator
{
    /// <summary>How many leading bytes are inspected when deciding whether a file is text.</summary>
    private const int SniffLength = 8192;

    /// <summary>
    /// Verifies that <paramref name="path"/> points to an existing text file.
    /// </summary>
    /// <param name="path">Path to the file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// <c>true</c> if the file is a non-empty text file and can be read;
    /// <c>false</c> if the file is empty (the caller should simply stop).
    /// </returns>
    /// <exception cref="ArgumentException"><paramref name="path"/> is null or whitespace.</exception>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    /// <exception cref="InvalidDataException">The file looks binary (not a text file).</exception>
    public static async Task<bool> EnsureNonEmptyTextFileAsync(
        string path,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("File path must not be empty.", nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"File not found: '{path}'.", path);
        }

        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true
        );
        if (stream.Length == 0)
        {
            return false;
        }

        var buffer = new byte[(int)Math.Min(stream.Length, SniffLength)];
        var read = await stream.ReadAsync(buffer.AsMemory(), cancellationToken);
        return !LooksLikeText(buffer.AsSpan(0, read))
            ? throw new InvalidDataException($"File is not a text file: '{path}'.")
            : true;
    }

    /// <summary>
    /// Heuristic: text files do not contain NUL bytes, except UTF-16/UTF-32 files,
    /// which are recognized by their byte order mark.
    /// </summary>
    private static bool LooksLikeText(ReadOnlySpan<byte> head)
    {
        if (HasUnicodeBom(head))
        {
            return true;
        }

        return !head.Contains((byte)0);
    }

    private static bool HasUnicodeBom(ReadOnlySpan<byte> b)
    {
        return b.Length >= 2 &&
               ((b[0] == 0xFF && b[1] == 0xFE) || (b[0] == 0xFE && b[1] == 0xFF)) // UTF-16 LE/BE (and UTF-32 LE)
               || b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF; // UTF-32 BE
    }
}