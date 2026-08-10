using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace andecr.ViewModels;

public class ConfigViewModel: ViewModelBase, IDeckEditorViewModel
{
    private readonly MainWindowViewModel _mainVm;

    // Флаги активного экрана
    public bool IsEditorActive => false;
    public bool IsConfigActive => true;

    public ReactiveCommand<Unit, Unit> EditorCommand { get; }
    public ReactiveCommand<Unit, Unit> ConfigCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToDecksCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    // Заглушки команд карточки (не используются на экране настроек)
    public ReactiveCommand<Unit, Unit> NewCardCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCardCommand { get; }

    public ConfigViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;

        // Возврат на экран редактора (например, EnglishDeckViewModel)
        EditorCommand = ReactiveCommand.Create(() => {
            _mainVm.CurrentScreen = new EnglishDeckViewModel(_mainVm);
        });

        // Кнопка настроек: уже на экране настроек
        ConfigCommand = ReactiveCommand.Create(() => { });

        GoToDecksCommand = ReactiveCommand.Create(() => {
            _mainVm.CurrentScreen = new DeckSelectionViewModel(_mainVm);
        });

        ExitCommand = ReactiveCommand.Create(() => {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        });

        NewCardCommand = ReactiveCommand.Create(() => { });
        SaveCardCommand = ReactiveCommand.Create(() => { });
    }

}