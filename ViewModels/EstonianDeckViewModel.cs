using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using andecr.Models;

namespace andecr.ViewModels;

public class EstonianDeckViewModel : ViewModelBase, IDeckEditorViewModel
{
    private readonly MainWindowViewModel _mainVm;

    private string _word = string.Empty;
    public string Word
    {
        get => _word;
        set => this.RaiseAndSetIfChanged(ref _word, value);
    }

    private string _translation = string.Empty;
    public string Translation
    {
        get => _translation;
        set => this.RaiseAndSetIfChanged(ref _translation, value);
    }

    /// <summary>Карточки, уже сохранённые в текущей колоде.</summary>
    public ObservableCollection<EnglishDictionaryCard> Cards { get; } = new();

    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }
    public ReactiveCommand<Unit, Unit> EditorCommand { get; }
    public ReactiveCommand<Unit, Unit> NewCardCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCardCommand { get; }
    public bool IsEditorActive => true;
    public bool IsConfigActive => false;
    public ReactiveCommand<Unit, Unit> GoToDecksCommand => GoBackCommand;
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    public ReactiveCommand<Unit, Unit> ConfigCommand { get; }

    public EstonianDeckViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;

        GoBackCommand = ReactiveCommand.Create(() =>
        {
            _mainVm.CurrentScreen = new DeckSelectionViewModel(_mainVm);
        });

        NewCardCommand = ReactiveCommand.Create(ResetEditor);

        SaveCardCommand = ReactiveCommand.Create(() =>
        {
            if (string.IsNullOrWhiteSpace(Word))
                return;

            Cards.Add(new EnglishDictionaryCard
            {
                DictionaryEntry = Word.Trim(),
                LiteraryTranslation = Translation.Trim()
            });

            ResetEditor();
        });

        ExitCommand = ReactiveCommand.Create(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        });
    }

    /// <summary>Сбрасывает все элементы управления редактора карточки в исходное состояние.</summary>
    private void ResetEditor()
    {
        Word = string.Empty;
        Translation = string.Empty;
    }
}
