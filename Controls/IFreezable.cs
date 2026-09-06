namespace andecr.Controls;

/// <summary>
/// Implemented by custom field controls (<see cref="AndecrTextBox"/>, <see cref="AndecrComboBox"/>) that
/// expose a "Freeze" toggle. Lets <see cref="andecr.Behaviors.FrozenBehavior"/> synchronize any such
/// control with <see cref="andecr.ViewModels.EnglishDeckViewModel"/> without depending on a specific
/// control type.
/// </summary>
public interface IFreezable
{
    /// <summary>
    /// Gets or sets a value indicating whether the control's value is frozen (preserved across
    /// <see cref="andecr.ViewModels.EnglishDeckViewModel.ResetEditor"/>).
    /// </summary>
    bool IsFrozen { get; set; }
}