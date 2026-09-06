using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace andecr.Controls;

/// <summary>
/// Represents a custom combo box control with an integrated label, freeze feature, and configurable
/// item source, following the same field-control pattern as <see cref="AndecrTextBox"/>.
/// </summary>
/// <remarks>
/// Implements <see cref="IFreezable"/> so that <see cref="andecr.Behaviors.FrozenBehavior"/> can link the
/// control's <see cref="IsFrozen"/> state to a named field on the hosting view model, exactly like it does
/// for <see cref="AndecrTextBox"/>.
/// </remarks>
public partial class AndecrComboBox : UserControl, IFreezable
{
    /// <summary>
    /// Identifies the <see cref="Label"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<AndecrComboBox, string?>(nameof(Label));

    /// <summary>
    /// Identifies the <see cref="IsFrozen"/> styled property.
    /// Defaults to <see cref="BindingMode.TwoWay"/>.
    /// </summary>
    public static readonly StyledProperty<bool> IsFrozenProperty =
        AvaloniaProperty.Register<AndecrComboBox, bool>(
            nameof(IsFrozen),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <see cref="ShowFreezeButton"/> styled property.
    /// Defaults to <see cref="BindingMode.TwoWay"/>.
    /// </summary>
    public static readonly StyledProperty<bool> ShowFreezeButtonProperty =
        AvaloniaProperty.Register<AndecrComboBox, bool>(
            nameof(ShowFreezeButton),
            true,
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <see cref="ItemsSource"/> styled property — the list of values
    /// offered by the embedded <see cref="ComboBox"/>.
    /// </summary>
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<AndecrComboBox, IEnumerable?>(nameof(ItemsSource));

    /// <summary>
    /// Identifies the <see cref="SelectedItem"/> styled property.
    /// Defaults to <see cref="BindingMode.TwoWay"/> so the selection flows back to the view model.
    /// </summary>
    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<AndecrComboBox, object?>(
            nameof(SelectedItem),
            defaultBindingMode: BindingMode.TwoWay);

    /// <summary>
    /// Identifies the <see cref="Placeholder"/> styled property.
    /// </summary>
    public static readonly StyledProperty<string?> PlaceholderProperty =
        AvaloniaProperty.Register<AndecrComboBox, string?>(nameof(Placeholder));

    /// <summary>
    /// Initializes a new instance of the <see cref="AndecrComboBox"/> class.
    /// </summary>
    public AndecrComboBox()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Freeze button is shown or not.
    /// </summary>
    public bool ShowFreezeButton
    {
        get => GetValue(ShowFreezeButtonProperty);
        set => SetValue(ShowFreezeButtonProperty, value);
    }

    /// <summary>
    /// Gets or sets the header or label text displayed above the combo box field.
    /// </summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    /// <summary>
    /// Gets or sets the list of values available for selection in the embedded <see cref="ComboBox"/>.
    /// </summary>
    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected value in the embedded <see cref="ComboBox"/>.
    /// </summary>
    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    /// <summary>
    /// Gets or sets the placeholder text shown when nothing is selected.
    /// </summary>
    public string? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field is frozen (its selected value does not clear
    /// on a new vocabulary card creation).
    /// </summary>
    public bool IsFrozen
    {
        get => GetValue(IsFrozenProperty);
        set => SetValue(IsFrozenProperty, value);
    }
}