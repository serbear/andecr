namespace andecr.ViewModels.Controls;

using System.Reactive;
using ReactiveUI;

/// <summary>
/// Defines the view model contract for the custom text box control, providing clipboard operations and state.
/// </summary>
public interface IAndecrTextBoxViewModel
{
    /// <summary>
    /// Gets a value indicating whether non-empty text is currently available in the clipboard to be pasted.
    /// </summary>
    bool CanPasteText { get; }
    
    /// <summary>
    /// Gets the command that pastes text from the clipboard into the control.
    /// </summary>
    ReactiveCommand<Unit, Unit> PasteTextCommand { get; }
}