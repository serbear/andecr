using System.Reactive.Linq;
using andecr.Controls;
using Avalonia;
using Avalonia.Controls;
using andecr.ViewModels;

namespace andecr.Behaviors;

/// <summary>
/// Attached behavior that links an <see cref="AndecrTextBox"/>'s <see cref="AndecrTextBox.IsFrozen"/>
/// state to the corresponding field's frozen flag in <see cref="EnglishDeckViewModel"/>
/// (<see cref="EnglishDeckViewModel.IsFieldFrozen"/> / <see cref="EnglishDeckViewModel.SetFieldFrozen"/>),
/// identified by <see cref="FieldNameProperty"/>.
/// </summary>
/// <remarks>
/// Synchronization is two-way:
/// <list type="bullet">
/// <item>VM → View: as soon as the control's <see cref="Control.DataContext"/> resolves to an
/// <see cref="EnglishDeckViewModel"/>, the control's <c>IsFrozen</c> is initialized from the VM.</item>
/// <item>View → VM: every subsequent toggle of the "Freeze" button on the control is written back into the
/// VM via <see cref="EnglishDeckViewModel.SetFieldFrozen"/>, so methods like
/// <see cref="EnglishDeckViewModel.ResetEditor"/> that consult <c>IsFieldFrozen</c> always see the
/// up-to-date state, regardless of how it was last changed.</item>
/// </list>
/// Reacting to the <see cref="Control.DataContextProperty"/> observable (rather than reading
/// <see cref="Control.DataContext"/> once, synchronously, when <see cref="FieldNameProperty"/> changes) avoids
/// a race with DataContext inheritance, which may not have propagated yet at that point in the control's
/// initialization.
/// </remarks>
public static class FrozenBehavior
{
    public static readonly AttachedProperty<string> FieldNameProperty =
        AvaloniaProperty.RegisterAttached<object, Control, string>(
            "FieldName", typeof(FrozenBehavior).ToString());

    static FrozenBehavior()
    {
        FieldNameProperty.Changed.Subscribe(OnFieldNameChanged);
    }

    public static string GetFieldName(Control element) =>
        element.GetValue(FieldNameProperty);

    public static void SetFieldName(Control element, string value) =>
        element.SetValue(FieldNameProperty, value);

    private static void OnFieldNameChanged(AvaloniaPropertyChangedEventArgs<string> args)
    {
        if (args.Sender is not AndecrTextBox textBox) return;

        var fieldName = args.NewValue.Value;
        if (string.IsNullOrEmpty(fieldName)) return;

        // VM -> View: pull the current frozen state of the field as soon as a suitable VM becomes available.
        textBox.GetObservable(Control.DataContextProperty)
            .OfType<EnglishDeckViewModel>()
            .Subscribe(vm => textBox.IsFrozen = vm.IsFieldFrozen(fieldName));

        // View -> VM: write every toggle switch change back to the VM (fetching the current DataContext at the time of
        // the event, rather than capturing it once), so that ResetEditor sees the correct state.
        textBox.GetObservable(AndecrTextBox.IsFrozenProperty)
            .Subscribe(isFrozen =>
            {
                if (textBox.DataContext is EnglishDeckViewModel vm)
                {
                    vm.SetFieldFrozen(fieldName, isFrozen);
                }
            });
    }
}