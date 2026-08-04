using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Platform;

namespace andecr.Services;

/// <summary>
/// Загружает списки допустимых значений (тегов, частей речи, маркеров и т.п.)
/// из текстовых файлов, упакованных в приложение как ресурсы Avalonia
/// (<c>Assets/*.txt</c>). Каждая строка файла — одно значение списка.
/// </summary>
public static class TextOptionListLoader
{
    /// <summary>
    /// Читает файл вида <c>avares://andecr/Assets/Tags.txt</c> и возвращает
    /// непустые строки без начальных/конечных пробелов.
    /// </summary>
    /// <param name="assetPath">
    /// Путь к ресурсу относительно сборки, например <c>Assets/Tags.txt</c>.
    /// </param>
    public static IReadOnlyList<string> Load(string assetPath)
    {
        var uri = new Uri($"avares://andecr/{assetPath}");

        if (!AssetLoader.Exists(uri))
            return Array.Empty<string>();

        using var stream = AssetLoader.Open(uri);
        using var reader = new StreamReader(stream);

        var lines = new List<string>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var trimmed = line.Trim();
            if (trimmed.Length > 0)
                lines.Add(trimmed);
        }

        return lines;
    }
}
