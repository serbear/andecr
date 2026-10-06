using Avalonia.Platform;

namespace andecr.Services;

/// <summary>
/// Loads lists of allowed values (tags, parts of speech, markers, etc.)
/// from text files bundled with the application as Avalonia resources
/// (<c>Assets/*.txt</c>). Each line in the file is a single list value.
/// </summary>
public static class TextOptionListLoader
{
    /// <summary>
    /// Reads a resource at <c>avares://andecr/Assets/filename.txt</c> and returns
    /// the non-empty, trimmed lines. Returns an empty list if the resource does not exist.
    /// </summary>
    /// <param name="assetPath">
    /// Path to the resource relative to the assembly, e.g. <c>Assets/filename.txt</c>.
    /// </param>
    /// <returns>A read-only list of the file's non-empty, trimmed lines.</returns>
    public static async Task<IReadOnlyList<string>> LoadAsync(string assetPath)
    {
        var uri = new Uri($"avares://andecr/{assetPath}");

        if (!AssetLoader.Exists(uri))
            return [];

        await using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);

        var lines = new List<string>();
        while (await reader.ReadLineAsync() is { } line)
        {
            var trimmed = line.Trim();
            if (trimmed.Length > 0)
                lines.Add(trimmed);
        }

        return lines;
    }
}