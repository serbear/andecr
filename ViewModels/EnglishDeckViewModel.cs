using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using andecr.Models;
using andecr.Services;

namespace andecr.ViewModels;

/// <summary>
/// View model for managing and editing an English deck and its cards.
/// </summary>
public class EnglishDeckViewModel : ViewModelBase, IDeckEditorViewModel, IHasRightMenu, IHasLeftMenu
{

    
    /// <summary>
    /// A fixed list of CEFR levels (not loaded from a file).
    /// </summary>
    private static readonly IReadOnlyList<string> CefrLevelValues =
    [
        "A1", "A2", "B1", "B2", "C1", "C2"
    ];

    /// <summary>
    /// Reference to the main window view model.
    /// </summary>
    private readonly MainWindowViewModel _mainVm;

    /// <summary>
    /// Backing field for the dictionary definition.
    /// </summary>
    private string _definition = string.Empty;

    /// <summary>
    /// Gets or sets the definition describing the target word or expression from the dictionary.
    /// </summary>
    public string Definition
    {
        get => _definition;
        set => this.RaiseAndSetIfChanged(ref _definition, value);
    }

    /// <summary>
    /// Backing field for the semantic domain/tag.
    /// </summary>
    private string _tag = string.Empty;

    /// <summary>
    /// Gets or sets the tag representing the semantic domain of the word selected from a fixed list.
    /// </summary>
    public string Tag
    {
        get => _tag;
        set => this.RaiseAndSetIfChanged(ref _tag, value);
    }

    /// <summary>
    /// Backing field for the dictionary entry title.
    /// </summary>
    private string _dictionaryEntry = string.Empty;

    /// <summary>
    /// Gets or sets the dictionary entry title used as the card title in Anki.
    /// </summary>
    public string DictionaryEntry
    {
        get => _dictionaryEntry;
        set => this.RaiseAndSetIfChanged(ref _dictionaryEntry, value);
    }

    /// <summary>
    /// Backing field for the literal translation.
    /// </summary>
    private string _literalTranslation = string.Empty;

    /// <summary>
    /// Gets or sets the literal translation of the context sentence.
    /// </summary>
    public string LiteralTranslation
    {
        get => _literalTranslation;
        set => this.RaiseAndSetIfChanged(ref _literalTranslation, value);
    }

    /// <summary>
    /// Backing field for the original target language context.
    /// </summary>
    private string _original = string.Empty;

    /// <summary>
    /// Gets or sets the original sentence/context in the target language.
    /// </summary>
    public string Original
    {
        get => _original;
        set => this.RaiseAndSetIfChanged(ref _original, value);
    }

    /// <summary>
    /// Backing field for the literary translation.
    /// </summary>
    private string _literaryTranslation = string.Empty;

    /// <summary>
    /// Gets or sets the natural (literary) translation of the original context.
    /// </summary>
    public string LiteraryTranslation
    {
        get => _literaryTranslation;
        set => this.RaiseAndSetIfChanged(ref _literaryTranslation, value);
    }

    /// <summary>
    /// Backing field for the part of speech.
    /// </summary>
    private string _partOfSpeech = string.Empty;

    /// <summary>
    /// Gets or sets the part of speech selected from a fixed list.
    /// </summary>
    public string PartOfSpeech
    {
        get => _partOfSpeech;
        set => this.RaiseAndSetIfChanged(ref _partOfSpeech, value);
    }

    /// <summary>
    /// Backing field for the British English transcription.
    /// </summary>
    private string _britishTranscription = string.Empty;

    /// <summary>
    /// Gets or sets the British English phonetic transcription.
    /// </summary>
    public string BritishTranscription
    {
        get => _britishTranscription;
        set => this.RaiseAndSetIfChanged(ref _britishTranscription, value);
    }

    /// <summary>
    /// Backing field for the American English transcription.
    /// </summary>
    private string _americanTranscription = string.Empty;

    /// <summary>
    /// Gets or sets the American English phonetic transcription.
    /// </summary>
    public string AmericanTranscription
    {
        get => _americanTranscription;
        set => this.RaiseAndSetIfChanged(ref _americanTranscription, value);
    }

    /// <summary>
    /// Backing field for the usage marker.
    /// </summary>
    private string _marker = string.Empty;

    /// <summary>
    /// Gets or sets the usage marker (e.g., slang, archaic, formal) selected from a fixed list.
    /// </summary>
    public string Marker
    {
        get => _marker;
        set => this.RaiseAndSetIfChanged(ref _marker, value);
    }

    /// <summary>
    /// Backing field for the CEFR language level.
    /// </summary>
    private string _cefrLevel = string.Empty;

    /// <summary>
    /// Gets or sets the CEFR level (A1–C2).
    /// </summary>
    public string CefrLevel
    {
        get => _cefrLevel;
        set => this.RaiseAndSetIfChanged(ref _cefrLevel, value);
    }

    /// <summary>
    /// Backing field for the sound file path or reference.
    /// </summary>
    private string _sound = string.Empty;

    /// <summary>
    /// Gets or sets the audio path/reference for the word's pronunciation.
    /// </summary>
    public string Sound
    {
        get => _sound;
        set => this.RaiseAndSetIfChanged(ref _sound, value);
    }

    /// <summary>
    /// Backing field for custom user notes.
    /// </summary>
    private string _notes = string.Empty;

