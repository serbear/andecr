using System.Reactive;
using andecr.Controls;
using ReactiveUI;

namespace andecr.ViewModels;

/// <summary>
/// Common contract for language deck editor ViewModels (Estonian, English, and subsequent ones).
/// Implementing this interface automatically provides support for the shared side menu <see cref="LeftMenuView"/>.
/// </summary>
public interface IDeckEditorViewModel
{
    /// <summary>
    /// Gets the command to exit the application or current section.
    /// </summary>
    ReactiveCommand<Unit, Unit> ExitCommand { get; }

    /// <summary>
    /// Gets the command to open the configuration/settings screen.
    /// </summary> 
    ReactiveCommand<Unit, Unit> ConfigCommand { get; }

    /// <summary>
    /// Gets the command to navigate to the deck selection screen.
    /// </summary>
    ReactiveCommand<Unit, Unit> GoToDecksCommand { get; }

    /// <summary>
    /// Gets the command to switch to the main editor view.
    /// </summary> 
    ReactiveCommand<Unit, Unit> EditorCommand { get; }

    /// <summary>
    /// Gets the command to create a new card by resetting the editor to its initial state.
    /// </summary>
    ReactiveCommand<Unit, Unit> NewCardCommand { get; }

    /// <summary>
    /// Gets the command to save the currently edited card to the active deck.
    /// </summary>
    ReactiveCommand<Unit, Unit> SaveCardCommand { get; }

    /// <summary>
    /// Gets a value indicating whether the editor screen is currently active.
    /// Used for toggle states and UI highlighting.
    /// </summary>
    bool IsEditorActive { get; }

    /// <summary>
    /// Gets a value indicating whether the configuration screen is currently active.
    /// Used for toggle states and UI highlighting.
    /// </summary> 
    bool IsConfigActive { get; }
}