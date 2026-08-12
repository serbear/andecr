using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using andecr.Services.Text;
using Avalonia.Input;
using ReactiveUI;

namespace andecr.ViewModels.Controls;

/// <summary>
/// View model implementation for the custom text box control handling clipboard synchronization logic.
/// </summary>
public class AndecrTextBoxViewModel : ReactiveObject, IAndecrTextBoxViewModel
{
    /// <summary>
    /// Asynchronous delegate to retrieve plain text from the clipboard.
    /// </summary>
    private readonly Func<Task<string?>> _getClipboardTextAsync;
    
    /// <summary>
    /// Asynchronous delegate to retrieve supported data formats currently stored in the clipboard.
    /// </summary>
    private readonly Func<Task<IReadOnlyList<DataFormat>?>> _getClipboardFormatsAsync;
    
    /// <summary>
    /// Callback action to update the text in the view target.
    /// </summary>
    private readonly Action<string?> _setTextAction;

    /// <summary>
    /// Service that cleans raw clipboard text (hidden formatting characters, redundant whitespace/line
    /// breaks, control characters) before it is handed to <see cref="_setTextAction"/>.
    /// </summary>
    private readonly IClipboardTextSanitizer _sanitizer;

    /// <summary>
    /// Backing field for <see cref="CanPasteText"/>.
    /// </summary>
    private bool _canPasteText;

    /// <summary>
    /// Gets a value indicating whether non-empty text is available in the clipboard.
    /// Updated via <see cref="RefreshClipboardStateAsync"/>.
    /// </summary>
    public bool CanPasteText
    {
        get => _canPasteText;
        private set => this.RaiseAndSetIfChanged(ref _canPasteText, value);
    }
    
    /// <summary>
    /// Gets the reactive command that pastes text from the clipboard into the text field.
    /// </summary>
    public ReactiveCommand<Unit, Unit> PasteTextCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndecrTextBoxViewModel"/> class with clipboard provider callbacks.
    /// </summary>
    /// <param name="getClipboardTextAsync">Delegate returning clipboard text asynchronously.</param>
    /// <param name="getClipboardFormatsAsync">Delegate returning available clipboard formats asynchronously.</param>
    /// <param name="setTextAction">Action invoked to set the pasted text string.</param>
    /// <param name="sanitizer">
    /// Service used to clean pasted text before it reaches <paramref name="setTextAction"/>. Defaults to a
    /// new <see cref="TextValueSanitizer"/> instance when omitted, since the sanitizer is stateless and
    /// safe to share.
    /// </param>
    public AndecrTextBoxViewModel(
        Func<Task<string?>> getClipboardTextAsync,
        Func<Task<IReadOnlyList<DataFormat>?>> getClipboardFormatsAsync,
        Action<string?> setTextAction,
        IClipboardTextSanitizer? sanitizer = null)
    {
        _getClipboardTextAsync = getClipboardTextAsync;
        _getClipboardFormatsAsync = getClipboardFormatsAsync;
        _setTextAction = setTextAction;
        _sanitizer = sanitizer ?? new TextValueSanitizer();

        var canPaste = this.WhenAnyValue(x => x.CanPasteText);
        PasteTextCommand = ReactiveCommand.CreateFromTask(PasteAsync, canPaste);
    }
    
    /// <summary>
    /// Executes the paste operation by fetching text from the clipboard provider, cleaning it via
    /// <see cref="_sanitizer"/>, and applying the result via the text callback.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task PasteAsync()
    {
        var text = await _getClipboardTextAsync();
        var cleanedText = _sanitizer.Sanitize(text);
        if (!string.IsNullOrEmpty(cleanedText))
        {
            _setTextAction(cleanedText);
        }
    }

    /// <summary>
    /// Re-evaluates <see cref="CanPasteText"/> based on the current clipboard contents.
    /// Performs an efficient check by checking available data formats prior to fetching the actual payload.
    /// </summary>
    /// <returns>A task representing the asynchronous refresh operation.</returns>
    public async Task RefreshClipboardStateAsync()
    {
        var formats = await _getClipboardFormatsAsync();

        var hasTextFormat = formats is not null
            && formats.Contains(DataFormat.Text);

        if (!hasTextFormat)
        {
            CanPasteText = false;
            return;
        }

        var text = await _getClipboardTextAsync();

        // Run the same cleanup that PasteAsync applies: clipboard content that consists solely of
        // control/formatting characters or whitespace should not enable pasting, even though the raw
        // string is technically non-empty.
        CanPasteText = !string.IsNullOrEmpty(_sanitizer.Sanitize(text));
    }
}