    /// <summary>
    /// Gets or sets any additional arbitrary notes.
    /// </summary>
    public string Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    /// <summary>
    /// Gets the collection of available tags, loaded from Assets/tags.txt.
    /// </summary>
    public ObservableCollection<string> Tags { get; } = [];

    /// <summary>
    /// Gets the collection of available parts of speech, loaded from Assets/parts_of_speech.txt.
    /// </summary>
    public ObservableCollection<string> PartsOfSpeech { get; } = [];

    /// <summary>
    /// Gets the collection of available markers, loaded from Assets/Markers.txt.
    /// </summary>>
    public ObservableCollection<string> Markers { get; } = [];

    /// <summary>
    /// Gets the collection of available CEFR levels (A1, A2, B1, B2, C1, C2).
    /// </summary>
    public ObservableCollection<string> CefrLevels { get; } = new(CefrLevelValues);

    /// <summary>
    /// Gets the collection of cards currently saved in the deck.
    /// </summary>
    public ObservableCollection<EnglishDictionaryCard> Cards { get; } = [];

    /// <summary>
    /// Gets a value indicating whether the editor view is currently active.
    /// </summary>
    public bool IsEditorActive => true;

    /// <summary>
    /// Gets a value indicating whether the configuration view is currently active.
    /// </summary> 
    public bool IsConfigActive => false;

    /// <summary>
    /// Gets the command to navigate back to the deck selection screen.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

    /// <summary>
    /// Gets the command to clear input fields and start creating a new card.
    /// </summary> 
    public ReactiveCommand<Unit, Unit> NewCardCommand { get; }

    /// <summary>
    /// Gets the command to save the current card input into the deck.
    /// </summary> 
    public ReactiveCommand<Unit, Unit> SaveCardCommand { get; }

    /// <summary>
    /// Gets the command to navigate back to the deck selection screen. Alias for <see cref="GoBackCommand"/>.
    /// </summary> 
    public ReactiveCommand<Unit, Unit> GoToDecksCommand => GoBackCommand;

    /// <summary>
    /// Gets the command to shut down the application.
    /// </summary> 
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    /// <summary>
    /// Gets the command to switch the editor view.
    /// </summary> 
    public ReactiveCommand<Unit, Unit> EditorCommand { get; }

    /// <summary>
    /// Gets the command to navigate to the settings screen.
    /// </summary> 
    public ReactiveCommand<Unit, Unit> ConfigCommand { get; }

    /// <summary>
    /// Holds a reference to the "Save" menu item to dynamically toggle its enabled state independently.
    /// </summary>
    private readonly MenuItemViewModel _saveMenuItem;

    /// <summary>
    /// Collection of right-hand menu items representing editor actions (e.g., Save, New).
    /// Used via explicit interface implementation to prevent collision with <see cref="IHasLeftMenu.MenuItems"/>.
    /// </summary>
    private readonly ObservableCollection<MenuItemViewModel> _rightMenuItems = [];

    /// <summary>
    /// Gets the items displayed in the right menu.
    /// </summary>
    ObservableCollection<MenuItemViewModel> IHasRightMenu.MenuItems => _rightMenuItems;

    /// <summary>
    /// Collection of left-hand menu items representing main navigation (e.g., Editor, Settings, Decks, Exit).
    /// Used via explicit interface implementation to prevent collision with <see cref="IHasRightMenu.MenuItems"/>.
    /// </summary>
    private readonly ObservableCollection<MenuItemViewModel> _leftMenuItems = [];

    /// <summary>
    /// Gets the items displayed in the left menu.
    /// </summary>
    ObservableCollection<MenuItemViewModel> IHasLeftMenu.MenuItems => _leftMenuItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnglishDeckViewModel"/> class, loading option lists,
    /// setting up commands, and configuring menus.
    /// </summary>
    /// <param name="mainVm">The main window view model instance.</param>
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
            if (!ValidateBeforeSave())
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

        ConfigCommand = ReactiveCommand.Create(() => { _mainVm.CurrentScreen = new ConfigViewModel(_mainVm); });

        // --- Правое меню этого экрана ---
        // "Сохранить" стартует выключенным: нечего сохранять, пока не заполнена
        // словарная статья. Это пример пункта 3 из задачи — экран сам управляет
        // enable-состоянием своего пункта меню.
        _saveMenuItem = new MenuItemViewModel
        {
            Icon = "💾",
            Header = "Сохранить",
            ToolTip = "Сохранить карточку в колоду",
            Command = SaveCardCommand,
            IsEnabled = true 
        };
        _rightMenuItems.Add(_saveMenuItem);

        _rightMenuItems.Add(new MenuItemViewModel
        {
            Icon = "🆕",
            Header = "Новая",
            ToolTip = "Новая карточка",
            Command = NewCardCommand
        });

        // --- Левое меню этого экрана (навигация между под-экранами редактора) ---
        // IsActive проставляется один раз при создании: IsEditorActive/IsConfigActive
        // здесь константны для конкретного экрана (см. #region Flags), поэтому
        // дополнительная подписка не нужна — в отличие от "Сохранить" выше.
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

    /// <summary>
    /// Resets all input controls in the card editor to their default empty states.
    /// </summary>
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

    private bool ValidateBeforeSave()
    {
        Console.WriteLine("validation");
        var (isValid, missing) = ValidationHelper.ValidateRequiredFields([
            (nameof(DictionaryEntry), DictionaryEntry),
            (nameof(Definition), Definition),
            (nameof(Original), Original)
        ]);
        
        // [DEBUG]
        Console.WriteLine(isValid);

        return isValid;
    }
}