using ReactiveUI;

namespace andecr.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentScreen;

    public ViewModelBase CurrentScreen
    {
        get => _currentScreen;
        set => this.RaiseAndSetIfChanged(ref _currentScreen, value);
    }

    public MainWindowViewModel()
    {
        _currentScreen = new DeckSelectionViewModel(this);
    }
}
