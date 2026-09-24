namespace andecr.ViewModels.Controls;

using System.Reactive;
using ReactiveUI;

/// <summary>
/// View model implementation for a selected-text markup toolbar. Wraps the current selection of a bound text
/// field in a configurable pair of open/close tags (e.g. "&lt;MARK&gt;"/"&lt;/MARK&gt;").
/// </summary>
public class SelectedTextToolsViewModel : ReactiveObject, ISelectedTextToolsViewModel
{
    /// <summary>
    /// Delegate returning the tag inserted immediately after the selected text. Read live for the same reason
    /// as <see cref="_getOpenTag"/>.
    /// </summary>
    private readonly Func<string> _getCloseTag;

    /// <summary>
    /// Delegate returning the tag inserted immediately before the selected text. Read live (rather than
    /// captured once) so it stays correct even if the owning control's OpenTag is set after construction.
    /// </summary>
    private readonly Func<string> _getOpenTag;

    /// <summary>
    /// Delegate returning the end index of the current selection in the bound text field.
    /// </summary>
    private readonly Func<int> _getSelectionEnd;

    /// <summary>
    /// Delegate returning the start index of the current selection in the bound text field.
    /// </summary>
    private readonly Func<int> _getSelectionStart;

    /// <summary>
    /// Delegate returning the full current text of the bound text field.
    /// </summary>
    private readonly Func<string?> _getText;

    /// <summary>
    /// Callback action to replace the full text of the bound text field.
    /// </summary>
    private readonly Action<string?> _setText;

    /// <summary>
    /// Backing field for <see cref="CanInsertMarkup"/>.
    /// </summary>
    private bool _canInsertMarkup;

    /// <summary>
    /// Backing field for <see cref="CanUnmark"/>.
    /// </summary>
    private bool _canUnmark;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectedTextToolsViewModel"/> class.
    /// </summary>
    /// <param name="getSelectionStart">Delegate returning the bound field's current selection start index.</param>
    /// <param name="getSelectionEnd">Delegate returning the bound field's current selection end index.</param>
    /// <param name="getText">Delegate returning the bound field's current full text.</param>
    /// <param name="setText">Action invoked with the updated full text after markup is inserted.</param>
    /// <param name="getOpenTag">Delegate returning the tag placed before the selected text.</param>
    /// <param name="getCloseTag">Delegate returning the tag placed after the selected text.</param>
    public SelectedTextToolsViewModel(
        Func<int> getSelectionStart,
        Func<int> getSelectionEnd,
        Func<string?> getText,
        Action<string?> setText,
        Func<string> getOpenTag,
        Func<string> getCloseTag)
    {
        _getSelectionStart = getSelectionStart;
        _getSelectionEnd = getSelectionEnd;
        _getText = getText;
        _setText = setText;
        _getOpenTag = getOpenTag;
        _getCloseTag = getCloseTag;

        var canInsert = this.WhenAnyValue(x => x.CanInsertMarkup);
        InsertMarkupCommand = ReactiveCommand.Create(InsertMarkup, canInsert);
        UnmarkCommand = ReactiveCommand.Create(Unmark, this.WhenAnyValue(vm => vm.CanUnmark));
    }

    /// <inheritdoc />
    public bool CanUnmark
    {
        get => _canUnmark;
        private set => this.RaiseAndSetIfChanged(ref _canUnmark, value);
    }

    /// <inheritdoc />
    public bool CanInsertMarkup
    {
        get => _canInsertMarkup;
        private set => this.RaiseAndSetIfChanged(ref _canInsertMarkup, value);
    }

    /// <inheritdoc />
    public ReactiveCommand<Unit, Unit> InsertMarkupCommand { get; }

    /// <inheritdoc />
    public ReactiveCommand<Unit, Unit> UnmarkCommand { get; }

    /// <inheritdoc />
    public void RefreshSelectionState()
    {
        CanInsertMarkup = _getSelectionStart() != _getSelectionEnd();

        var text = _getText();
        var openTag = _getOpenTag();
        var closeTag = _getCloseTag();
        CanUnmark = !string.IsNullOrEmpty(text)
                    && (text.Contains(openTag) || (closeTag.Length > 0 && text.Contains(closeTag)));
    }

    /// <summary>
    /// Removes every instance of the configured open and close tags from the bound text field's text.
    /// Does nothing if the text is null or empty.
    /// </summary>
    private void Unmark()
    {
        var text = _getText();
        if (string.IsNullOrEmpty(text)) return;

        var updated = text.Replace(_getOpenTag(), string.Empty).Replace(_getCloseTag(), string.Empty);
        _setText(updated);
    }

    /// <summary>
    /// Wraps the currently selected text of the bound field in the configured open/close tags. Does nothing
    /// if there is no selection, or if the reported selection indices no longer fit the current text (e.g.
    /// the text changed elsewhere between the last selection update and this call).
    /// </summary>
    private void InsertMarkup()
    {
        Console.Write("InsertMarkup");
        var start = _getSelectionStart();
        var end = _getSelectionEnd();
        if (start == end)
        {
            return;
        }

        var lowerIndex = Math.Min(start, end);
        var upperIndex = Math.Max(start, end);
        var text = _getText() ?? string.Empty;

        if (lowerIndex < 0 || upperIndex > text.Length)
        {
            return;
        }

        var selectedText = text[lowerIndex..upperIndex];
        var updatedText = string.Concat(
            text[..lowerIndex],
            _getOpenTag(),
            selectedText,
            _getCloseTag(),
            text[upperIndex..]);

        _setText(updatedText);
    }
}