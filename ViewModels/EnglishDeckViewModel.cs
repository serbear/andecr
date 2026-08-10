using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using andecr.Models;
using andecr.Services;

namespace andecr.ViewModels;

public class EnglishDeckViewModel : ViewModelBase, IDeckEditorViewModel
{
    /// <summary>Фиксированный список уровней CEFR (не загружается из файла).</summary>
    private static readonly IReadOnlyList<string> CefrLevelValues = new[]
    {
        "A1", "A2", "B1", "B2", "C1", "C2"
    };

    private readonly MainWindowViewModel _mainVm;

    private string _definition = string.Empty;
    /// <summary>Определение — описание изучаемого слова или выражения из словаря.</summary>
    public string Definition
    {
        get => _definition;
        set => this.RaiseAndSetIfChanged(ref _definition, value);
    }

    private string _tag = string.Empty;
    /// <summary>Тег — смысловая область слова, из фиксированного списка.</summary>
    public string Tag
    {
        get => _tag;
        set => this.RaiseAndSetIfChanged(ref _tag, value);
    }

    private string _dictionaryEntry = string.Empty;
    /// <summary>Словарная статья — название карточки в списке Anki.</summary>
    public string DictionaryEntry
    {
        get => _dictionaryEntry;
        set => this.RaiseAndSetIfChanged(ref _dictionaryEntry, value);
    }

    private string _literalTranslation = string.Empty;
    /// <summary>Дословный перевод контекста использования слова.</summary>
    public string LiteralTranslation
    {
        get => _literalTranslation;
        set => this.RaiseAndSetIfChanged(ref _literalTranslation, value);
    }

    private string _original = string.Empty;
    /// <summary>Оригинал — контекст использования слова на изучаемом языке.</summary>
    public string Original
    {
        get => _original;
        set => this.RaiseAndSetIfChanged(ref _original, value);
    }

    private string _literaryTranslation = string.Empty;
    /// <summary>Литературный перевод — смысловой перевод оригинала.</summary>
    public string LiteraryTranslation
    {
        get => _literaryTranslation;
        set => this.RaiseAndSetIfChanged(ref _literaryTranslation, value);
    }

    private string _partOfSpeech = string.Empty;
    /// <summary>Часть речи, из фиксированного списка.</summary>
    public string PartOfSpeech
    {
        get => _partOfSpeech;
        set => this.RaiseAndSetIfChanged(ref _partOfSpeech, value);
    }

    private string _britishTranscription = string.Empty;
    /// <summary>Транскрипция британская.</summary>
    public string BritishTranscription
    {
        get => _britishTranscription;
        set => this.RaiseAndSetIfChanged(ref _britishTranscription, value);
    }

    private string _americanTranscription = string.Empty;
    /// <summary>Транскрипция американская.</summary>
    public string AmericanTranscription
    {
        get => _americanTranscription;
        set => this.RaiseAndSetIfChanged(ref _americanTranscription, value);
    }

    private string _marker = string.Empty;
    /// <summary>Маркер (сленг, устаревшее, формальное и т.п.), из фиксированного списка.</summary>
    public string Marker
    {
        get => _marker;
        set => this.RaiseAndSetIfChanged(ref _marker, value);
    }

    private string _cefrLevel = string.Empty;
    /// <summary>Уровень CEFR (A1–C2).</summary>
    public string CefrLevel
    {
        get => _cefrLevel;
        set => this.RaiseAndSetIfChanged(ref _cefrLevel, value);
    }

    private string _sound = string.Empty;
    /// <summary>Звук — ссылка на звуковой файл озвучки слова.</summary>
    public string Sound
    {
        get => _sound;
        set => this.RaiseAndSetIfChanged(ref _sound, value);
    }

    private string _notes = string.Empty;
    /// <summary>Заметки — произвольные примечания.</summary>
    public string Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    /// <summary>Список допустимых тегов, загружается из Assets/Tags.txt.</summary>
    public ObservableCollection<string> Tags { get; } = new();

    /// <summary>Список допустимых частей речи, загружается из Assets/PartsOfSpeech.txt.</summary>
    public ObservableCollection<string> PartsOfSpeech { get; } = new();

    /// <summary>Список допустимых маркеров, загружается из Assets/Markers.txt.</summary>
    public ObservableCollection<string> Markers { get; } = new();

    /// <summary>Список уровней CEFR: A1, A2, B1, B2, C1, C2.</summary>
    public ObservableCollection<string> CefrLevels { get; } = new(CefrLevelValues);

    /// <summary>Карточки, уже сохранённые в текущей колоде.</summary>
    public ObservableCollection<EnglishDictionaryCard> Cards { get; } = new();


    #region Flasgs

    public bool IsEditorActive => true;
    public bool IsConfigActive => false;

    #endregion

    #region Commands

    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }
    public ReactiveCommand<Unit, Unit> NewCardCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCardCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToDecksCommand => GoBackCommand;
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }
    public ReactiveCommand<Unit, Unit> EditorCommand { get; }
    public ReactiveCommand<Unit, Unit> ConfigCommand { get; }

    #endregion
    

    public EnglishDeckViewModel(MainWindowViewModel mainVm)
    {
        _mainVm = mainVm;

        foreach (var tag in TextOptionListLoader.Load("Assets/tags.txt"))
            Tags.Add(tag);

        foreach (var partOfSpeech in TextOptionListLoader.Load("Assets/parts_of_speech.txt"))
            PartsOfSpeech.Add(partOfSpeech);

        foreach (var marker in TextOptionListLoader.Load("Assets/markers.txt"))
            Markers.Add(marker);
        
        // Кнопка редактора: остаётся на текущем экране редактора
        EditorCommand = ReactiveCommand.Create(() => { });
        
        GoBackCommand = ReactiveCommand.Create(() =>
        {
            _mainVm.CurrentScreen = new DeckSelectionViewModel(_mainVm);
        });

        NewCardCommand = ReactiveCommand.Create(ResetEditor);

        SaveCardCommand = ReactiveCommand.Create(() =>
        {
            if (string.IsNullOrWhiteSpace(DictionaryEntry))
                return;

            Cards.Add(new EnglishDictionaryCard
            {
                Definition = Definition.Trim(),
                Tag = Tag,
                DictionaryEntry = DictionaryEntry.Trim(),
                LiteralTranslation = LiteralTranslation.Trim(),
                Original = Original.Trim(),
                LiteraryTranslation = LiteraryTranslation.Trim(),
                PartOfSpeech = PartOfSpeech,
                BritishTranscription = BritishTranscription.Trim(),
                AmericanTranscription = AmericanTranscription.Trim(),
                Marker = Marker,
                CefrLevel = CefrLevel,
                Sound = Sound.Trim(),
                Notes = Notes.Trim()
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

        ConfigCommand = ReactiveCommand.Create(() => {
            _mainVm.CurrentScreen = new ConfigViewModel(_mainVm);
        });
    }

    /// <summary>Сбрасывает все элементы управления редактора карточки в исходное состояние.</summary>
    private void ResetEditor()
    {
        Definition = string.Empty;
        Tag = string.Empty;
        DictionaryEntry = string.Empty;
        LiteralTranslation = string.Empty;
        Original = string.Empty;
        LiteraryTranslation = string.Empty;
        PartOfSpeech = string.Empty;
        BritishTranscription = string.Empty;
        AmericanTranscription = string.Empty;
        Marker = string.Empty;
        CefrLevel = string.Empty;
        Sound = string.Empty;
        Notes = string.Empty;
    }
}
