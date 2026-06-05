using ReactiveUI;

namespace andecr.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _greeting = "Добро пожаловать в andecr!";

    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);
    }
}
