using System.Collections.ObjectModel;
using System.Reactive;
using andecr.Models;
using andecr.Services;
using andecr.Services.DeckExport;
using andecr.Services.Text;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;

namespace andecr.ViewModels;

/// <summary>
/// Names of the editable text/selection fields of the card editor.
/// Used as keys into <see cref="EnglishDeckViewModel"/>'s indexer,
/// replacing what used to be 13 separate properties with their own backing fields.
/// </summary>
public static class CardField
{
    public const string DictionaryEntry = "DictionaryEntry";
    public const string Definition = "Definition";
    public const string Tag = "Tag";
    public const string LiteralTranslation = "LiteralTranslation";
    public const string Original = "Original";
    public const string LiteraryTranslation = "LiteraryTranslation";
    public const string PartOfSpeech = "PartOfSpeech";
    public const string BritishTranscription = "BritishTranscription";
    public const string AmericanTranscription = "AmericanTranscription";
    public const string Marker = "Marker";
    public const string CefrLevel = "CefrLevel";
    public const string Sound = "Sound";
    public const string Notes = "Notes";

    /// <summary>
    /// All field keys, in the order they are present in the export file record.
    /// In this order, the data will be concatenated into a string to be inserted into the collection export file.
    /// </summary>
    public static readonly IReadOnlyList<string> All =
    [
        DictionaryEntry,
        Definition,
        Tag,
        LiteralTranslation,
        LiteraryTranslation,
        Original,
        AmericanTranscription,
        BritishTranscription,
        PartOfSpeech,
        Marker,
        CefrLevel,
        Sound,
        Notes
    ];
}

