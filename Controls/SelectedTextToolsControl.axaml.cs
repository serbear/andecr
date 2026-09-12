using System.Windows.Input;
using andecr.ViewModels.Controls;
using Avalonia;
using Avalonia.Controls;
using ReactiveUI;

namespace andecr.Controls;

/// <summary>
/// Base implementation for a small toolbar that wraps the currently selected text of a bound
/// <see cref="TextBox"/> in a pair of tags (e.g. "&lt;MARK&gt;"/"&lt;/MARK&gt;"). Concrete, purpose-specific
/// toolbars (<see cref="TextBoxContextSelectedTextTools"/>, <see cref="TextBoxTranscriptionSelectedTextTools"/>)
/// derive from this class and simply override the default button label and tag pair via
/// <see cref="AvaloniaProperty.OverrideDefaultValue{TOwner}"/> — there is no need for a second AXAML file.
/// </summary>
public partial class SelectedTextToolsControl : UserControl
{
    /// <summary>
    /// Identifies the <see cref="ButtonLabel"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> ButtonLabelProperty =
        AvaloniaProperty.Register<SelectedTextToolsControl, string?>(nameof(ButtonLabel));

    /// <summary>
    /// Identifies the <see cref="OpenTag"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> OpenTagProperty =
        AvaloniaProperty.Register<SelectedTextToolsControl, string>(nameof(OpenTag), string.Empty);

    /// <summary>
    /// Identifies the <see cref="CloseTag"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string> CloseTagProperty =
        AvaloniaProperty.Register<SelectedTextToolsControl, string>(nameof(CloseTag), string.Empty);

    /// <summary>
    /// Identifies the <see cref="TargetTextBox"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TextBox?> TargetTextBoxProperty =
        AvaloniaProperty.Register<SelectedTextToolsControl, TextBox?>(nameof(TargetTextBox));

    public static readonly DirectProperty<SelectedTextToolsControl, bool> CanInsertMarkupProperty =
        AvaloniaProperty.RegisterDirect<SelectedTextToolsControl, bool>(
            nameof(CanInsertMarkup),
            o => o.CanInsertMarkup);

    /// <summary>
    /// The view model driving <see cref="InsertMarkupCommand"/> and selection-state tracking. Uses live
    /// property reads (<see cref="OpenTag"/>, <see cref="CloseTag"/>, <see cref="TargetTextBox"/>) rather than
    /// values captured at construction time, so it stays correct even though those properties are normally
    /// set from AXAML after this constructor has already run.
    /// </summary>
    private readonly ISelectedTextToolsViewModel _viewModel;

    /// <summary>
    /// Backing field for <see cref="CanInsertMarkup"/>.
    /// </summary>
    private bool _canInsertMarkup;

    /// <summary>
    /// Subscription tracking changes to <see cref="TargetTextBox"/>'s current selection end, kept so it can be
    /// disposed when the target changes or this control is detached.
    /// </summary>
    private IDisposable? _selectionEndSubscription;

    /// <summary>
    /// Subscription tracking changes to <see cref="TargetTextBox"/>'s current selection start, kept so it can
    /// be disposed when the target changes or this control is detached.
    /// </summary>
    private IDisposable? _selectionStartSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="SelectedTextToolsControl"/> class.
    /// </summary>
    public SelectedTextToolsControl()
    {
        InitializeComponent();

        _viewModel = new SelectedTextToolsViewModel(
            getSelectionStart: () => TargetTextBox?.SelectionStart ?? 0,
            getSelectionEnd: () => TargetTextBox?.SelectionEnd ?? 0,
            getText: () => TargetTextBox?.Text,
            setText: updatedText =>
            {
                if (TargetTextBox is not null)
                {
                    TargetTextBox.Text = updatedText;
                }
            },
            getOpenTag: () => OpenTag,
            getCloseTag: () => CloseTag);

        // Notify Avalonia's binding system when the view model's CanInsertMarkup changes.
        // Must use SetAndRaise (not a hand-built AvaloniaPropertyChangedEventArgs) so that bindings such as IsEnabled
        // actually observe the change.
        if (_viewModel is ReactiveObject reactiveVm)
        {
            reactiveVm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ISelectedTextToolsViewModel.CanInsertMarkup))
                {
                    SetAndRaise(
                        CanInsertMarkupProperty,
                        ref _canInsertMarkup,
                        _viewModel.CanInsertMarkup
                    );
                }
            };
        }
    }

    public bool CanInsertMarkup => _canInsertMarkup;

    /// <summary>
    /// Gets or sets the text displayed on the toolbar's button.
    /// </summary>
    public string? ButtonLabel
    {
        get => GetValue(ButtonLabelProperty);
        set => SetValue(ButtonLabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the tag inserted immediately before the selected text (e.g. "&lt;MARK&gt;").
    /// </summary>
    public string OpenTag
    {
        get => GetValue(OpenTagProperty);
        set => SetValue(OpenTagProperty, value);
    }

    /// <summary>
    /// Gets or sets the tag inserted immediately after the selected text (e.g. "&lt;/MARK&gt;").
    /// </summary>
    public string CloseTag
    {
        get => GetValue(CloseTagProperty);
        set => SetValue(CloseTagProperty, value);
    }

    /// <summary>
    /// Gets or sets the text field whose selection this toolbar operates on. Normally set automatically by the
    /// hosting <see cref="AndecrTextBox"/> to its own internal text box — application code does not usually
    /// need to set this directly.
    /// </summary>
    public TextBox? TargetTextBox
    {
        get => GetValue(TargetTextBoxProperty);
        set => SetValue(TargetTextBoxProperty, value);
    }

    /// <summary>
    /// Gets the command that wraps the current selection of <see cref="TargetTextBox"/> in
    /// <see cref="OpenTag"/>/<see cref="CloseTag"/>. Exposed for callers that need to trigger it externally;
    /// normal usage binds to it directly from this control's own AXAML.
    /// </summary>
    public ICommand InsertMarkupCommand => _viewModel.InsertMarkupCommand;

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TargetTextBoxProperty)
        {
            AttachToTextBox(change.GetNewValue<TextBox?>());
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DetachFromTextBox();
        base.OnDetachedFromVisualTree(e);
    }

    /// <summary>
    /// Subscribes to the given text field's selection so <see cref="ISelectedTextToolsViewModel.CanInsertMarkup"/>
    /// stays in sync, disposing any previous subscription first.
    /// </summary>
    /// <param name="textBox">The text field to observe, or <see langword="null"/> to only detach.</param>
    private void AttachToTextBox(TextBox? textBox)
    {
        DetachFromTextBox();

        if (textBox is not null)
        {
            _selectionStartSubscription = textBox
                .GetObservable(TextBox.SelectionStartProperty)
                .Subscribe(_ => _viewModel.RefreshSelectionState());
            _selectionEndSubscription = textBox
                .GetObservable(TextBox.SelectionEndProperty)
                .Subscribe(_ => _viewModel.RefreshSelectionState());
        }

        _viewModel.RefreshSelectionState();
    }

    /// <summary>
    /// Disposes any active selection subscriptions on the previously-attached text field.
    /// </summary>
    private void DetachFromTextBox()
    {
        _selectionStartSubscription?.Dispose();
        _selectionStartSubscription = null;

        _selectionEndSubscription?.Dispose();
        _selectionEndSubscription = null;
    }
}