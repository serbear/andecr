using System;
using System.Windows.Input;
using andecr.Services;
using andecr.ViewModels.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input.Platform;
using Avalonia.Media;

namespace andecr.Controls;

/// <summary>
/// Represents a custom text box control with integrated label, freeze feature, and clipboard paste capabilities.
/// </summary>
public partial class AndecrTextBox : UserControl
{
    /// <summary>
    /// Identifies the <see cref="Label"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<AndecrTextBox, string?>(nameof(Label));

    /// <summary>
    /// Identifies the <see cref="Text"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<AndecrTextBox, string?>(
            nameof(Text),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <see cref="Watermark"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> WatermarkProperty =
        AvaloniaProperty.Register<AndecrTextBox, string?>(nameof(Watermark));

    /// <summary>
    /// Identifies the <see cref="AcceptsReturn"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> AcceptsReturnProperty =
        AvaloniaProperty.Register<AndecrTextBox, bool>(nameof(AcceptsReturn));

    /// <summary>
    /// Identifies the <see cref="TextWrapping"/> styled property.
    /// </summary>
    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        AvaloniaProperty.Register<AndecrTextBox, TextWrapping>(
            nameof(TextWrapping),
            TextWrapping.NoWrap);

    /// <summary>
    /// Identifies the <see cref="FieldHeight"/> styled property.
    /// Distinct from <see cref="Visual.Height"/> as it specifically targets the internal input control.
    /// </summary>
    public static readonly StyledProperty<double> FieldHeightProperty =
        AvaloniaProperty.Register<AndecrTextBox, double>(
            nameof(FieldHeight),
            double.NaN);

    /// <summary>
    /// Identifies the <see cref="FieldWidth"/> styled property.
    /// Distinct from <see cref="Visual.Width"/> as it specifically targets the internal input control.
    /// </summary>
    public static readonly StyledProperty<double> FieldWidthProperty =
        AvaloniaProperty.Register<AndecrTextBox, double>(
            nameof(FieldWidth),
            double.NaN);

