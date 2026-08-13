using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using ReactiveUI;

namespace andecr.ViewModels.Controls;

/// <summary>
/// ViewModel for managing a collection of string markers with add/remove operations.
/// </summary>
/// <remarks>
/// Maintains an observable collection of selected markers and exposes:
/// <list type="bullet">
/// <item><description>Add/remove operations via commands and methods</description></item>
/// <item><description>A comma-separated string representation for data binding</description></item>
/// <item><description>Collection change notifications to sync with the view</description></item>
/// </list>
/// </remarks>
public class MarkerListViewModel : ReactiveObject, IMarkerListViewModel
{
    private string _markersString = string.Empty;
    private string? _selectedMarkerToAdd;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkerListViewModel"/> class.
    /// </summary>
    /// <remarks>
    /// Subscribes to <see cref="SelectedMarkers"/> collection changes to update <see cref="MarkersString"/>.
    /// Initializes <see cref="AddMarkerCommand"/>.
    /// </remarks>
    public MarkerListViewModel()
    {
        SelectedMarkers.CollectionChanged += OnSelectedMarkersChanged;

        AddMarkerCommand = ReactiveCommand.Create(() =>
        {
            var marker = SelectedMarkerToAdd;
            if (string.IsNullOrWhiteSpace(marker) || SelectedMarkers.Contains(marker))
                return;

            SelectedMarkers.Add(marker);
        });
    }

    /// <inheritdoc/>
    public ObservableCollection<string> SelectedMarkers { get; } = [];

    /// <inheritdoc/>
    public string MarkersString
    {
        get => _markersString;
        private set => this.RaiseAndSetIfChanged(ref _markersString, value);
    }

    /// <inheritdoc/>
    public string? SelectedMarkerToAdd
    {
        get => _selectedMarkerToAdd;
        set => this.RaiseAndSetIfChanged(ref _selectedMarkerToAdd, value);
    }

    /// <inheritdoc/>
    public ReactiveCommand<Unit, Unit> AddMarkerCommand { get; }

    /// <summary>
    /// Removes the specified marker from the selected markers' collection.
    /// </summary>
    /// <param name="marker">The marker string to remove. Must not be null.</param>
    /// <remarks>
    /// Called by the view when a marker chip's remove button is clicked.
    /// No-op if the marker is not present in the collection.
    /// </remarks>
    public void RemoveMarker(string marker)
    {
        SelectedMarkers.Remove(marker);
    }

    /// <summary>
    /// Handles changes to the <see cref="SelectedMarkers"/> collection.
    /// </summary>
    /// <param name="sender">The source collection.</param>
    /// <param name="e">The event data containing the collection change details.</param>
    /// <remarks>
    /// Updates <see cref="MarkersString"/> to reflect the current state of the collection.
    /// </remarks>
    private void OnSelectedMarkersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        MarkersString = string.Join(",", SelectedMarkers);
    }
}