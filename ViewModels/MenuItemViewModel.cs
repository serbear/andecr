using System.Windows.Input;
using ReactiveUI;

namespace andecr.ViewModels;

/// <summary>
/// Represents a single item in a dynamic screen menu, containing an icon, header, command,
/// and two independent state bindings — IsEnabled / IsActive. Screens create a list of these items
/// in their constructors (see IHasRightMenu). The right menu (RightMenuView) has no knowledge
/// of specific screens and simply renders the provided items.
/// </summary>
public class MenuItemViewModel : ReactiveObject
{
    /// <summary>
    /// Gets or sets the icon or emoji displayed to the left of the header label.
    /// </summary>
    public required string Icon { get; init; }

    /// <summary>
    /// Gets or sets the header label for the menu item.
    /// </summary>
    public required string Header { get; init; }

    /// <summary>
    /// Gets or sets the tooltip text displayed on hover.
    /// </summary>
    public string? ToolTip { get; init; }

    /// <summary>
    /// Gets or sets the command executed when the menu item is clicked.
    /// </summary>
    public ICommand? Command { get; init; }

    /// <summary>
    /// Backing field for <see cref="IsEnabled"/>.
    /// </summary>
    private bool _isEnabled = true;

    /// <summary>
    /// Gets or sets a value indicating whether the menu item is enabled.
    /// The owner screen can update this state at any time (e.g., disabling "Save" until a form is complete)
    /// to automatically refresh the UI.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

    /// <summary>
    /// Backing field for <see cref="IsActive"/>.
    /// </summary>
    private bool _isActive;

    /// <summary>
    /// Gets or sets a value indicating whether this item represents the active sub-screen, used for UI highlighting
    /// (e.g., highlighting "Editor" while the editor sub-screen is active).
    /// Optional for action-based items like "Save" or "Exit".
    /// </summary>
    public bool IsActive
    {
        get => _isActive;
        set => this.RaiseAndSetIfChanged(ref _isActive, value);
    }
}