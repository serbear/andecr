using System.Reactive.Linq;
using andecr.Controls;
using Avalonia;
using Avalonia.Controls;
using andecr.ViewModels;

namespace andecr.Behaviors;

/// <summary>
/// Attached behavior that links any <see cref="IFreezable"/> control's <see cref="IFreezable.IsFrozen"/>
/// state to the corresponding field's frozen flag in <see cref="EnglishDeckViewModel"/>
/// (<see cref="EnglishDeckViewModel.IsFieldFrozen"/> / <see cref="EnglishDeckViewModel.SetFieldFrozen"/>),
/// identified by <see cref="FieldNameProperty"/>.
/// </summary>
/// <remarks>
/// Works uniformly with any control implementing <see cref="IFreezable"/> (<see cref="AndecrTextBox"/>,
/// <see cref="AndecrComboBox"/>, future field controls) rather than being hard-wired to one control type.
/// Synchronization is two-way, same as before:
/// <list type="bullet">
/// <item>VM → View: as soon as the control's <see cref="Control.DataContext"/> resolves to an
/// <see cref="EnglishDeckViewModel"/>, the control's <c>IsFrozen</c> is initialized from the VM.</item>
/// <item>View → VM: every subsequent toggle of the "Freeze" button is written back into the VM via
/// <see cref="EnglishDeckViewModel.SetFieldFrozen"/>. Rather than depending on a specific control's static
/// <c>IsFrozenProperty</c> object, this listens to Avalonia's own <see cref="AvaloniaObject.PropertyChanged"/>
/// event — raised for any <see cref="AvaloniaProperty"/> change on the object — and filters by property name,
/// so it works no matter which concrete <c>StyledProperty&lt;bool&gt;</c> the control registered.</item>
/// </list>
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
        if (args.Sender is not Control control || control is not IFreezable freezable) return;

        var fieldName = args.NewValue.Value;
        if (string.IsNullOrEmpty(fieldName)) return;

        // VM -> View: pull the current frozen state of the field as soon as a suitable VM becomes available.
        control.GetObservable(Control.DataContextProperty)
            .OfType<EnglishDeckViewModel>()
            .Subscribe(vm => freezable.IsFrozen = vm.IsFieldFrozen(fieldName));

        // View -> VM: write every toggle switch change back to the VM (fetching the current DataContext at the
        // time of the event, rather than capturing it once), so that ResetEditor sees the correct state.
        Observable.FromEventPattern<EventHandler<AvaloniaPropertyChangedEventArgs>, AvaloniaPropertyChangedEventArgs>(
                h => control.PropertyChanged += h,
                h => control.PropertyChanged -= h)
            .Where(e => e.EventArgs.Property.Name == nameof(IFreezable.IsFrozen))
            .Subscribe(_ =>
            {
                if (control.DataContext is EnglishDeckViewModel vm)
                {
                    vm.SetFieldFrozen(fieldName, freezable.IsFrozen);
                }
            });
    }
}