/// <summary>
/// View model for managing and editing an English deck and its cards.
/// </summary>
/// <remarks>
/// <para>
/// This view model handles the state and logic for the deck editor screen, including:
/// <list type="bullet">
/// <item><description>Management of card fields (<see cref="CardField"/>) via an indexer</description></item>
/// <item><description>
/// Loading and providing option lists (tags, parts of speech, markers, CEFR levels)
/// </description></item>
/// <item><description>
/// Menu items for left and right navigation bars through explicit interface implementations
/// </description></item>
/// <item><description>
/// Commands for saving new cards, resetting the editor, and navigating between screens
/// </description></item>
/// </list>
/// </para>
/// <para>
/// The view model uses an indexer-based field storage system instead of individual properties,
/// allowing dynamic access to card fields by name. Each field can also be "frozen" to preserve its
/// value during reset operations.
/// </para>
/// </remarks>
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
    /// Storage for all editable card field values (Definition, Tag, DictionaryEntry, etc.),
    /// keyed by <see cref="CardField"/>. Replaces the previous 13 individual properties with
    /// their own backing fields.
    /// </summary>
    private readonly Dictionary<string, string> _fields =
        CardField.All.ToDictionary(key => key, _ => string.Empty);

    /// <summary>
    /// Storage for all editable card field 'IsFrozen' states.
    /// </summary>
    private readonly Dictionary<string, bool> _frozenStates =
        CardField.All.ToDictionary(key => key, _ => false);

    /// <summary>
    /// Collection of left-hand menu items representing main navigation (e.g., Editor, Settings, Decks, Exit).
    /// Used via explicit interface implementation to prevent collision with <see cref="IHasRightMenu.MenuItems"/>.
    /// </summary>
    private readonly ObservableCollection<MenuItemViewModel> _leftMenuItems = [];

    /// <summary>
    /// Reference to the main window view model.
    /// </summary>
    private readonly MainWindowViewModel _mainVm;

    /// <summary>
    /// Collection of right-hand menu items representing editor actions (e.g., Save, New).
    /// Used via explicit interface implementation to prevent collision with <see cref="IHasLeftMenu.MenuItems"/>.
    /// </summary>
    private readonly ObservableCollection<MenuItemViewModel> _rightMenuItems = [];

    /// <summary>
    /// Holds a reference to the "Save" menu item to dynamically toggle its enabled state independently.
    /// </summary>
    private readonly MenuItemViewModel _saveMenuItem;

    private int _selectedTabIndex;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnglishDeckViewModel"/> class.
    /// </summary>
    /// <param name="mainVm">The main window view model instance providing navigation and global state.</param>
    /// <remarks>
    /// <para>
    /// During initialization, this constructor:
    /// <list type="number">
    /// <item><description>
    /// Loads option lists from text files (tags, parts of speech, markers) using <see cref="TextOptionListLoader"/>.
    /// </description></item>
    /// <item><description>Sets up the CEFR levels collection with predefined values (A1-C2).</description></item>
    /// <item><description>
    /// Configures all commands for the editor screen, including save, new card, navigation, and exit.
    /// </description></item>
    /// <item><description>
    /// Builds left and right menu item collections with appropriate icons and commands.
    /// </description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The "Save" menu item starts in a disabled state and its enabled state is managed dynamically
    /// by the view model through validation logic in <see cref="ValidateRequiredFieldsBeforeSave"/>.
    /// </para>
    /// <para>
    /// The left menu items represent navigation between sub-screens (Editor, Settings, Decks, Exit),
    /// while the right menu items represent editor actions (Save, New).
    /// </para>
    /// </remarks>
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

        GoBackCommand = ReactiveCommand.Create(() => { _mainVm.CurrentScreen = new DeckSelectionViewModel(_mainVm); });

        NewCardCommand = ReactiveCommand.Create(ResetEditor);

        SaveCardCommand = ReactiveCommand.Create(() =>
        {
            if (!ValidateRequiredFieldsBeforeSave())
            {
                Console.WriteLine("Required fields are not fulfilled.");
                return;
            }

            // Concatenate the vocabulary card field data in a string and copy to the clipboard
            var exportFileRecordString = DeckExport.CreateDeckRecord(_fields);
            try
            {
                _ = Clipboard.CopyTextAsync(exportFileRecordString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // Cards.Add(new EnglishDictionaryCard
            // {
            //     Definition = this[CardField.Definition].Trim(),
            //     Tag = this[CardField.Tag],
            //     DictionaryEntry = this[CardField.DictionaryEntry].Trim(),
            //     LiteralTranslation = this[CardField.LiteralTranslation].Trim(),
            //     Original = this[CardField.Original].Trim(),
            //     LiteraryTranslation = this[CardField.LiteraryTranslation].Trim(),
            //     PartOfSpeech = this[CardField.PartOfSpeech],
            //     BritishTranscription = this[CardField.BritishTranscription].Trim(),
            //     AmericanTranscription = this[CardField.AmericanTranscription].Trim(),
            //     Marker = this[CardField.Marker],
            //     CefrLevel = this[CardField.CefrLevel],
            //     Sound = this[CardField.Sound].Trim(),
            //     Notes = this[CardField.Notes].Trim()
            // });

            ResetEditor();

            // Switch to the first tab.
            SelectedTabIndex = 0;
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
    /// Interaction raised whenever the marker chip list should be cleared.
    /// </summary>
    /// <remarks>
    /// The <see cref="andecr.Controls.MarkerList"/> control manages its own internal view model, so this
    /// view model cannot clear it directly. The view's code-behind must register a handler for this
    /// interaction that calls <see cref="andecr.Controls.MarkerList.ClearMarkers"/> on the control instance.
    /// </remarks>
    public Interaction<Unit, Unit> ClearMarkersInteraction { get; } = new();

    /// <summary>
    /// Gets or sets the index of the currently selected tab.
    /// </summary>
    /// <value>
    /// The zero-based index of the selected tab. Default is 0.
    /// </value>
    /// <remarks>
    /// <para>When this value changes, the property setter automatically raises the
    /// <see cref="PropertyChanged"/> event via <see cref="RaiseAndSetIfChanged"/>,
    /// allowing the UI to update in response to the new selection.</para>
    /// <para>Setting this property to a negative value or a value greater than the
    /// number of available tabs may cause an <see cref="ArgumentOutOfRangeException"/>
    /// in the bound control or view model logic. Ensure the value is within the valid range
    /// before assignment.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Switch to the second tab
    /// viewModel.SelectedTabIndex = 1;
    ///
    /// // Switch back to the first tab
    /// viewModel.SelectedTabIndex = 0;
    /// </code>
    /// </example>
    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedTabIndex, value);
    }

    /// <summary>
    /// Gets a delegate that normalizes raw text input using standard formatting rules for definition pastes.
    /// </summary>
    /// <value>
    /// A <see cref="Func{string, string}"/> delegate pointing to 
    /// <see cref="Normalizers.NormalizeDefinitionPasteTextFunc"/>.
    /// </value>
    /// <remarks>
    /// This delegate is intended for use in UI scenarios where users paste definition text 
    /// (e.g., glossary entries, term descriptions) and consistent formatting is desired.
    /// 
    /// The normalization rules are:
    /// <list type="bullet">
    /// <item><description>First character is converted to uppercase.</description></item>
    /// <item><description>A period is appended if the original text doesn't already end with one.</description></item>
    /// </list>
    /// 
    /// Null or empty input is returned as-is (no transformation applied).
    /// </remarks>
    /// <example>
    /// <code>
    /// var normalizer = NormalizeDefinitionPasteText;
    /// 
    /// string result1 = normalizer("hello world");   // "Hello world."
    /// string result2 = normalizer("Definition.");   // "Definition."
    /// string result3 = normalizer("");              // ""
    /// string result4 = normalizer(null);            // null
    /// </code>
    /// </example>
    public Func<string, string> NormalizeDefinitionPasteText { get; } =
        rawText => Normalizers.NormalizeText(
            rawText,
            [':']);

    /// <summary>
    /// Gets or sets the value of a card field identified by one of the <see cref="CardField"/> keys.
    /// Bound from AXAML via the indexer syntax, e.g. <c>{Binding [Definition]}</c>.
    /// </summary>
    /// <param name="field">One of the <see cref="CardField"/> constants.</param>
    public string this[string field]
    {
        get => _fields.TryGetValue(field, out var value) ? value : string.Empty;
        set
        {
            if (_fields.TryGetValue(field, out var current) && current == value)
                return;

            _fields[field] = new TextValueSanitizer().Sanitize(value);

            this.RaisePropertyChanged();
        }
    }

    public bool this[string field, bool isFrozen]
    {
        get => _frozenStates.TryGetValue(field, out var value) && value;
        set
        {
            if (_frozenStates.TryGetValue(field, out var current) && current == value)
                return;

            _frozenStates[field] = value;
            this.RaisePropertyChanged();
        }
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
    /// </summary>
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
    /// Gets the command to navigate back to the deck selection screen.
    /// </summary>
    public ReactiveCommand<Unit, Unit> GoBackCommand { get; }

    /// <summary>
    /// Gets a value indicating whether the editor view is currently active.
    /// </summary>
    public bool IsEditorActive => true;

    /// <summary>
    /// Gets a value indicating whether the configuration view is currently active.
    /// </summary>
    public bool IsConfigActive => false;

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
    /// Gets the items displayed in the left menu.
    /// </summary>
    ObservableCollection<MenuItemViewModel> IHasLeftMenu.MenuItems => _leftMenuItems;

    /// <summary>
    /// Gets the items displayed in the right menu.
    /// </summary>
    ObservableCollection<MenuItemViewModel> IHasRightMenu.MenuItems => _rightMenuItems;

    /// <summary>
    /// Determines whether the specified card field is currently frozen.
    /// </summary>
    /// <param name="field">The field identifier, one of the <see cref="CardField"/> constants.</param>
    /// <returns>
    /// <c>true</c> if the field is frozen; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// Frozen fields retain their values when the editor is reset via <see cref="ResetEditor"/>.
    /// </remarks>
    public bool IsFieldFrozen(string field)
    {
        return _frozenStates.TryGetValue(field, out var frozen) && frozen;
    }

    /// <summary>
    /// Sets the frozen state of a specific card field.
    /// </summary>
    /// <param name="field">The field identifier, one of the <see cref="CardField"/> constants.</param>
    /// <param name="isFrozen">
    /// <c>true</c> to freeze the field (prevent it from being cleared during reset);
    /// <c>false</c> to unfreeze it.
    /// </param>
    /// <remarks>
    /// This method is intended to be called from XAML bindings rather than directly from code.
    /// When a field is frozen, its value is preserved when <see cref="ResetEditor"/> is called.
    /// The method raises the <see cref="ReactiveObject.PropertyChanged"/> event to update the UI.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Freeze the DictionaryEntry field to keep it while clearing other fields
    /// viewModel.SetFieldFrozen(CardField.DictionaryEntry, true);
    /// </code>
    /// </example>
    public void SetFieldFrozen(string field, bool isFrozen)
    {
        if (_frozenStates.TryGetValue(field, out var current) && current == isFrozen)
        {
            return;
        }

        _frozenStates[field] = isFrozen;
        this.RaisePropertyChanged();
    }

    /// <summary>
    /// Resets all non-frozen card fields to their default empty state and clears markers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method iterates through all <see cref="CardField.All"/> fields and clears each one
    /// unless it is marked as frozen via <see cref="SetFieldFrozen"/>.
    /// </para>
    /// <para>
    /// The Marker field is handled separately: it is not a plain text field but a collection managed by
    /// the <see cref="andecr.Controls.MarkerList"/> control. When the Marker field is not frozen,
    /// the method triggers the <see cref="ClearMarkersInteraction"/> to notify the view to clear
    /// the marker chips in the UI.
    /// </para>
    /// </remarks>
    private void ResetEditor()
    {
        foreach (var field in CardField.All)
        {
            if (!IsFieldFrozen(field))
            {
                this[field] = string.Empty;
            }
        }

        // Marker field is not frozen-aware like the others (it's driven by MarkerList's own selection
        // state, not a plain text field), so it needs an explicit signal to the view to clear its chips.
        if (!IsFieldFrozen(CardField.Marker))
        {
            ClearMarkersInteraction.Handle(Unit.Default).Subscribe();
        }
    }

    /// <summary>
    /// Validates that all required fields contain non-empty values before saving.
    /// </summary>
    /// <returns>
    /// <c>true</c> if all required fields are valid; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// The required fields are:
    /// <list type="bullet">
    /// <item><description><see cref="CardField.DictionaryEntry"/></description></item>
    /// <item><description><see cref="CardField.Definition"/></description></item>
    /// <item><description><see cref="CardField.Original"/></description></item>
    /// </list>
    /// Each value is sanitized using <see cref="TextValueSanitizer"/> before validation.
    /// </remarks>
    private bool ValidateRequiredFieldsBeforeSave()
    {
        var (isValid, _) = ValidationHelper.ValidateRequiredFields([
            (CardField.DictionaryEntry, new TextValueSanitizer().Sanitize(this[CardField.DictionaryEntry])),
            (CardField.Definition, new TextValueSanitizer().Sanitize(this[CardField.Definition])),
            (CardField.Original, new TextValueSanitizer().Sanitize(this[CardField.Original]))
        ]);
        return isValid;
    }
}