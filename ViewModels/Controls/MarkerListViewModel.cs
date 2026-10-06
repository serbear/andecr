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
    private const string MarkerSeparator = ", ";

    /// <summary>
    /// The full pool of markers available to this control, in the order they should be displayed.
    /// Used to restore removed markers to <see cref="AvailableMarkers"/> at the correct position.
    /// </summary>
    private readonly List<string> _markerOrder = [];

    /// <summary>Backing field for <see cref="MarkersString"/>.</summary>
    private string _markersString = string.Empty;

    /// <summary>
    /// Lookup of marker to its position in <see cref="_markerOrder"/>.
    /// Used as the sort key when restoring a marker to <see cref="AvailableMarkers"/>, so the correct
    /// insertion point is found with a binary search instead of repeated <see cref="List{T}.IndexOf(T)"/> calls.
    /// </summary>
    /// <remarks>
    /// Rebuilt in <see cref="SetAvailableMarkers"/> whenever the pool changes. If the pool contains duplicates,
    /// the first occurrence determines the marker's position. Markers absent from this dictionary are not part
    /// of the known order and are appended to the end of <see cref="AvailableMarkers"/>.
    /// </remarks>
    private readonly Dictionary<string, int> _orderIndex = new();

    /// <summary>Backing field for <see cref="SelectedMarkerToAdd"/>.</summary> 
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
            AvailableMarkers.Remove(marker);

            // Clear the dropdown selection since the previously selected item is no longer available.
            SelectedMarkerToAdd = null;
        });
        ClearMarkersCommand = ReactiveCommand.Create(ClearMarkers);
    }

    /// <inheritdoc/>
    public ReactiveCommand<Unit, Unit> ClearMarkersCommand { get; }

    /// <inheritdoc/>
    public ObservableCollection<string> SelectedMarkers { get; } = [];

    /// <inheritdoc/>
    public ObservableCollection<string> AvailableMarkers { get; } = [];

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

    /// <inheritdoc/>
    public void SetAvailableMarkers(IEnumerable<string> markers)
    {
        _markerOrder.Clear();
        _markerOrder.AddRange(markers);

        // Update the order index of markers
        _orderIndex.Clear();
        for (var i = 0; i < _markerOrder.Count; i++)
        {
            _orderIndex.TryAdd(_markerOrder[i], i);
        }

        RebuildAvailableMarkers();
    }

    /// <summary>
    /// Repopulates <see cref="AvailableMarkers"/> from <see cref="_markerOrder"/>, skipping selected markers.
    /// Keeps the original display order without reloading the source list.
    /// </summary>
    private void RebuildAvailableMarkers()
    {
        var selected = SelectedMarkers.ToHashSet();
        AvailableMarkers.Clear();
        foreach (var marker in _markerOrder.Where(marker => !selected.Contains(marker)))
        {
            AvailableMarkers.Add(marker);
        }
    }


    /// <summary>
    /// Clears all currently selected markers, returning them to the "add marker" dropdown.
    /// </summary>
    /// <remarks>
    /// Executes the internal view model's <see cref="MarkerListViewModel.ClearMarkersCommand"/>.
    /// Intended to be called from the code-behind of the parent view (e.g. in response to an
    /// interaction raised by the owning page/deck view model), since the control's internal
    /// <see cref="MarkerListViewModel"/> instance is not otherwise externally accessible.
    /// </remarks>
    private void ClearMarkers()
    {
        if (SelectedMarkers.Count == 0)
        {
            return;
        }

        SelectedMarkers.Clear();
        RebuildAvailableMarkers();
    }

    /// <summary>
    /// Removes the specified marker from the selected markers' collection.
    /// </summary>
    /// <param name="marker">The marker string to remove. Must not be null.</param>
    /// <remarks>
    /// Called by the view when a marker chip's remove button is clicked.
    /// No-op if the marker is not present in the collection. If removed, the marker is restored to
    /// <see cref="AvailableMarkers"/> at its original position.
    /// </remarks>
    public void RemoveMarker(string marker)
    {
        if (SelectedMarkers.Remove(marker))
        {
            RestoreAvailableMarker(marker);
        }
    }

    /// <summary>
    /// Re-inserts a marker into <see cref="AvailableMarkers"/> at the position matching its place in
    /// <see cref="_markerOrder"/>.
    /// </summary>
    /// <param name="marker">The marker to restore.</param>
    /// <remarks>
    /// Falls back to appending at the end if the marker is not part of the known marker order
    /// (e.g. it was added dynamically and never registered via <see cref="SetAvailableMarkers"/>).
    /// </remarks>
    private void RestoreAvailableMarker(string marker)
    {
        if (!_orderIndex.TryGetValue(marker, out var orderIndex))
        {
            AvailableMarkers.Add(marker);
            return;
        }

        int lo = 0, hi = AvailableMarkers.Count;
        while (lo < hi)
        {
            var mid = (lo + hi) / 2;
            if (_orderIndex.TryGetValue(AvailableMarkers[mid], out var midIndex) && midIndex <= orderIndex)
            {
                lo = mid + 1;
            }
            else
            {
                hi = mid;
            }
        }

        AvailableMarkers.Insert(lo, marker);
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
        MarkersString = string.Join(
            MarkerSeparator,
            SelectedMarkers
        );
    }
}