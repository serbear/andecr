using ReactiveUI;
using System.Reactive;

namespace andecr.ViewModels;

public class EnglishDeckViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainVm;

    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

    public EnglishDeckViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;

        GoBackCommand = ReactiveCommand.Create(() =>
        {
            _mainVm.CurrentScreen = new DeckSelectionViewModel(_mainVm);
        });
    }
}
