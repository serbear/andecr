namespace andecr.Services;

using System;
using System.IO;

public static class AppFileService
{
    /// <summary>
    /// Returns the full path to a file inside the OS-specific application data directory.
    /// If the application directory does not exist yet, it is created automatically.
    /// </summary>
    /// <remarks>
    /// The base directory depends on the platform:
    /// <list type="bullet">
    /// <item><description>Windows: <c>%LOCALAPPDATA%\{Constants.ApplicationName}</c></description></item>
    /// <item><description>Linux: <c>~/.config/{Constants.ApplicationName}</c></description></item>
    /// <item><description>macOS: <c>~/.config/{Constants.ApplicationName}</c></description></item>
    /// </list>
    /// The file itself is neither created nor checked for existence; the method only builds the path.
    /// </remarks>
    /// <param name="fileName">The file name without a path (for example, <c>"settings.json"</c>).</param>
    /// <returns>The full path to the file inside the application directory.</returns>
    /// <exception cref="PlatformNotSupportedException">
    /// Thrown if the current operating system is not Windows, Linux, or macOS.
    /// </exception>
    /// <exception cref="IOException">
    /// May be thrown if the application directory cannot be created.
    /// </exception>
    public static string GetFilePath(string fileName)
    {
        string basePath;
        const string configDirectory = ".config";

        if (OperatingSystem.IsWindows())
        {
            // %LOCALAPPDATA% (Environment.SpecialFolder.LocalApplicationData)
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            basePath = Path.Combine(
                localAppData,
                Constants.ApplicationName
            );
        }
        else if (OperatingSystem.IsLinux())
        {
            // ~/.config/andetor/
            // UserProfile returns home directory (~)
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            basePath = Path.Combine(
                homeDir,
                configDirectory,
                Constants.ApplicationName
            );
        }
        else if (OperatingSystem.IsMacOS())
        {
            // Для macOS по стандартам XDG тоже часто используют ~/.config/ 
            // (альтернативно можно использовать ~/Library/Application Support/)
            // var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            // basePath = Path.Combine(
            // homeDir,
            // configDirectory,
            // Constants.ApplicationName
            // );
            throw new NotImplementedException();
        }
        else
        {
            throw new PlatformNotSupportedException("The current operationg system is not supported.");
        }

        // Guarantee that the directory is exists.
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        return Path.Combine(basePath, fileName);
    }
}