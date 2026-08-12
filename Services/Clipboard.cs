using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace andecr.Services;

/// <summary>
/// Provides static methods for interacting with the system clipboard in an Avalonia application.
/// Supports retrieving text and available data formats, as well as copying text to the clipboard.
/// Handles platform-specific limitations and fallback behavior for unsupported operations.
/// </summary>
public static class Clipboard
{
    /// <summary>
    /// Asynchronously retrieves the text content from the system clipboard.
    /// </summary>
    /// <param name="userControl">The UserControl used to obtain the TopLevel and clipboard instance.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the clipboard text, or null if the clipboard is unavailable or no text is present.
    /// </returns>
    public static async Task<string?> GetTextAsync(UserControl userControl)
    {
        var topLevel = TopLevel.GetTopLevel(userControl);
        var clipboard = topLevel?.Clipboard;
        return clipboard is null ? null : await clipboard.TryGetTextAsync();
    }
    
    /// <summary>
    /// Asynchronously retrieves the list of data formats currently available on the system clipboard.
    /// </summary>
    /// <param name="userControl">The UserControl used to obtain the TopLevel and clipboard instance.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a read-only list of supported data formats,
    /// or null if the clipboard is unavailable or format enumeration is not supported.
    /// </returns>
    public static async Task<IReadOnlyList<DataFormat>?> GetFormatAsync(UserControl userControl)
    {
        var topLevel = TopLevel.GetTopLevel(userControl);
        var clipboard = topLevel?.Clipboard;
        if (clipboard is null)
        {
            return null;
        }
        try
        {
            return await clipboard.GetDataFormatsAsync();
        }
        catch (NotSupportedException)
        {
            // Some backends (e.g., Avalonia on Web, specific Linux builds) may not support format enumeration.
            // In this case, we simply assume that "the format is unknown" — a fallback check via
            // TryGetTextAsync in RefreshClipboardStateAsync will handle it later.
            return null;
        }
    }
    
    /// <summary>
    /// Asynchronously copies the specified text to the system clipboard.
    /// </summary>
    /// <param name="text">The text to copy to the clipboard.</param>
    /// <param name="window">
    /// An optional Window instance to use for clipboard access.
    /// If null, the active window is automatically retrieved.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when no window is available or the clipboard service is not accessible.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when an error occurs during the copy operation, wrapping the original exception message.
    /// </exception>
    public static async Task CopyTextAsync(string text, Window? window = null)
    {
        // Try to get an active window if 'window' is not passed.
        window ??= GetActiveWindow();

        if (window == null)
            throw new InvalidOperationException("Cannot get the window.");

        var clipboard = window.Clipboard;
        if (clipboard == null)
            throw new InvalidOperationException("The Clipboard is not available.");

        try
        {
            await clipboard.SetTextAsync(text);
        }
        catch (Exception ex)
        {
            throw new Exception($"Clipboard copy error: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Attempts to retrieve the currently active application window.
    /// </summary>
    /// <returns>The active Window instance, or null if no active window can be determined.</returns>
    private static Window? GetActiveWindow()
    {
        // Для получения активного окна можно использовать:
        return Application.Current?.ApplicationLifetime?
            .GetType()
            .GetProperty("MainWindow")?
            .GetValue(Application.Current.ApplicationLifetime) as Window;
    }
}