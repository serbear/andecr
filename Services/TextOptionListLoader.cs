namespace andecr.Services;

/// <summary>
/// Loads lists of allowed values (tags, parts of speech, markers, etc.) from text files
/// on disk. Each line in the file is a single list value.
/// </summary>
public static class TextOptionListLoader
{
    /// <summary>
    /// Reads the file at <paramref name="filePath"/> and returns its non-empty, trimmed lines.
    /// </summary>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    public static async Task<IReadOnlyList<string>> LoadAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var lines = new List<string>();

        await foreach (var line in File.ReadLinesAsync(filePath, cancellationToken))
        {
            var trimmed = line.Trim();
            if (trimmed.Length > 0)
                lines.Add(trimmed);
        }

        return lines;
    }
}