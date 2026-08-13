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