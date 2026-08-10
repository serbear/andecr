using System.Collections.ObjectModel;

namespace andecr.ViewModels;

/// <summary>
/// Contract for a screen ViewModel that intends to display its own set of items in the shared Left Menu of the main
/// window (SideMenuView).
/// </summary>
public interface IHasLeftMenu
{
    /// <summary>
    /// Gets the menu items for this screen. 
    /// The collection must persist for the entire lifetime of the screen; 
    /// state changes (enabled/active) should be made by mutating individual items rather than replacing the collection.
    /// </summary>
    ObservableCollection<MenuItemViewModel> MenuItems { get; }
}