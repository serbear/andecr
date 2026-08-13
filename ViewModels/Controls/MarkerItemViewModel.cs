using System.Reactive;
using ReactiveUI;

namespace andecr.ViewModels.Controls;

/// <summary>
/// ViewModel for an individual marker chip control.
/// </summary>
/// <remarks>
/// Provides a remove command that signals the parent view model to delete this marker from the collection.
/// </remarks>
public class MarkerItemViewModel : ReactiveObject, IMarkerItemViewModel
{
    ///  <inheritdoc/>
    public ReactiveCommand<Unit, Unit> RemoveMarkerCommand { get; } = ReactiveCommand.Create(() => { });
}