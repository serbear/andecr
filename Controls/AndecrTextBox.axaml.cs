using andecr.ViewModels.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input.Platform;
using Avalonia.Media;

namespace andecr.Controls;

public partial class AndecrTextBox : UserControl
{
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<AndecrTextBox, string?>(nameof(Label));

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<AndecrTextBox, string?>(
            nameof(Text),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string?> WatermarkProperty =
        AvaloniaProperty.Register<AndecrTextBox, string?>(nameof(Watermark));

    public static readonly StyledProperty<bool> AcceptsReturnProperty =
        AvaloniaProperty.Register<AndecrTextBox, bool>(nameof(AcceptsReturn));

    public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
        AvaloniaProperty.Register<AndecrTextBox, TextWrapping>(
            nameof(TextWrapping),
            TextWrapping.NoWrap);

    // Отдельное имя, т.к. Height уже занят базовым Control (это высота ВНУТРЕННЕГО TextBox,
    // а не всего UserControl).
    public static readonly StyledProperty<double> FieldHeightProperty =
        AvaloniaProperty.Register<AndecrTextBox, double>(
            nameof(FieldHeight),
            double.NaN);

    // Отдельное имя, т.к. Width уже занят базовым Control (это высота ВНУТРЕННЕГО TextBox,
    // а не всего UserControl).
    public static readonly StyledProperty<double> FieldWidthProperty =
        AvaloniaProperty.Register<AndecrTextBox, double>(
            nameof(FieldWidth),
            double.NaN);
    
    // Состояние кнопки "Freeze". TwoWay по умолчанию, чтобы внешний код мог
    // и читать, и программно менять состояние заморозки.
    public static readonly StyledProperty<bool> IsFrozenProperty =
        AvaloniaProperty.Register<AndecrTextBox, bool>(
            nameof(IsFrozen),
            defaultBindingMode: BindingMode.TwoWay);

    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string? Watermark
    {
        get => GetValue(WatermarkProperty);
        set => SetValue(WatermarkProperty, value);
    }

    public bool AcceptsReturn
    {
        get => GetValue(AcceptsReturnProperty);
        set => SetValue(AcceptsReturnProperty, value);
    }

    public TextWrapping TextWrapping
    {
        get => GetValue(TextWrappingProperty);
        set => SetValue(TextWrappingProperty, value);
    }

    /// <summary>
    /// Высота внутреннего TextBox. По умолчанию NaN (автоматическая высота — для однострочных полей).
    /// </summary>
    public double FieldHeight
    {
        get => GetValue(FieldHeightProperty);
        set => SetValue(FieldHeightProperty, value);
    }
    public double FieldWidth
    {
        get => GetValue(FieldWidthProperty);
        set => SetValue(FieldWidthProperty, value);
    }

    /// <summary>
    /// Заморожено ли поле (кнопка Freeze нажата). Пока true — TextBox доступен только для чтения.
    /// </summary>
    public bool IsFrozen
    {
        get => GetValue(IsFrozenProperty);
        set => SetValue(IsFrozenProperty, value);
    }

    public AndecrTextBox()
    {
        InitializeComponent();
        
        DataContext = new AndecrTextBoxViewModel(
            getClipboardTextAsync: async () =>
            {
                var topLevel = TopLevel.GetTopLevel(this);
                var clipboard = topLevel?.Clipboard;
                return clipboard is null ? null : await clipboard.TryGetTextAsync();
            },
            setText: pastedText => Text = pastedText);
    }
}