    /// <summary>
    /// Identifies the <see cref="IsFrozen"/> styled property.
    /// Defaults to <see cref="BindingMode.TwoWay"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsFrozenProperty =
        AvaloniaProperty.Register<AndecrTextBox, bool>(
            nameof(IsFrozen),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the header or label text displayed above the text input field.
    /// </summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the text content entered in the input field.
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder watermark text displayed when the field is empty.
    /// </summary>
    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether newline characters are inserted when pressing Enter.
    /// </summary>
    public bool AcceptsReturn
    {
        get => GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    /// <summary>
    /// Gets or sets how text wraps when it reaches the edge of the input control.
    /// </summary>
    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    /// <summary>
    /// Gets or sets the explicit height of the internal input text box.
    /// Defaults to <see cref="double.NaN"/> (auto-height).
    /// </summary>
    public double FieldHeight
    {
        get => GetValue(FieldHeightProperty);
        set => SetValue(FieldHeightProperty, value);
    }

    /// <summary>
    /// Gets or sets the explicit width of the internal input text box.
    /// Defaults to <see cref="double.NaN"/> (auto-width).
    /// </summary>
    public double FieldWidth
    {
        get => GetValue(FieldWidthProperty);
        set => SetValue(FieldWidthProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the input field is frozen (the data in the input field does not clear
    /// on a new vocabulary card creation).
    /// </summary>
    public bool IsFrozen
    {
        get => GetValue(IsFrozenProperty);
        set => SetValue(IsFrozenProperty, value);
    }

    /// <summary>
    /// Identifies the <see cref="CanPasteText"/> styled property.
    /// </summary>
    public static readonly StyledProperty<bool> CanPasteTextProperty =
        AvaloniaProperty.Register<AndecrTextBox, bool>(nameof(CanPasteText));

    /// <summary>
    /// Gets a value indicating whether non-empty text is currently available in the clipboard to be pasted.
    /// Mirrors the internal <see cref="AndecrTextBoxViewModel.CanPasteText"/> so external controls (e.g.
    /// <see cref="AndecrPasteBothButton"/>) can observe and bind to it without accessing the private
    /// <see cref="ViewModel"/>.
    /// </summary>
    public bool CanPasteText
    {
        get => GetValue(CanPasteTextProperty);
        private set => SetValue(CanPasteTextProperty, value);
    }

    /// <summary>
    /// Gets the strongly-typed view model attached as the current DataContext.
    /// </summary> 
    // private AndecrTextBoxViewModel ViewModel => (AndecrTextBoxViewModel)DataContext!;
    private readonly AndecrTextBoxViewModel _viewModel;

    private AndecrTextBoxViewModel ViewModel => _viewModel;

    /// <summary>
    /// Holds a reference to the active window subscription to ensure clean unsubscription upon removal from
    /// the visual tree.
    /// </summary>
    private WindowBase? _subscribedWindow;

    /// <summary>
    /// Initializes a new instance of the <see cref="AndecrTextBox"/> class.
    /// </summary>
    public AndecrTextBox()
    {
        InitializeComponent();

        var viewModel = new AndecrTextBoxViewModel(
            getClipboardTextAsync: async () => await Clipboard.GetTextAsync(this),
            getClipboardFormatsAsync: async () => await Clipboard.GetFormatAsync(this),
            setTextAction: pastedText => Text = pastedText
        );

        _viewModel = viewModel;


        // Mirror the view model's CanPasteText into our own public StyledProperty so external controls can
        // observe it (e.g. via ElementName bindings) without needing access to the private ViewModel.
        CanPasteText = viewModel.CanPasteText;
        viewModel.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(AndecrTextBoxViewModel.CanPasteText))
            {
                CanPasteText = viewModel.CanPasteText;
            }
        };

        // DataContext = viewModel;
        InternalRoot.DataContext = viewModel;
    }

    /// <summary>
    /// Executes the underlying paste command, pasting clipboard text into this control's text field.
    /// Intended for external callers (e.g. <see cref="AndecrPasteBothButton"/>) that need to trigger a paste
    /// without direct access to the private <see cref="ViewModel"/>. Does nothing if pasting is not currently
    /// available (see <c>CanPasteText</c>).
    /// </summary>
    public void PasteText()
    {
        // ReactiveCommand's own public CanExecute is an IObservable<bool> (used for IsEnabled bindings), not a
        // method — the actual ICommand.CanExecute/Execute are implemented explicitly, so we must go through the
        // ICommand interface to call them.
        var command = (ICommand)ViewModel.PasteTextCommand;
        if (command.CanExecute(null))
        {
            command.Execute(null);
        }
    }

    /// <summary>
    /// Handles attachment to the visual tree, subscribing to window activation events and evaluating initial
    /// clipboard state.
    /// </summary>
    /// <param name="e">Event args describing visual tree attachment.</param> 
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Activated is defined in WindowBase, not in TopLevel: on desktop, TopLevel.GetTopLevel(...) usually returns a
        // Window (which derives from WindowBase), but on single-view host platforms (Android/iOS/Browser), it might be
        // a plain TopLevel without focus-based activation — in that case, we simply don't subscribe and rely on the
        // initial check below.
        if (TopLevel.GetTopLevel(this) is WindowBase window)
        {
            _subscribedWindow = window;
            window.Activated += Window_OnActivated;
        }

        // Initial check upon adding the control to the tree — in case the clipboard already contains data and
        // the window hasn't become active since launch.
        _ = ViewModel.RefreshClipboardStateAsync();
    }

    /// <summary>
    /// Handles detachment from the visual tree, unsubscribing from window events to prevent memory leaks.
    /// </summary>
    /// <param name="e">Event args describing visual tree detachment.</param>
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_subscribedWindow is not null)
        {
            _subscribedWindow.Activated -= Window_OnActivated;
            _subscribedWindow = null;
        }

        base.OnDetachedFromVisualTree(e);
    }

    /// <summary>
    /// Event handler invoked when the parent window gains focus, triggering a clipboard state update.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void Window_OnActivated(object? sender, EventArgs e)
    {
        // The user might have copied text in another app while our window was inactive.
        // Refocusing is the only reliable yet cheap way to detect this without constantly polling the clipboard with a
        // timer (Avalonia lacks a "ClipboardChanged" event).
        _ = ViewModel.RefreshClipboardStateAsync();
    }
}