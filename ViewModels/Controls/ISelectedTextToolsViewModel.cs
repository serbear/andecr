namespace andecr.ViewModels.Controls;

using System.Reactive;
using ReactiveUI;

/// <summary>
/// Defines the view model contract for a selected-text markup toolbar, providing the ability to wrap the
/// currently selected text of a bound text field in a pair of tags.
/// </summary>
public interface ISelectedTextToolsViewModel
{
    /// <summary>
    /// Gets a value indicating whether the bound text field currently has a non-empty text selection.
    /// </summary>
    bool CanInsertMarkup { get; }

    /// <summary>
    /// Gets the command that wraps the current selection in the configured open/close tags.
    /// </summary>
    ReactiveCommand<Unit, Unit> InsertMarkupCommand { get; }

    /// <summary>
    /// Re-evaluates <see cref="CanInsertMarkup"/> based on the current selection of the bound text field.
    /// Must be called whenever the bound field's selection (or its underlying text) changes, since there is
    /// no shared "selection changed" event to observe automatically.
    /// </summary>
    void RefreshSelectionState();
}