using ReactiveUI;
using System.Reactive;

namespace andecr.ViewModels;

public class DeckSelectionViewModel : ViewModelBase
{
    private readonly MainWindowViewModel _mainVm;

    public ReactiveCommand<Unit, Unit> OpenEnglishCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenEstonianCommand { get; }

    public DeckSelectionViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;

        OpenEnglishCommand = ReactiveCommand.Create(() =>
        {
            _mainVm.CurrentScreen = new EnglishDeckViewModel(_mainVm);
        });

        OpenEstonianCommand = ReactiveCommand.Create(() =>
        {
            _mainVm.CurrentScreen = new EstonianDeckViewModel(_mainVm);
        });
    }
}
