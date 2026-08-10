using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace andecr.Controls;

/// <summary>
/// A button that triggers the paste command on two linked <see cref="AndecrTextBox"/> controls simultaneously.
/// </summary>
public partial class AndecrPasteBothButton : UserControl
{
    /// <summary>
    /// Identifies the <see cref="FirstTextBox"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AndecrTextBox?> FirstTextBoxProperty =
        AvaloniaProperty.Register<AndecrPasteBothButton, AndecrTextBox?>(nameof(FirstTextBox));

    /// <summary>
    /// Identifies the <see cref="SecondTextBox"/> styled property.
    /// </summary>
    public static readonly StyledProperty<AndecrTextBox?> SecondTextBoxProperty =
        AvaloniaProperty.Register<AndecrPasteBothButton, AndecrTextBox?>(nameof(SecondTextBox));

    /// <summary>
    /// Gets or sets the first <see cref="AndecrTextBox"/> that should receive the pasted clipboard text.
    /// Typically bound to a named sibling control via the <c>#Name</c> element binding syntax.
    /// </summary>
    public AndecrTextBox? FirstTextBox
    {
        get => GetValue(FirstTextBoxProperty);
        set => SetValue(FirstTextBoxProperty, value);
    }

    /// <summary>
    /// Gets or sets the second <see cref="AndecrTextBox"/> that should receive the pasted clipboard text.
    /// Typically bound to a named sibling control via the <c>#Name</c> element binding syntax.
    /// </summary>
    public AndecrTextBox? SecondTextBox
    {
        get => GetValue(SecondTextBoxProperty);
        set => SetValue(SecondTextBoxProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AndecrPasteBothButton"/> class.
    /// </summary>
    public AndecrPasteBothButton()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the "Paste Both" button click, invoking <see cref="AndecrTextBox.PasteText"/> on both
    /// linked controls. Either reference may be <c>null</c> (e.g. not yet set), in which case it is skipped.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    private void PasteBothButton_OnClick(object? sender, RoutedEventArgs e)
    {
        FirstTextBox?.PasteText();
        SecondTextBox?.PasteText();
    }
}
