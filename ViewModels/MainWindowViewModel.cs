using System.Collections.ObjectModel;
using ReactiveUI;


namespace andecr.ViewModels;

/// <summary>
/// View model for the main window, responsible for managing the active screen
/// and dynamically updating navigation menus based on the current screen capabilities.
/// </summary>
public class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Backing field for <see cref="CurrentScreen"/>.
    /// </summary>
    private ViewModelBase _currentScreen = null!;

    /// <summary>
    /// Gets or sets the currently active screen view model.
    /// </summary>
    public ViewModelBase CurrentScreen
    {
        get => _currentScreen;
        set => this.RaiseAndSetIfChanged(ref _currentScreen, value);
    }

    /// <summary>
    /// Backing field for <see cref="RightMenuItems"/>.
    /// </summary>
    private ObservableCollection<MenuItemViewModel>? _rightMenuItems;

    /// <summary>
    /// Gets the collection of right menu items for the current screen.
    /// Automatically recalculated whenever <see cref="CurrentScreen"/> changes:
    /// if the new screen implements <see cref="IHasRightMenu"/>, its MenuItems are used;
    /// otherwise, it is set to <see langword="null"/> and the RightMenuView hides itself.
    /// </summary>
    public ObservableCollection<MenuItemViewModel>? RightMenuItems
    {
        get => _rightMenuItems;
        private set => this.RaiseAndSetIfChanged(ref _rightMenuItems, value);
    }

    /// <summary>
    /// Backing field for <see cref="LeftMenuItems"/>.
    /// </summary>
    private ObservableCollection<MenuItemViewModel>? _leftMenuItems;

    /// <summary>
    /// Gets the collection of left menu items for the current screen.
    /// Automatically recalculated whenever <see cref="CurrentScreen"/> changes:
    /// if the new screen implements <see cref="IHasLeftMenu"/>, its MenuItems are used;
    /// otherwise, it is set to <see langword="null"/>.
    /// </summary>
    public ObservableCollection<MenuItemViewModel>? LeftMenuItems
    {
        get => _leftMenuItems;
        private set => this.RaiseAndSetIfChanged(ref _leftMenuItems, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class,
    /// sets up subscriptions to track <see cref="CurrentScreen"/> changes, and sets the initial screen.
    /// </summary>
    public MainWindowViewModel()
    {
        this.WhenAnyValue(x => x.CurrentScreen)
            .Subscribe(screen =>
            {
                RightMenuItems = (screen as IHasRightMenu)?.MenuItems;
                LeftMenuItems = (screen as IHasLeftMenu)?.MenuItems;
            });

        // Важно: присваивать через свойство, а не через приватное поле —
        // иначе подписка выше не увидит первого экрана и RightMenuItems
        // не проставится при старте приложения.
        CurrentScreen = new DeckSelectionViewModel(this);
    }
}