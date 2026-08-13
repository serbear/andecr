using System.Reactive;
using ReactiveUI;

namespace andecr.ViewModels.Controls;

/// <summary>
/// Defines the contract for a view model representing a single marker item.
/// </summary>
public interface IMarkerItemViewModel
{
    /// <summary>
    /// Gets the command that requests removal of this marker.
    /// </summary>
    /// <value>A reactive command that executes the removal request.</value>
    /// <remarks>
    /// The command executes an empty action; actual removal is handled by the parent <see cref="MarkerList"/> control
    /// subscribing to the <see cref="MarkerItem.RemoveRequested"/> event.
    /// </remarks>
    ReactiveCommand<Unit, Unit> RemoveMarkerCommand { get; }
}