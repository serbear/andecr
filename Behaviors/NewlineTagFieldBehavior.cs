namespace andecr.Behaviors;

using Controls;
using ViewModels;
using Avalonia;

/// <summary>
/// Keeps an <see cref="IDeckEditorViewModel"/>'s per-field "replace line breaks on export" state
/// automatically synchronized with each <see cref="AndecrTextBox.NewlineTag"/> styled property.
/// The control's own <c>NewlineTag</c> value in XAML is the single source of truth — there is no
/// separately maintained field-name list to fall out of sync.
/// </summary>
public static class NewlineTagFieldBehavior
{
    /// <summary>
    /// Identifies the attached "FieldName" property — the key under which the control's current
    /// <see cref="AndecrTextBox.NewlineTag"/> value is registered in the view model.
    /// </summary>
    public static readonly AttachedProperty<string?> FieldNameProperty =
        AvaloniaProperty.RegisterAttached<AndecrTextBox, string?>(
            "FieldName", typeof(NewlineTagFieldBehavior));

    static NewlineTagFieldBehavior()
    {
        FieldNameProperty.Changed.AddClassHandler<AndecrTextBox>((box, _) => Sync(box));
        AndecrTextBox.NewlineTagProperty.Changed.AddClassHandler<AndecrTextBox>((box, _) => Sync(box));
        StyledElement.DataContextProperty.Changed.AddClassHandler<AndecrTextBox>((box, _) => Sync(box));
    }

    public static string? GetFieldName(AndecrTextBox box) => box.GetValue(FieldNameProperty);
    public static void SetFieldName(AndecrTextBox box, string? value) => box.SetValue(FieldNameProperty, value);

    /// <summary>
    /// Pushes the control's current <see cref="AndecrTextBox.NewlineTag"/> value into the view model,
    /// keyed by <see cref="FieldNameProperty"/>. Runs whenever FieldName, NewlineTag, or DataContext
    /// changes, so it self-corrects if any of them changes later (e.g. NewlineTag toggled at runtime).
    /// </summary>
    private static void Sync(AndecrTextBox box)
    {
        var fieldName = GetFieldName(box);
        if (string.IsNullOrEmpty(fieldName))
        {
            return;
        }

        // AndecrTextBox never overrides its own DataContext (only InternalRoot's), so this is the
        // inherited outer DataContext — EnglishDeckViewModel in practice.
        if (box.DataContext is IDeckEditorViewModel vm)
        {
            vm.SetNewlineTagField(fieldName, box.NewlineTag);
        }
    }
}