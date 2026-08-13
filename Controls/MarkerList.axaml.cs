using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using andecr.ViewModels.Controls;

namespace andecr.Controls;

/// <summary>
/// A user control that displays a list of marker chips with add/remove capabilities.
/// </summary>
/// <remarks>
/// The control consists of:
/// <list type="bullet">
/// <item><description>A dropdown for selecting markers to add</description></item>
/// <item><description>An add button</description></item>
/// <item><description>A panel displaying marker chips with remove buttons</description></item>
/// </list>
/// The control internally manages its own <see cref="MarkerListViewModel"/> and synchronizes with parent data context
/// via bindable properties.
/// </remarks>
public partial class MarkerList : UserControl
{
    /// <summary>
    /// Identifies the <see cref="MarkerItems"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<ObservableCollection<string>> MarkerItemsProperty =
        AvaloniaProperty.Register<MarkerList, ObservableCollection<string>>("MarkerItems");
    
    /// <summary>
    /// Identifies the <see cref="MarkersString"/> dependency property.
    /// </summary>
    /// <remarks>The default value is an empty string.</remarks>
    public static readonly StyledProperty<string> MarkersStringProperty =
        AvaloniaProperty.Register<MarkerList, string>(
            nameof(MarkersString),
            defaultValue: string.Empty,
            defaultBindingMode: BindingMode.OneWayToSource);

    private readonly Dictionary<string, MarkerItem> _markerChips = new();

    private readonly MarkerListViewModel _viewModel;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkerList"/> control.
    /// </summary>
    /// <remarks>
    /// Creates the internal view model, subscribes to collection changes, and sets the DataContext for the internal
    /// root element.
    /// </remarks>
    public MarkerList()
    {
        InitializeComponent();

        _viewModel = new MarkerListViewModel();
        _viewModel.SelectedMarkers.CollectionChanged += OnSelectedMarkersChanged;

        InternalRoot.DataContext = _viewModel;
    }
    
    /// <summary>
    /// Gets or sets the collection of marker strings displayed by the control.
    /// </summary>
    /// <value>An observable collection of marker strings.</value>
    /// <remarks>
    /// The control does not directly modify this collection.
    /// It maintains its own internal collection synchronized with the view model.
    /// </remarks>
    public ObservableCollection<string> MarkerItems
    {
        get => GetValue(MarkerItemsProperty);
        set => SetValue(MarkerItemsProperty, value);
    }

    /// <summary>
    /// Gets or sets the comma-separated string representation of markers.
    /// </summary>
    /// <value>A string with markers joined by commas.</value>
    /// <remarks>
    /// One-way-to-source binding mode. The control updates this property whenever the internal marker
    /// collection changes.
    /// </remarks>
    public string MarkersString
    {
        get => GetValue(MarkersStringProperty);
        set => SetValue(MarkersStringProperty, value);
    }
    
    /// <summary>
    /// Handles changes to the internal marker collection.
    /// </summary>
    /// <param name="sender">The source collection.</param>
    /// <param name="e">The collection change event data.</param>
    /// <remarks>
    /// Updates <see cref="MarkersString"/> and manages the visual marker chips by adding, removing, or resetting them
    /// as needed.
    /// </remarks>
    private void OnSelectedMarkersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SetValue(MarkersStringProperty, _viewModel.MarkersString);

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add when e.NewItems is not null:
                foreach (string marker in e.NewItems)
                    AddMarkerChip(marker);
                break;

            case NotifyCollectionChangedAction.Remove when e.OldItems is not null:
                foreach (string marker in e.OldItems)
                    RemoveMarkerChip(marker);
                break;

            case NotifyCollectionChangedAction.Reset:
                SelectedMarkersPanel.Children.Clear();
                _markerChips.Clear();
                break;
        }
    }

    /// <summary>
    /// Adds a visual marker chip for the specified marker string.
    /// </summary>
    /// <param name="marker">The marker to display as a chip.</param>
    /// <remarks>
    /// Creates a <see cref="MarkerItem"/> control with the marker text and hooks up the remove request event
    /// to call <see cref="MarkerListViewModel.RemoveMarker"/>.
    /// Does nothing if a chip for this marker already exists.
    /// </remarks>
    private void AddMarkerChip(string marker)
    {
        // Do not add a duplicate chip in the list.
        if (_markerChips.ContainsKey(marker))
            return;

        var markerItem = new MarkerItem { Text = marker };
        markerItem.RemoveRequested += (_, _) => _viewModel.RemoveMarker(marker);

        _markerChips[marker] = markerItem;
        SelectedMarkersPanel.Children.Add(markerItem);
    }

    /// <summary>
    /// Removes the visual marker chip for the specified marker string.
    /// </summary>
    /// <param name="marker">The marker whose chip should be removed.</param>
    /// <remarks>
    /// Removes the chip from the visual panel and disposes of event handlers.
    /// Does nothing if no chip exists for the specified marker.
    /// </remarks>
    private void RemoveMarkerChip(string marker)
    {
        if (!_markerChips.Remove(marker, out var markerItem))
            return;

        SelectedMarkersPanel.Children.Remove(markerItem);
    }
}