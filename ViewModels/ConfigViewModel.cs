using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace andecr.ViewModels;

/// <summary>
/// View model for the settings/configuration screen.
/// </summary>
public class ConfigViewModel : ViewModelBase, IDeckEditorViewModel, IHasRightMenu, IHasLeftMenu
{
    /// <summary>
    /// Reference to the main window view model used for navigation.
    /// </summary>
    private readonly MainWindowViewModel _mainVm;

    /// <summary>
    /// Active screen flags.
    /// Gets a value indicating whether the editor screen is currently active.
    /// </summary>
    public bool IsEditorActive => false;

    /// <summary>
    /// Gets a value indicating whether the configuration screen is currently active.
    /// </summary> 
    public bool IsConfigActive => true;

    /// <summary>
    /// Command to navigate to the editor screen.
    /// </summary>
    public ReactiveCommand<Unit, Unit> EditorCommand { get; }

    /// <summary>
    /// Command to navigate to the configuration screen.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ConfigCommand { get; }

    /// <summary>
    /// Command to navigate to the deck selection screen.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoToDecksCommand { get; }

    /// <summary>
    /// Command to exit the application.
    /// </summary>
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    /// <summary>
    /// Command to create a new vocabulary card.
    /// </summary>
    public ReactiveCommand<Unit, Unit> NewCardCommand { get; }

    /// <summary>
    /// Command to save the current vocabulary card.
    /// </summary> 
    public ReactiveCommand<Unit, Unit> SaveCardCommand { get; }

    /// <summary>
    /// Left menu items.
    /// </summary>
    private readonly ObservableCollection<MenuItemViewModel> _leftMenuItems = [];

    /// <summary>
    /// Gets the collection of left menu items.
    /// </summary>
    ObservableCollection<MenuItemViewModel> IHasLeftMenu.MenuItems => _leftMenuItems;

    /// <summary>
    /// Right menu of the settings screen.
    /// </summary>
    public ObservableCollection<MenuItemViewModel> MenuItems { get; } = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigViewModel"/> class.
    /// </summary>
    /// <param name="mainVm">The main window view model.</param>
    public ConfigViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;

        // Return to the editor screen (e.g., EnglishDeckViewModel)
        EditorCommand = ReactiveCommand.Create(() => { _mainVm.CurrentScreen = new EnglishDeckViewModel(_mainVm); });

        // Settings button: already on the settings screen
        ConfigCommand = ReactiveCommand.Create(() => { });

        GoToDecksCommand =
            ReactiveCommand.Create(() => { _mainVm.CurrentScreen = new DeckSelectionViewModel(_mainVm); });

        ExitCommand = ReactiveCommand.Create(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        });

        NewCardCommand = ReactiveCommand.Create(() => { });
        SaveCardCommand = ReactiveCommand.Create(() => { });

        var exportCommand = ReactiveCommand.Create(() =>
        {
            // TODO: actual settings export — this is just a stub showing 
            // that a screen can have its own unique menu items.
        });

        MenuItems.Add(new MenuItemViewModel
        {
            Icon = "📤",
            Header = "Экспорт",
            ToolTip = "[DEBUG] Экспортировать настройки",
            Command = exportCommand
        });

        _leftMenuItems.Add(new MenuItemViewModel
        {
            Icon = "[E]",
            Header = "Редактор",
            ToolTip = "Редактор колоды",
            Command = EditorCommand,
            IsActive = IsEditorActive
        });

        _leftMenuItems.Add(new MenuItemViewModel
        {
            Icon = "[X]",
            Header = "Настройки",
            ToolTip = "Перейти на экран редактирования настроек",
            Command = ConfigCommand,
            IsActive = IsConfigActive
        });

        _leftMenuItems.Add(new MenuItemViewModel
        {
            Icon = "📚",
            Header = "Колоды",
            ToolTip = "К выбору колоды",
            Command = GoToDecksCommand
        });

        _leftMenuItems.Add(new MenuItemViewModel
        {
            Icon = "🚪",
            Header = "Выход",
            ToolTip = "Выйти из приложения",
            Command = ExitCommand
        });
    }
}