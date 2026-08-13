using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace andecr.ViewModels.Controls;

/// <summary>
/// Defines the contract for a view model that manages a collection of markers.
/// </summary>
/// <remarks>
/// Implementations must provide marker collection management with add/remove operations and string representation
/// for data binding.
/// </remarks>
public interface IMarkerListViewModel
{
    /// <summary>
    /// Gets the observable collection of currently selected markers.
    /// </summary>
    /// <value>A collection of marker strings.</value>
    ObservableCollection<string> SelectedMarkers { get; }

    /// <summary>
    /// Gets the observable collection of markers available for selection (i.e. not yet added to
    /// <see cref="SelectedMarkers"/>).
    /// </summary>
    /// <value>A collection of marker strings not currently selected.</value>
    /// <remarks>
    /// Intended as the binding source for the "add marker" dropdown. Automatically kept in sync:
    /// a marker is removed from this collection when added to <see cref="SelectedMarkers"/>, and
    /// restored when removed from it.
    /// </remarks>
    ObservableCollection<string> AvailableMarkers { get; }

    /// <summary>
    /// Replaces the full pool of markers that can be selected.
    /// </summary>
    /// <param name="markers">The complete set of markers available to this control, in display order.</param>
    /// <remarks>
    /// Resets <see cref="AvailableMarkers"/> to the given set, excluding any markers already present in
    /// <see cref="SelectedMarkers"/>. Call this whenever the source list of markers changes.
    /// </remarks>
    void SetAvailableMarkers(IEnumerable<string> markers);
    
    /// <summary>
    /// Gets the current markers as a comma-separated string.
    /// </summary>
    /// <value>A string containing all markers joined by commas, or empty string if none.</value>
    /// <remarks>
    /// Automatically updated when <see cref="SelectedMarkers"/> changes.
    /// Used for one-way-to-source binding to the parent view.
    /// </remarks> 
    string MarkersString { get; }
    
    /// <summary>
    /// Gets or sets the marker selected in the dropdown list for addition.
    /// </summary>
    /// <value>The marker string to add, or null if none selected.</value>
    /// <remarks>
    /// Raising this property triggers the view to update the dropdown selection.
    /// </remarks> 
    string? SelectedMarkerToAdd { get; set; }
    
    /// <summary>
    /// Gets the command that adds the currently selected marker to the list.
    /// </summary>
    /// <value>A reactive command that executes the add operation.</value>
    /// <remarks>
    /// Execution is skipped if <see cref="SelectedMarkerToAdd"/> is null, whitespace, or already present
    /// in <see cref="SelectedMarkers"/>.
    /// </remarks> 
    ReactiveCommand<Unit, Unit> AddMarkerCommand { get; }
}