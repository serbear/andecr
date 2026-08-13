using andecr.ViewModels.Controls;
using Avalonia;
using Avalonia.Controls;

namespace andecr.Controls;

/// <summary>
/// A user control that displays a single marker chip with a remove button.
/// </summary>
/// <remarks>
/// The control consists of a text label and a close button that raises the <see cref="RemoveRequested"/> event
/// when clicked.
/// </remarks>
public partial class MarkerItem : UserControl
{
    /// <summary>
    /// Identifies the <see cref="Text"/> dependency property.
    /// </summary>
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MarkerItem, string>("Text");

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkerItem"/> control.
    /// </summary>
    /// <remarks>
    /// Creates the view model, sets the DataContext, and subscribes
    /// to <see cref="MarkerItemViewModel.RemoveMarkerCommand"/> to raise the <see cref="RemoveRequested"/> event.
    /// </remarks>
    public MarkerItem()
    {
        InitializeComponent();
        ViewModel = new MarkerItemViewModel();
        InternalRoot.DataContext = ViewModel;
        ViewModel.RemoveMarkerCommand.Subscribe(_ => RemoveRequested?.Invoke(this, EventArgs.Empty));
    }

    /// <summary>
    /// Gets the internal view model for this marker item.
    /// </summary>
    /// <value>A <see cref="MarkerItemViewModel"/> instance.</value>
    private MarkerItemViewModel ViewModel { get; }

    /// <summary>
    /// Gets or sets the text displayed on the marker chip.
    /// </summary>
    /// <value>The marker text string.</value>
    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Occurs when the user requests removal of this marker.
    /// </summary>
    /// <remarks>
    /// Raised when the remove button is clicked. The parent control subscribes to this event to handle
    /// the actual removal.
    /// </remarks>
    public event EventHandler? RemoveRequested;
}